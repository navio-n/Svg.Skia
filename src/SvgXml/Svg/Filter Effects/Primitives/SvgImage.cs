﻿using System;
using Xml;

namespace Svg.FilterEffects
{
    [Element("feImage")]
    public class SvgImage : SvgFilterPrimitive, ISvgPresentationAttributes, ISvgStylableAttributes, ISvgResourcesAttributes
    {
        [Attribute("preserveAspectRatio", SvgNamespace)]
        public string? AspectRatio
        {
            get => GetAttribute("preserveAspectRatio");
            set => SetAttribute("preserveAspectRatio", value);
        }

        [Attribute("href", XLinkNamespace)]
        public string? Href
        {
            get => GetAttribute("href");
            set => SetAttribute("href", value);
        }

        public override void Print(Action<string> write, string indent)
        {
            base.Print(write, indent);

            if (AspectRatio != null)
            {
                write($"{indent}{nameof(AspectRatio)}: \"{AspectRatio}\"");
            }
            if (Href != null)
            {
                write($"{indent}{nameof(Href)}: \"{Href}\"");
            }
        }
    }
}