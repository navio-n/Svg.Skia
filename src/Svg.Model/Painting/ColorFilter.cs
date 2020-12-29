﻿using Svg.Model.Painting.ColorFilters;

namespace Svg.Model.Painting
{
    public abstract class ColorFilter
    {
        public static ColorFilter CreateColorMatrix(float[] matrix)
        {
            return new ColorMatrixColorFilter
            {
                Matrix = matrix
            };
        }

        public static ColorFilter CreateTable(byte[]? tableA, byte[]? tableR, byte[]? tableG, byte[]? tableB)
        {
            return new TableColorFilter
            {
                TableA = tableA,
                TableB = tableB,
                TableG = tableG,
                TableR = tableR
            };
        }

        public static ColorFilter CreateBlendMode(Color c, BlendMode mode)
        {
            return new BlendModeColorFilter
            {
                Color = c,
                Mode = mode
            };
        }

        public static ColorFilter CreateLumaColor()
        {
            return new LumaColorColorFilter
            {
            };
        }
    }
}
