﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace VRCOSC.Game.Graphics.Containers.Screens.ModuleSelect;

public class ModuleSelectionSidePanelContent : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            new GridContainer
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new[]
                {
                    new Dimension(GridSizeMode.Absolute, 50),
                    new Dimension()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        new ModuleSearchBar
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both
                        }
                    },
                    new Drawable[]
                    {
                        new ModuleOptionContainer
                        {
                            Anchor = Anchor.TopCentre,
                            Origin = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(5)
                        }
                    }
                }
            }
        };
    }
}