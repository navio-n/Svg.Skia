﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Svg.Skia
{
    public class CircleDrawable : DrawablePath
    {
        public CircleDrawable(SvgCircle svgCircle, SKRect skOwnerBounds, IgnoreAttributes ignoreAttributes = IgnoreAttributes.None)
        {
            IgnoreAttributes = ignoreAttributes;
            IsDrawable = CanDraw(svgCircle, IgnoreAttributes);

            if (!IsDrawable)
            {
                return;
            }

            Path = svgCircle.ToSKPath(svgCircle.FillRule, skOwnerBounds, _disposable);
            if (Path == null || Path.IsEmpty)
            {
                IsDrawable = false;
                return;
            }

            IsAntialias = SvgPaintingExtensions.IsAntialias(svgCircle);

            TransformedBounds = Path.Bounds;

            Transform = SvgTransformsExtensions.ToSKMatrix(svgCircle.Transforms);

            ClipPath = ignoreAttributes.HasFlag(IgnoreAttributes.Clip) ? null : SvgClippingExtensions.GetSvgVisualElementClipPath(svgCircle, TransformedBounds, new HashSet<Uri>(), _disposable);
            MaskDrawable = ignoreAttributes.HasFlag(IgnoreAttributes.Mask) ? null : SvgClippingExtensions.GetSvgVisualElementMask(svgCircle, TransformedBounds, new HashSet<Uri>(), _disposable);
            if (MaskDrawable != null)
            {
                CreateMaskPaints();
            }
            Opacity = ignoreAttributes.HasFlag(IgnoreAttributes.Opacity) ? null : SvgPaintingExtensions.GetOpacitySKPaint(svgCircle, _disposable);
            Filter = ignoreAttributes.HasFlag(IgnoreAttributes.Filter) ? null : SvgFiltersExtensions.GetFilterSKPaint(svgCircle, TransformedBounds, _disposable);

            if (SvgPaintingExtensions.IsValidFill(svgCircle))
            {
                Fill = SvgPaintingExtensions.GetFillSKPaint(svgCircle, TransformedBounds, ignoreAttributes, _disposable);
                if (Fill == null)
                {
                    IsDrawable = false;
                    return;
                }
            }

            if (SvgPaintingExtensions.IsValidStroke(svgCircle, TransformedBounds))
            {
                Stroke = SvgPaintingExtensions.GetStrokeSKPaint(svgCircle, TransformedBounds, ignoreAttributes, _disposable);
                if (Stroke == null)
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
