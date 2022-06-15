﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Bindables;

namespace VRCOSC.Game.Modules;

public class ModuleAttributeData
{
    public string DisplayName { get; }
    public string Description { get; }
    public Bindable<object> Attribute { get; }

    public ModuleAttributeData(string displayName, string description, object defaultValue)
    {
        DisplayName = displayName;
        Description = description;
        Attribute = new Bindable<object>(defaultValue);
    }
}
