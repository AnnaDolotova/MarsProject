using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApplication4
{
    class PixelFormat
    {
       public OpenTK.Graphics.OpenGL.PixelInternalFormat pif;
       public OpenTK.Graphics.OpenGL.PixelFormat pf;
       public OpenTK.Graphics.OpenGL.PixelType pt;
    }

    class Format8bppIndexed : PixelFormat
    {
        public Format8bppIndexed(out OpenTK.Graphics.OpenGL.PixelInternalFormat pif, out OpenTK.Graphics.OpenGL.PixelFormat pf, out OpenTK.Graphics.OpenGL.PixelType pt)
        {
            pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb8;
            pf = OpenTK.Graphics.OpenGL.PixelFormat.ColorIndex;
            pt = OpenTK.Graphics.OpenGL.PixelType.Bitmap;
        }
    }

    class Format16bppRgb555 : PixelFormat
    {
        public Format16bppRgb555(out OpenTK.Graphics.OpenGL.PixelInternalFormat pif, out OpenTK.Graphics.OpenGL.PixelFormat pf, out OpenTK.Graphics.OpenGL.PixelType pt)
        {
            pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb5A1;
            pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
            pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedShort5551Ext;
        }
    }

    class Format24bppRgb : PixelFormat
    {
        public Format24bppRgb(out OpenTK.Graphics.OpenGL.PixelInternalFormat pif, out OpenTK.Graphics.OpenGL.PixelFormat pf, out OpenTK.Graphics.OpenGL.PixelType pt)
        {
            pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb8;
            pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
            pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
        }
    }

    class Format32bppArgb : PixelFormat
    {
        public Format32bppArgb(out OpenTK.Graphics.OpenGL.PixelInternalFormat pif, out OpenTK.Graphics.OpenGL.PixelFormat pf, out OpenTK.Graphics.OpenGL.PixelType pt)
        {
            pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba;
            pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
        }
    }
}
