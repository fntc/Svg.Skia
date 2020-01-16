﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Svg.Skia
{
    public class RectangleDrawable : DrawablePath
    {
        public RectangleDrawable(SvgRectangle svgRectangle, SKRect skOwnerBounds, IgnoreAttributes ignoreAttributes = IgnoreAttributes.None)
        {
            IgnoreAttributes = ignoreAttributes;
            IsDrawable = CanDraw(svgRectangle, IgnoreAttributes);

            if (!IsDrawable)
            {
                return;
            }

            Path = SKPathUtil.ToSKPath(svgRectangle, svgRectangle.FillRule, skOwnerBounds, _disposable);
            if (Path == null || Path.IsEmpty)
            {
                IsDrawable = false;
                return;
            }

            IsAntialias = SKPaintUtil.IsAntialias(svgRectangle);

            TransformedBounds = Path.Bounds;

            Transform = SKMatrixUtil.GetSKMatrix(svgRectangle.Transforms);

            PathClip = SvgClipPathUtil.GetSvgVisualElementClipPath(svgRectangle, TransformedBounds, new HashSet<Uri>(), _disposable);
            PictureMask = SvgMaskUtil.GetSvgVisualElementMask(svgRectangle, TransformedBounds, new HashSet<Uri>(), _disposable);
            CreateMaskPaints();
            PaintOpacity = ignoreAttributes.HasFlag(IgnoreAttributes.Opacity) ? null : SKPaintUtil.GetOpacitySKPaint(svgRectangle, _disposable);
            PaintFilter = ignoreAttributes.HasFlag(IgnoreAttributes.Filter) ? null : SKPaintUtil.GetFilterSKPaint(svgRectangle, TransformedBounds, _disposable);

            if (SKPaintUtil.IsValidFill(svgRectangle))
            {
                PaintFill = SKPaintUtil.GetFillSKPaint(svgRectangle, TransformedBounds, ignoreAttributes, _disposable);
                if (PaintFill == null)
                {
                    IsDrawable = false;
                    return;
                }
            }

            if (SKPaintUtil.IsValidStroke(svgRectangle, TransformedBounds))
            {
                PaintStroke = SKPaintUtil.GetStrokeSKPaint(svgRectangle, TransformedBounds, ignoreAttributes, _disposable);
                if (PaintStroke == null)
                {
                    IsDrawable = false;
                    return;
                }
            }

            // TODO: Transform _skBounds using _skMatrix.
            SKMatrix.MapRect(ref Transform, out TransformedBounds, ref TransformedBounds);
        }
    }
}
