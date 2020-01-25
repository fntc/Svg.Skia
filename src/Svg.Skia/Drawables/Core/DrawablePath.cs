﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using SkiaSharp;

namespace Svg.Skia
{
    public abstract class DrawablePath : Drawable
    {
        public SKPath? Path;
        public List<Drawable>? MarkerDrawables;

        protected override void Draw(SKCanvas canvas)
        {
            if (Fill != null)
            {
                canvas.DrawPath(Path, Fill);
            }

            if (Stroke != null)
            {
                canvas.DrawPath(Path, Stroke);
            }

            if (MarkerDrawables != null)
            {
                foreach (var drawable in MarkerDrawables)
                {
                    drawable.Draw(canvas, 0f, 0f);
                } 
            }
        }

        public override Drawable? HitTest(SKPoint skPoint)
        {
            if (MarkerDrawables != null)
            {
                foreach (var drawable in MarkerDrawables)
                {
                    var result = drawable.HitTest(skPoint);
                    if (result != null)
                    {
                        return result;
                    }
                } 
            }
            return base.HitTest(skPoint);
        }
    }
}
