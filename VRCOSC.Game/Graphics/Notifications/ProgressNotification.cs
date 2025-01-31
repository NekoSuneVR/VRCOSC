﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using VRCOSC.Game.Graphics.Themes;
using VRCOSC.Game.Graphics.UI;

namespace VRCOSC.Game.Graphics.Notifications;

public sealed partial class ProgressNotification : BasicNotification
{
    private float progress;

    public float Progress
    {
        set
        {
            progress = value;
            Schedule(() => progressBar.Current.Value = progress);
        }
    }

    private ProgressBar progressBar = null!;

    protected override Drawable CreateForeground()
    {
        var foreground = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, 5)
            },
            Content = new[]
            {
                new[]
                {
                    base.CreateForeground()
                },
                new Drawable[]
                {
                    progressBar = new ProgressBar
                    {
                        RelativeSizeAxes = Axes.Both,
                        BackgroundColour = ThemeManager.Current[ThemeAttribute.Darker],
                        SelectionColour = ThemeManager.Current[ThemeAttribute.Success]
                    }
                }
            }
        };

        progressBar.Current.BindValueChanged(e =>
        {
            if (Math.Abs(e.NewValue - 1f) < 0.01f) Hide();
        });

        return foreground;
    }

    protected override bool OnClick(ClickEvent e) => true;
}
