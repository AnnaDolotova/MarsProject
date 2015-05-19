#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

// Export 8/16-bit Textures and make sure they are loaded correctly.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApplication4
{
    class ImageGDI
    {

        public static void LoadFromDisk( string filename, out uint texturehandle,
            out OpenTK.Graphics.OpenGL.TextureTarget dimension, out int Width, out int Height)
        {
            dimension = (OpenTK.Graphics.OpenGL.TextureTarget)0;
            texturehandle = TextureLoaderParameters.OpenGLDefaultTexture;
            ErrorCode GLError = ErrorCode.NoError;

            Bitmap CurrentBitmap = null;

            try // Exceptions will be thrown if any Problem occurs while working on the file. 
            {
                CurrentBitmap = new Bitmap( filename );

                Width = CurrentBitmap.Width;
                Height = CurrentBitmap.Height;

                dimension = OpenTK.Graphics.OpenGL.TextureTarget.Texture2D;

                GL.GenTextures(1, out texturehandle); //������ ���� ��� ��� ����������� ������� � ���������� ��� � ������
                GL.BindTexture(dimension, texturehandle); //������ � ��������� ���������� ������ � ����������� ���������� ��������  

                #region Load Texture
                OpenTK.Graphics.OpenGL.PixelInternalFormat pif;
                OpenTK.Graphics.OpenGL.PixelFormat pf;
                OpenTK.Graphics.OpenGL.PixelType pt;

                switch (CurrentBitmap.PixelFormat)
                {
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed: 
                    pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb8;
                    pf = OpenTK.Graphics.OpenGL.PixelFormat.ColorIndex;
                    pt = OpenTK.Graphics.OpenGL.PixelType.Bitmap;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555: 
                    pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb5A1;
                    pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
                    pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedShort5551Ext;
                    break;


                case System.Drawing.Imaging.PixelFormat.Format24bppRgb: 
                    pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb8;
                    pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgr;
                    pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
                    break;

                case System.Drawing.Imaging.PixelFormat.Format32bppArgb: 
                    pif = OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba;
                    pf = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                    pt = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
                    break;

                default:
                    throw new ArgumentException( "ERROR: Unsupported Pixel Format " + CurrentBitmap.PixelFormat );
                }

                //�������������� ����� ��������� ������ Rectangle � �������� ������������� � ��������. 
                //LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format) ��������� ������ Bitmap � ��������� ������. 
                //������ BitmapData ���������� �������� ������� Bitmap, ����� ��� ������, ������ ��������, ��������� ����� ������ ����� � ������ � ����� ������ ������ ���������.

                BitmapData Data = CurrentBitmap.LockBits(new System.Drawing.Rectangle( 0, 0, CurrentBitmap.Width, CurrentBitmap.Height), ImageLockMode.ReadOnly, CurrentBitmap.PixelFormat );

                // image is 2D
                if (TextureLoaderParameters.BuildMipmapsForUncompressed)
                {
                    throw new Exception("Cannot build mipmaps, Glu is deprecated.");
                }
                else
                    //Data.Scan0 - ����� ������ ������� ������� � �������� �������
                    //��������� �������� � ����������� ��������� �� ����������
                    GL.TexImage2D(dimension, 0, pif, Data.Width, Data.Height, TextureLoaderParameters.Border, pf, pt, Data.Scan0);

                GL.Finish( );
                GLError = GL.GetError( );
                if (GLError != ErrorCode.NoError)
                {
                    throw new ArgumentException( "Error building TexImage. GL Error: " + GLError );
                }

                CurrentBitmap.UnlockBits(Data); //��������������� �������

                #endregion Load Texture
                
                #region Set Texture Parameters
                //�������� ���������� ��������� ��������, ���� ��� ������ ��� ������ ������� �������
                //������ �������� ��������� �������� ����� �� �������� ���� ���� �������, ����� �������� ��������� ����� ����� � �����, � ������� ������� ����� �������� ��������.
                GL.TexParameter(dimension, TextureParameterName.TextureMinFilter, (int)TextureLoaderParameters.MinificationFilter);
                GL.TexParameter(dimension, TextureParameterName.TextureMagFilter, (int)TextureLoaderParameters.MagnificationFilter);
                //����������� ��������� ����� ���� s � t
                GL.TexParameter(dimension, TextureParameterName.TextureWrapS, (int)TextureLoaderParameters.WrapModeS);
                GL.TexParameter(dimension, TextureParameterName.TextureWrapT, (int)TextureLoaderParameters.WrapModeT);
                //������ ������ ���������� ���������������� �������� �������� ������� ��������, ������ ������� ��������� ����� ����� � �����, � ������� ������� ����� �������� ��������.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);		// Linear Filtering
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);		// Linear Filtering

                //����� ��������, ����� ���� ���������� ����� ������������

                GLError = GL.GetError( );
                if ( GLError != ErrorCode.NoError )
                {
                    throw new ArgumentException( "Error setting Texture Parameters. GL Error: " + GLError );
                }
                #endregion Set Texture Parameters

                return; // success
            } catch ( Exception e )
            {
                dimension = (TextureTarget) 0;
                texturehandle = TextureLoaderParameters.OpenGLDefaultTexture;
                throw new ArgumentException( "Texture Loading Error: Failed to read file " + filename + ".\n" + e );
                // return; // failure
            } finally
            {
                CurrentBitmap = null;
            }
        }

    }
}