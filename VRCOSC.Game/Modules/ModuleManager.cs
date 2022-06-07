﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CoreOSC.IO;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Platform;
using VRCOSC.Game.Util;

namespace VRCOSC.Game.Modules;

public sealed class ModuleManager : Container<ModuleGroup>
{
    private const string osc_ip_address = "127.0.0.1";
    private const int osc_send_port = 9000;
    private const int osc_receive_port = 9001;

    private UdpClient sendingClient;
    private CancellationTokenSource token;

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        List<Module> modules = ReflectiveEnumerator.GetEnumerableOfType<Module>();

        foreach (ModuleType type in Enum.GetValues(typeof(ModuleType)))
        {
            var moduleGroup = new ModuleGroup(type);

            foreach (var module in modules.Where(module => module.Type.Equals(type)))
            {
                module.DataManager = new ModuleDataManager(storage, module.GetType().Name);
                module.CreateAttributes();
                module.DataManager.LoadData();
                moduleGroup.Add(new ModuleContainer(module));
            }

            Add(moduleGroup);
        }
    }

    public void Start()
    {
        token = new CancellationTokenSource();
        sendingClient = new UdpClient(osc_ip_address, osc_send_port);

        this.ForEach(child =>
        {
            child.UpdateSendingClient(sendingClient);
            child.Start();
        });

        Task.Factory.StartNew(beginListening, TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        token.Cancel();

        this.ForEach(child => child.Stop());

        sendingClient.Dispose();
    }

    private async void beginListening()
    {
        var receivingClient = new UdpClient(osc_ip_address, osc_receive_port);

        while (!token.IsCancellationRequested)
        {
            var message = await receivingClient.ReceiveMessageAsync();
            this.ForEach(child => child.OnOSCMessage(message));
        }

        receivingClient.Dispose();
    }
}
