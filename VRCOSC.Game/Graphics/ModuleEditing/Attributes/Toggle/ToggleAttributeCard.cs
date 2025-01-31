﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using VRCOSC.Game.Graphics.Themes;
using VRCOSC.Game.Graphics.UI.Button;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Graphics.ModuleEditing.Attributes.Toggle;

public sealed partial class ToggleAttributeCard : AttributeCardSingle
{
    private ToggleButton toggleButton = null!;

    public ToggleAttributeCard(ModuleAttributeSingle attributeData)
        : base(attributeData)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(new Container
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fit,
            Padding = new MarginPadding(10),
            Child = toggleButton = new ToggleButton
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                BorderColour = ThemeManager.Current[ThemeAttribute.Border],
                BorderThickness = 2,
                ShouldAnimate = false,
                State = { Value = (bool)AttributeData.Attribute.Value }
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        toggleButton.State.ValueChanged += e => UpdateAttribute(e.NewValue);
    }

    protected override void SetDefault()
    {
        base.SetDefault();
        toggleButton.State.Value = (bool)AttributeData.Attribute.Value;
    }
}
