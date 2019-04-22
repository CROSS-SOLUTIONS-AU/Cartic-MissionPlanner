﻿using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class Brushes
    {
        public static Brush Transparent { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0x00FFFFFF) } };
        public static Brush AliceBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0F8FF) } };
        public static Brush AntiqueWhite { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFAEBD7) } };
        public static Brush Aqua { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00FFFF) } };
        public static Brush Aquamarine { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF7FFFD4) } };
        public static Brush Azure { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0FFFF) } };
        public static Brush Beige { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF5F5DC) } };
        public static Brush Bisque { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFE4C4) } };
        public static Brush Black { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush BlanchedAlmond { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFEBCD) } };
        public static Brush Blue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF0000FF) } };
        public static Brush BlueViolet { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF8A2BE2) } };
        public static Brush Brown { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFA52A2A) } };
        public static Brush BurlyWood { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDEB887) } };
        public static Brush CadetBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF5F9EA0) } };
        public static Brush Chartreuse { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF7FFF00) } };
        public static Brush Chocolate { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFD2691E) } };
        public static Brush Coral { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF7F50) } };
        public static Brush CornflowerBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF6495ED) } };
        public static Brush Cornsilk { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFF8DC) } };
        public static Brush Crimson { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDC143C) } };
        public static Brush Cyan { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00FFFF) } };
        public static Brush DarkBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00008B) } };
        public static Brush DarkCyan { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF008B8B) } };
        public static Brush DarkGoldenrod { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB8860B) } };
        public static Brush DarkGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFA9A9A9) } };
        public static Brush DarkGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF006400) } };
        public static Brush DarkKhaki { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFBDB76B) } };
        public static Brush DarkMagenta { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF8B008B) } };
        public static Brush DarkOliveGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF556B2F) } };
        public static Brush DarkOrange { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF8C00) } };
        public static Brush DarkOrchid { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF9932CC) } };
        public static Brush DarkRed { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF8B0000) } };
        public static Brush DarkSalmon { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFE9967A) } };
        public static Brush DarkSeaGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF8FBC8B) } };
        public static Brush DarkSlateBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF483D8B) } };
        public static Brush DarkSlateGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF2F4F4F) } };
        public static Brush DarkTurquoise { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00CED1) } };
        public static Brush DarkViolet { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF9400D3) } };
        public static Brush DeepPink { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF1493) } };
        public static Brush DeepSkyBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00BFFF) } };
        public static Brush DimGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF696969) } };
        public static Brush DodgerBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF1E90FF) } };
        public static Brush Firebrick { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB22222) } };
        public static Brush FloralWhite { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFAF0) } };
        public static Brush ForestGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF228B22) } };
        public static Brush Fuchsia { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF00FF) } };
        public static Brush Gainsboro { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDCDCDC) } };
        public static Brush GhostWhite { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF8F8FF) } };
        public static Brush Gold { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFD700) } };
        public static Brush Goldenrod { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDAA520) } };
        public static Brush Gray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF808080) } };
        public static Brush Green { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF008000) } };
        public static Brush GreenYellow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFADFF2F) } };
        public static Brush Honeydew { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0FFF0) } };
        public static Brush HotPink { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF69B4) } };
        public static Brush IndianRed { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFCD5C5C) } };
        public static Brush Indigo { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF4B0082) } };
        public static Brush Ivory { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFF0) } };
        public static Brush Khaki { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0E68C) } };
        public static Brush Lavender { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFE6E6FA) } };
        public static Brush LavenderBlush { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFF0F5) } };
        public static Brush LawnGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF7CFC00) } };
        public static Brush LemonChiffon { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFACD) } };
        public static Brush LightBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFADD8E6) } };
        public static Brush LightCoral { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF08080) } };
        public static Brush LightCyan { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFE0FFFF) } };
        public static Brush LightGoldenrodYellow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFAFAD2) } };
        public static Brush LightGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF90EE90) } };
        public static Brush LightGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFD3D3D3) } };
        public static Brush LightPink { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFB6C1) } };
        public static Brush LightSalmon { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFA07A) } };
        public static Brush LightSeaGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF20B2AA) } };
        public static Brush LightSkyBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF87CEFA) } };
        public static Brush LightSlateGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF778899) } };
        public static Brush LightSteelBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB0C4DE) } };
        public static Brush LightYellow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFE0) } };
        public static Brush Lime { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00FF00) } };
        public static Brush LimeGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF32CD32) } };
        public static Brush Linen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFAF0E6) } };
        public static Brush Magenta { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF00FF) } };
        public static Brush Maroon { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF800000) } };
        public static Brush MediumAquamarine { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF66CDAA) } };
        public static Brush MediumBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF0000CD) } };
        public static Brush MediumOrchid { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFBA55D3) } };
        public static Brush MediumPurple { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF9370DB) } };
        public static Brush MediumSeaGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF3CB371) } };
        public static Brush MediumSlateBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF7B68EE) } };
        public static Brush MediumSpringGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00FA9A) } };
        public static Brush MediumTurquoise { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF48D1CC) } };
        public static Brush MediumVioletRed { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFC71585) } };
        public static Brush MidnightBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF191970) } };
        public static Brush MintCream { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF5FFFA) } };
        public static Brush MistyRose { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFE4E1) } };
        public static Brush Moccasin { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFE4B5) } };
        public static Brush NavajoWhite { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFDEAD) } };
        public static Brush Navy { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000080) } };
        public static Brush OldLace { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFDF5E6) } };
        public static Brush Olive { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF808000) } };
        public static Brush OliveDrab { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF6B8E23) } };
        public static Brush Orange { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFA500) } };
        public static Brush OrangeRed { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF4500) } };
        public static Brush Orchid { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDA70D6) } };
        public static Brush PaleGoldenrod { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFEEE8AA) } };
        public static Brush PaleGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF98FB98) } };
        public static Brush PaleTurquoise { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFAFEEEE) } };
        public static Brush PaleVioletRed { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDB7093) } };
        public static Brush PapayaWhip { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFEFD5) } };
        public static Brush PeachPuff { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFDAB9) } };
        public static Brush Peru { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFCD853F) } };
        public static Brush Pink { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFC0CB) } };
        public static Brush Plum { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFDDA0DD) } };
        public static Brush PowderBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB0E0E6) } };
        public static Brush Purple { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF800080) } };
        public static Brush Red { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF0000) } };
        public static Brush RosyBrown { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFBC8F8F) } };
        public static Brush RoyalBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF4169E1) } };
        public static Brush SaddleBrown { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF8B4513) } };
        public static Brush Salmon { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFA8072) } };
        public static Brush SandyBrown { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF4A460) } };
        public static Brush SeaGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF2E8B57) } };
        public static Brush SeaShell { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFF5EE) } };
        public static Brush Sienna { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFA0522D) } };
        public static Brush Silver { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFC0C0C0) } };
        public static Brush SkyBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF87CEEB) } };
        public static Brush SlateBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF6A5ACD) } };
        public static Brush SlateGray { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF708090) } };
        public static Brush Snow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFAFA) } };
        public static Brush SpringGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF00FF7F) } };
        public static Brush SteelBlue { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF4682B4) } };
        public static Brush Tan { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFD2B48C) } };
        public static Brush Teal { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF008080) } };
        public static Brush Thistle { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFD8BFD8) } };
        public static Brush Tomato { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFF6347) } };
        public static Brush Turquoise { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF40E0D0) } };
        public static Brush Violet { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFEE82EE) } };
        public static Brush Wheat { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF5DEB3) } };
        public static Brush White { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFFF) } };
        public static Brush WhiteSmoke { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF5F5F5) } };
        public static Brush Yellow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFF00) } };
        public static Brush YellowGreen { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF9ACD32) } };
        public static Brush ActiveBorder { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB4B4B4) } };
        public static Brush ActiveCaption { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF99B4D1) } };
        public static Brush ActiveCaptionText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush AppWorkspace { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFABABAB) } };
        public static Brush ButtonFace { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0F0F0) } };
        public static Brush ButtonHighlight { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFFF) } };
        public static Brush ButtonShadow { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFA0A0A0) } };
        public static Brush Control { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0F0F0) } };
        public static Brush ControlLightLight { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFFF) } };
        public static Brush ControlLight { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFE3E3E3) } };
        public static Brush ControlDark { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFA0A0A0) } };
        public static Brush ControlDarkDark { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF696969) } };
        public static Brush ControlText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush Desktop { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush GradientActiveCaption { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFB9D1EA) } };
        public static Brush GradientInactiveCaption { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFD7E4F2) } };
        public static Brush GrayText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF6D6D6D) } };
        public static Brush Highlight { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF0078D7) } };
        public static Brush HighlightText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFFF) } };
        public static Brush HotTrack { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF0066CC) } };
        public static Brush InactiveCaption { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFBFCDDB) } };
        public static Brush InactiveBorder { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF4F7FC) } };
        public static Brush InactiveCaptionText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush Info { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFE1) } };
        public static Brush InfoText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush Menu { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0F0F0) } };
        public static Brush MenuBar { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFF0F0F0) } };
        public static Brush MenuHighlight { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF3399FF) } };
        public static Brush MenuText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };
        public static Brush ScrollBar { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFC8C8C8) } };
        public static Brush Window { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFFFFFFFF) } };
        public static Brush WindowFrame { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF646464) } };
        public static Brush WindowText { get; } = new SolidBrush() { nativeBrush = new SKPaint() { Color = new SKColor(0xFF000000) } };


    }
}