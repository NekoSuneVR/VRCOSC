﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using System.Collections.Generic;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using VRCOSC.Game.Modules.Frameworks;

namespace VRCOSC.Game.Modules.Modules.Spotify;

public class SpotifyModule : IntegrationModule
{
    public override string Title => "Spotify";
    public override string Description => "Integration with the Spotify desktop app";
    public override string Author => "VolcanicArts";
    public override Colour4 Colour => Color4Extensions.FromHex(@"1ed760").Darken(0.5f);
    public override ModuleType Type => ModuleType.Integrations;
    public override string TargetProcess => "spotify";
    public override string TargetExe => GetSetting<string>(SpotifySettings.InstallLocation);

    protected override Dictionary<Enum, WindowsVKey[]> KeyCombinations => new()
    {
        { SpotifyInputParameters.SpotifyPlayPause, new[] { WindowsVKey.VK_SPACE } },
        { SpotifyInputParameters.SpotifyNext, new[] { WindowsVKey.VK_LCONTROL, WindowsVKey.VK_RIGHT } },
        { SpotifyInputParameters.SpotifyPrevious, new[] { WindowsVKey.VK_LCONTROL, WindowsVKey.VK_LEFT } },
        { SpotifyInputParameters.SpotifyVolumeUp, new[] { WindowsVKey.VK_LCONTROL, WindowsVKey.VK_UP } },
        { SpotifyInputParameters.SpotifyVolumeDown, new[] { WindowsVKey.VK_LCONTROL, WindowsVKey.VK_DOWN } }
    };

    public override void CreateAttributes()
    {
        CreateSetting(SpotifySettings.ShouldStart, "Should Start", "Should Spotify start on module run", false);
        CreateSetting(SpotifySettings.InstallLocation, "Install Location", "The location of your spotify.exe file", $@"C:\Users\{Environment.UserName}\AppData\Roaming\Spotify\spotify.exe");

        RegisterInputParameter(SpotifyInputParameters.SpotifyPlayPause, typeof(bool));
        RegisterInputParameter(SpotifyInputParameters.SpotifyNext, typeof(bool));
        RegisterInputParameter(SpotifyInputParameters.SpotifyPrevious, typeof(bool));
        RegisterInputParameter(SpotifyInputParameters.SpotifyVolumeUp, typeof(bool));
        RegisterInputParameter(SpotifyInputParameters.SpotifyVolumeDown, typeof(bool));
    }

    public override void Start()
    {
        var shouldStart = GetSetting<bool>(SpotifySettings.ShouldStart);
        if (shouldStart) StartTarget();
    }

    protected override void OnBoolParameterReceived(Enum key, bool value)
    {
        if (!value) return;

        Terminal.Log($"Received input of {key}");

        ExecuteShortcut(key);
    }
}

public enum SpotifySettings
{
    ShouldStart,
    InstallLocation
}

public enum SpotifyInputParameters
{
    SpotifyPlayPause,
    SpotifyNext,
    SpotifyPrevious,
    SpotifyVolumeUp,
    SpotifyVolumeDown
}
