using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixFontOption
{
    class FixColor
    {
        public string Name { get; set; } = "";
        public List<byte?> Target { get; set; } = new List<byte?>();
        public List<byte?> Change { get; set; } = new List<byte?>();
        public List<byte?> Mask { get; set; } = new List<byte?>();
    }
    internal class ModConfig
    {
        public bool Debug { get; set; } = false;
        public bool HideShadow { get; set; } = true;
        public bool EnableFixDialogueFontLineSpace { get; set; } = false;
        public int DialogueFontLineSpace { get; set; } = 42;
        public bool EnableFixSmallFontLineSpace { get; set; } = false;
        public int SmallFontLineSpace { get; set; } = 26;
        public bool EnableFixFontPixelZoom { get; set; } = false;
        public float FontPixelZoom { get; set; } = 1f;
        public bool EnableBMFontLineSpace {get; set; } = false;
        public int BMFontLineSpace { get; set; } = 49;
        public bool EnableFixFontColor { get; set; } = false;
        public List<FixColor> FontColor { get; set; } = new List<FixColor>();
        public bool EnableFixImgColor { get; set; } = false;
        public List<FixColor> ImgColor { get; set; } = new List<FixColor>();
    }
}
