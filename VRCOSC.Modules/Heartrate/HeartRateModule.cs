// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.Game.Modules;

namespace VRCOSC.Modules.Heartrate;

public abstract partial class HeartRateModule : ChatBoxModule
{
    private static readonly TimeSpan heartrate_timeout = TimeSpan.FromSeconds(10);

    public override string Author => "VolcanicArts";
    public override string Prefab => "VRCOSC-Heartrate";
    public override ModuleType Type => ModuleType.Health;
    protected override TimeSpan DeltaUpdate => TimeSpan.FromSeconds(2);
    protected override int ChatBoxPriority => 1;

    protected override bool DefaultChatBoxDisplay => false;
    protected override string DefaultChatBoxFormat => "Heartrate                                        %hr% bpm";
    protected override IEnumerable<string> ChatBoxFormatValues => new[] { "%hr%" };

    protected HeartRateProvider? HeartRateProvider;
    private int lastHeartrate;
    private DateTimeOffset lastHeartrateTime;
    private int connectionCount;

    private bool isReceiving => lastHeartrateTime + heartrate_timeout >= DateTimeOffset.Now;

    protected abstract HeartRateProvider CreateHeartRateProvider();

    protected override void CreateAttributes()
    {
        base.CreateAttributes();
        CreateParameter<bool>(HeartrateParameter.Enabled, ParameterMode.Write, "VRCOSC/Heartrate/Enabled", "Enabled", "Whether this module is attempting to emit values");
        CreateParameter<float>(HeartrateParameter.Normalised, ParameterMode.Write, "VRCOSC/Heartrate/Normalised", "Normalised", "The heartrate value normalised to 240bpm");
        CreateParameter<float>(HeartrateParameter.Units, ParameterMode.Write, "VRCOSC/Heartrate/Units", "Units", "The units digit 0-9 mapped to a float");
        CreateParameter<float>(HeartrateParameter.Tens, ParameterMode.Write, "VRCOSC/Heartrate/Tens", "Tens", "The tens digit 0-9 mapped to a float");
        CreateParameter<float>(HeartrateParameter.Hundreds, ParameterMode.Write, "VRCOSC/Heartrate/Hundreds", "Hundreds", "The hundreds digit 0-9 mapped to a float");
    }

    protected override string GetChatBoxText() => GetSetting<string>(ChatBoxSetting.ChatBoxFormat).Replace("%hr%", lastHeartrate.ToString());

    protected override void OnModuleStart()
    {
        base.OnModuleStart();
        attemptConnection();

        lastHeartrateTime = DateTimeOffset.Now - heartrate_timeout;
    }

    private void attemptConnection()
    {
        if (connectionCount >= 3)
        {
            Log("Connection cannot be established");
            return;
        }

        connectionCount++;
        HeartRateProvider = CreateHeartRateProvider();
        HeartRateProvider.OnHeartRateUpdate += HandleHeartRateUpdate;
        HeartRateProvider.OnConnected += () => connectionCount = 0;
        HeartRateProvider.OnDisconnected += () =>
        {
            Task.Run(async () =>
            {
                if (IsStopping || HasStopped) return;

                SendParameter(HeartrateParameter.Enabled, false);
                await Task.Delay(2000);
                attemptConnection();
            });
        };
        HeartRateProvider.Initialise();
        HeartRateProvider.Connect();
    }

    protected override void OnModuleStop()
    {
        if (HeartRateProvider is null) return;

        if (connectionCount < 3) HeartRateProvider.Disconnect();
        SendParameter(HeartrateParameter.Enabled, false);
    }

    protected override void OnModuleUpdate()
    {
        if (!isReceiving) SendParameter(HeartrateParameter.Enabled, false);
    }

    protected virtual void HandleHeartRateUpdate(int heartrate)
    {
        lastHeartrate = heartrate;
        lastHeartrateTime = DateTimeOffset.Now;

        var normalisedHeartRate = heartrate / 240.0f;
        var individualValues = toDigitArray(heartrate, 3);

        SendParameter(HeartrateParameter.Enabled, true);
        SendParameter(HeartrateParameter.Normalised, normalisedHeartRate);
        SendParameter(HeartrateParameter.Units, individualValues[2] / 10f);
        SendParameter(HeartrateParameter.Tens, individualValues[1] / 10f);
        SendParameter(HeartrateParameter.Hundreds, individualValues[0] / 10f);
    }

    private static int[] toDigitArray(int num, int totalWidth)
    {
        return num.ToString().PadLeft(totalWidth, '0').Select(digit => int.Parse(digit.ToString())).ToArray();
    }

    protected enum HeartrateParameter
    {
        Enabled,
        Normalised,
        Units,
        Tens,
        Hundreds
    }
}
