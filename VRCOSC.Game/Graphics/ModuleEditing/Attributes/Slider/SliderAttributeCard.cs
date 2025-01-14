﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using VRCOSC.Game.Graphics.Themes;
using VRCOSC.Game.Graphics.UI;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Graphics.ModuleEditing.Attributes.Slider;

public abstract partial class SliderAttributeCard<T> : AttributeCardSingle where T : struct, IComparable<T>, IConvertible, IEquatable<T>
{
    protected ModuleAttributeSingleWithBounds AttributeDataWithBounds;

    private VRCOSCSlider<T> slider = null!;

    protected SliderAttributeCard(ModuleAttributeSingleWithBounds attributeData)
        : base(attributeData)
    {
        AttributeDataWithBounds = attributeData;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        ContentFlow.Add(slider = new VRCOSCSlider<T>
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            RelativeSizeAxes = Axes.X,
            Height = 40,
            BorderColour = ThemeManager.Current[ThemeAttribute.Border],
            BorderThickness = 2,
            Current = CreateCurrent()
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        slider.Current.ValueChanged += e => UpdateAttribute(e.NewValue);
    }

    protected override void SetDefault()
    {
        base.SetDefault();
        slider.Current.Value = (T)AttributeData.Attribute.Value;
    }

    protected abstract Bindable<T> CreateCurrent();
}
