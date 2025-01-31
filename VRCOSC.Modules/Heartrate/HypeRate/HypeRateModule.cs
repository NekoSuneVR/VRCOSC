﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.Game;

namespace VRCOSC.Modules.Heartrate.HypeRate;

public sealed partial class HypeRateModule : HeartRateModule
{
    public override string Title => "HypeRate";
    public override string Description => "Connects to HypeRate.io and sends your heartrate to VRChat";
    protected override TimeSpan DeltaUpdate => TimeSpan.FromSeconds(10);

    private bool receivedHeartRate;

    protected override HeartRateProvider CreateHeartRateProvider() => new HypeRateProvider(GetSetting<string>(HypeRateSetting.Id), Secrets.GetSecret(VRCOSCSecretsKeys.Hyperate));

    protected override void CreateAttributes()
    {
        CreateSetting(HypeRateSetting.Id, "HypeRate ID", "Your HypeRate ID given on your device", string.Empty);
        base.CreateAttributes();
    }

    protected override void OnModuleStart()
    {
        var hypeRateId = GetSetting<string>(HypeRateSetting.Id);

        if (string.IsNullOrEmpty(hypeRateId))
        {
            Log("Cannot connect to HypeRate. Please enter an Id");
            return;
        }

        SendParameter(HeartrateParameter.Enabled, false);

        base.OnModuleStart();
    }

    protected override void OnModuleUpdate()
    {
        if (!(HeartRateProvider?.IsConnected ?? false)) return;

        ((HypeRateProvider)HeartRateProvider).SendWsHeartBeat();
        if (!receivedHeartRate) SendParameter(HeartrateParameter.Enabled, false);
        receivedHeartRate = false;
    }

    protected override void HandleHeartRateUpdate(int heartrate)
    {
        base.HandleHeartRateUpdate(heartrate);
        receivedHeartRate = true;
    }

    private enum HypeRateSetting
    {
        Id
    }
}
