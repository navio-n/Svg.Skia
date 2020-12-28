﻿
using Svg.Model.Paint;

namespace Svg.Model.ColorFilters
{
    public sealed class BlendModeColorFilter : ColorFilter
    {
        public Color Color { get; set; }
        public BlendMode Mode { get; set; }
    }
}