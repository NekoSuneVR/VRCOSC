﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Bindables;
using VRCOSC.Game.Modules;

namespace VRCOSC.Game.Graphics.ModuleEditing.Attributes;

public abstract class AttributeCardSingle : AttributeCard
{
    protected readonly ModuleAttributeSingle AttributeData;

    protected AttributeCardSingle(ModuleAttributeSingle attributeData)
        : base(attributeData)
    {
        AttributeData = attributeData;
    }

    protected override void LoadComplete()
    {
        AttributeData.Attribute.BindValueChanged(e => Schedule(performAttributeUpdate, e), true);
    }

    private void performAttributeUpdate(ValueChangedEvent<object> e)
    {
        UpdateValues(e.NewValue);
        UpdateResetToDefault(!AttributeData.IsDefault());
    }

    protected virtual void UpdateValues(object value)
    {
        AttributeData.Attribute.Value = value;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        AttributeData.Attribute.ValueChanged -= performAttributeUpdate;
    }
}
