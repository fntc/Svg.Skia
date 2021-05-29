﻿using System;
using System.Collections.Generic;
using Svg.Model.Primitives;

namespace Svg.Model.Drawables.Elements
{
    public sealed class MaskDrawable : DrawableContainer
    {
        private MaskDrawable(IAssetLoader assetLoader)
            : base(assetLoader)
        {
        }

        public static MaskDrawable Create(SvgMask svgMask, SKRect skOwnerBounds, DrawableBase? parent, IAssetLoader assetLoader, DrawAttributes ignoreAttributes = DrawAttributes.None)
        {
            var drawable = new MaskDrawable(assetLoader)
            {
                Element = svgMask,
                Parent = parent,
                IgnoreAttributes = ignoreAttributes,
                IsDrawable = true
            };

            if (!drawable.IsDrawable)
            {
                return drawable;
            }

            var maskUnits = svgMask.MaskUnits;
            var maskContentUnits = svgMask.MaskContentUnits;
            var xUnit = svgMask.X;
            var yUnit = svgMask.Y;
            var widthUnit = svgMask.Width;
            var heightUnit = svgMask.Height;

            // TODO: Pass correct skViewport
            var skRectTransformed = SvgModelExtensions.CalculateRect(xUnit, yUnit, widthUnit, heightUnit, maskUnits, skOwnerBounds, skOwnerBounds, svgMask);
            if (skRectTransformed is null)
            {
                drawable.IsDrawable = false;
                return drawable;
            }
   
            var skMatrix = SKMatrix.CreateIdentity();

            if (maskContentUnits == SvgCoordinateUnits.ObjectBoundingBox)
            {
                var skBoundsTranslateTransform = SKMatrix.CreateTranslation(skOwnerBounds.Left, skOwnerBounds.Top);
                skMatrix = skMatrix.PreConcat(skBoundsTranslateTransform);

                var skBoundsScaleTransform = SKMatrix.CreateScale(skOwnerBounds.Width, skOwnerBounds.Height);
                skMatrix = skMatrix.PreConcat(skBoundsScaleTransform);
            }

            drawable.CreateChildren(svgMask, skOwnerBounds, drawable, assetLoader, ignoreAttributes);

            drawable.Overflow = skRectTransformed;

            drawable.IsAntialias = SvgModelExtensions.IsAntialias(svgMask);

            drawable.GeometryBounds = skRectTransformed.Value;

            drawable.TransformedBounds = drawable.GeometryBounds;

            drawable.Transform = skMatrix;

            // TODO: Transform _skBounds using _skMatrix.
            drawable.TransformedBounds = drawable.Transform.MapRect(drawable.TransformedBounds);

            drawable.Fill = null;
            drawable.Stroke = null;

            return drawable;
        }

        public override void PostProcess(SKRect? viewport)
        {
            var element = Element;
            if (element is null)
            {
                return;
            }

            var enableMask = !IgnoreAttributes.HasFlag(DrawAttributes.Mask);

            ClipPath = null;

            if (enableMask)
            {
                MaskDrawable = SvgModelExtensions.GetSvgElementMask(element, TransformedBounds, new HashSet<Uri>(), AssetLoader);
                if (MaskDrawable is { })
                {
                    CreateMaskPaints();
                }
            }
            else
            {
                MaskDrawable = null;
            }

            Opacity = null;
            Filter = null;
        }
    }
}
