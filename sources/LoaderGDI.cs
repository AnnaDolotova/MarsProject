using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

//3DEM program exports 8/16-bit Textures

namespace DrawHeightmapGL
{
    class ImageGDI
    {

        public static void LoadFromDisk(string filename, out uint texturehandle, out OpenTK.Graphics.OpenGL.TextureTarget dimension, out int Width, out int Height)
        {
            try //exceptions if any problem occurs working on the file. 
            {
                CurrentBitmap = new Bitmap(filename);

                Width = CurrentBitmap.Width;
                Height = CurrentBitmap.Height;

                if (CurrentBitmap.Height > 1)
                    dimension = OpenTK.Graphics.OpenGL.TextureTarget.Texture2D;
                else
                    dimension = OpenTK.Graphics.OpenGL.TextureTarget.Texture1D;

                GL.GenTextures( 1, out texturehandle );
                GL.BindTexture( dimension, texturehandle );

                #region Load Texture

                OpenTK.Graphics.OpenGL.PixelInternalFormat pif; //The internal format with which the data will be stored in the buffer object
                OpenTK.Graphics.OpenGL.PixelFormat pf; //The format of the data in memory addressed by data
                OpenTK.Graphics.OpenGL.PixelType pt; //The type of the data whose address in memory is given by data

                if (TextureLoaderParameters.Verbose)  //will be declared in static parametres like false
                   Trace.WriteLine( "File: " + filename + " Format: " + CurrentBitmap.PixelFormat );

                switch ( CurrentBitmap.PixelFormat ) //redo PixelFormat
                {
                    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed: // setup Specifies that the format is 8 bits per pixel, indexed. The color table therefore has 256 colors in it.
                    case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555: //The pixel format is 16 bits per pixel. The color information specifies 32,768 shades of color, of which 5 bits are red, 5 bits are green, 5 bits are blue, and 1 bit is alpha.
                    case System.Drawing.Imaging.PixelFormat.Format16bppRgb555: // Specifies that the format is 16 bits per pixel; 5 bits each are used for the red, green, and blue components. The remaining bit is not used.
                    case System.Drawing.Imaging.PixelFormat.Format16bppRgb565: //Specifies that the format is 16 bits per pixel; 5 bits are used for the red component, 6 bits are used for the green component, and 5 bits are used for the blue component.
                    case System.Drawing.Imaging.PixelFormat.Format24bppRgb: //Specifies that the format is 24 bits per pixel; 8 bits each are used for the red, green, and blue components.
                    case System.Drawing.Imaging.PixelFormat.Format32bppRgb: //Specifies that the format is 32 bits per pixel; 8 bits each are used for the red, green, and blue components. The remaining 8 bits are not used.
                case System.Drawing.Imaging.PixelFormat.Canonical: //The default pixel format of 32 bits per pixel. The format specifies 24-bit color depth and an 8-bit alpha channel.
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb: //Specifies that the format is 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue components.

                default:
                    throw new ArgumentException( "ERROR: Unsupported Pixel Format " + CurrentBitmap.PixelFormat );
                }

                BitmapData Data = CurrentBitmap.LockBits( new System.Drawing.Rectangle( 0, 0, CurrentBitmap.Width, CurrentBitmap.Height ), ImageLockMode.ReadOnly, CurrentBitmap.PixelFormat );

               //then I have to find cases, when image is 1D or 2D
                #endregion Load Texture
                
                #region Set Texture Parameters
                //there will be some parameters

                GLError = GL.GetError( ); 
                if ( GLError != ErrorCode.NoError )
                {
                    throw new ArgumentException( "Error setting Texture Parameters. GL Error: " + GLError );
                }
                #endregion Set Texture Parameters

                return; // success
            } 
            
            catch ( Exception e )
            {
                dimension = (TextureTarget) 0;
                texturehandle = TextureLoaderParameters.OpenGLDefaultTexture;
                throw new ArgumentException( "Texture Loading Error: Failed to read file " + filename + ".\n" + e );
                // return; // failure
            }
          
            finally
            {
                CurrentBitmap = null;
            }
        }

    }
}