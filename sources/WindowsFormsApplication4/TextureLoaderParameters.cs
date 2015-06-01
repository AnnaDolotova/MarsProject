#region --- License ---
/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing details.
 */
#endregion

using System;

using OpenTK.Graphics.OpenGL;

namespace WindowsFormsApplication4
{

    //The parameters in this class have only effect on the following Texture loads.
    public static class TextureLoaderParameters
    {
        //Always-valid fallback parameter for GL.BindTexture (Default: 0). This number will be returned if loading the Texture failed. You can set this to a checkerboard texture or similar, which you have already loaded.
        public static uint OpenGLDefaultTexture = 0;

        //Compressed formats must have a border of 0, so this is constant.
        public const int Border = 0;

        /// <summary>false==DirectX TexCoords, true==OpenGL TexCoords (Default: true)</summary>
        public static bool FlipImages = true;

        //When enabled, will use Glu to create MipMaps for images loaded with GDI+ (Default: false)
        public static bool BuildMipmapsForUncompressed = false;

        //Selects the Magnification filter for following Textures to be loaded. (Default: Nearest)
        public static TextureMagFilter MagnificationFilter = TextureMagFilter.Nearest;

        //Selects the Minification filter for following Textures to be loaded. (Default: Nearest)
        public static TextureMinFilter MinificationFilter = TextureMinFilter.Nearest;

        //Selects the S Wrapping for following Textures to be loaded. (Default: Repeat)
        public static TextureWrapMode WrapModeS = TextureWrapMode.Repeat;

        //Selects the T Wrapping for following Textures to be loaded. (Default: Repeat)
        public static TextureWrapMode WrapModeT = TextureWrapMode.Repeat;

        //Simple const for mesh
        public const int textr_const = 1000;
    }

}
