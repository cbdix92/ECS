using System;
using System.Runtime.InteropServices;

namespace CMDR.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct GLYPHMETRICSFLOAT
    {
        public float gmfBlackBoxX;
        public float gmfBlackBoxY;
        public PointFloat gmfptGlyphOrigin;
        public float gmfCellIncX;
        public float gmfCellIncY;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PointFloat
    {
        readonly float X;
        readonly float Y;
    }
}