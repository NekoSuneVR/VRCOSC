﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Squirrel;
using VRCOSC.Game.Graphics.Updater;

namespace VRCOSC.Desktop.Updater;

public class SquirrelUpdateManager : VRCOSCUpdateManager
{
    private GithubUpdateManager updateManager;
    private UpdateInfo updateInfo;

    [Resolved]
    private GameHost host { get; set; }

    public override async void CheckForUpdate(bool useDelta = true)
    {
        try
        {
            Logger.Log("Attempting to find update...");
            updateManager ??= new GithubUpdateManager(@"https://github.com/VolcanicArts/VRCOSC");

            if (!updateManager.IsInstalledApp)
            {
                Logger.Log("Cannot update. Not installed app");
                return;
            }

            Logger.Log("Checking for update...");
            updateInfo = await updateManager.CheckForUpdate(!useDelta).ConfigureAwait(false);

            if (updateInfo.ReleasesToApply.Count == 0) return;

            Logger.Log("Found updates to apply!");

            Show();

            SetPhase(UpdatePhase.Download);
            await updateManager.DownloadReleases(updateInfo.ReleasesToApply, progress => UpdateProgress(progress / 100f)).ConfigureAwait(false);

            SetPhase(UpdatePhase.Install);
            await updateManager.ApplyReleases(updateInfo, progress => UpdateProgress(progress / 100f)).ConfigureAwait(false);

            SetPhase(UpdatePhase.Success);
        }
        catch (Exception)
        {
            //delta update may have failed due to the installed version being too outdated. Retry without trying for delta
            if (useDelta)
            {
                CheckForUpdate(false);
                return;
            }

            SetPhase(UpdatePhase.Fail);
            throw;
        }
    }

    public override void RequestRestart()
    {
        UpdateManager.RestartAppWhenExited().ContinueWith(_ => host.Exit()).ConfigureAwait(false);
    }
}
