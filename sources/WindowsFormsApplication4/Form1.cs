using OpenTK; //OpenGL
using OpenTK.Graphics;
using OpenTK.Math;
using OpenTK.Input;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApplication4
{
    public partial class Form1 : Form
    {
        float MESH_RESOLUTION = 10.0f;									// Pixels
        float MESH_HEIGHTSCALE = 0.01f;									// Height Scale

        Mesh mesh = new Mesh();

        Font serif = new Font(FontFamily.GenericSerif, 16.0f);
        Font sans = new Font(FontFamily.GenericSansSerif, 10.0f);
        TextPrinter text = new TextPrinter();

        private bool loaded = false;

        private bool mouseDown = false; // observe

        private bool isDrawSphere = false;

        IntPtr esfera;

        float X = -5500.0f;        // Translate screen to x direction (left or right)
        float Y = -27000.0f;        // Translate screen to y direction (up or down)
        float Z = -50000.0f;        // Translate screen to z direction (zoom in or out)

        float rotX = 13.0f;    // Rotate screen on x axis 
        float rotY = 0.0f;    // Rotate screen on y axis
        float rotZ = 0.0f;    // Rotate screen on z axis

        float old_x, old_y, xdiff, ydiff;        // Used for mouse event

        public uint TMU2_Handle;
        public OpenTK.Graphics.OpenGL.TextureTarget TMU2_Target;
        int marsImageWidth, marsImageHeight;

        BeginMode drawMode;
        ColorMode colorMode;

        Camera cam = new Camera();
        OpenTK.Vector2 lastMousePos = new OpenTK.Vector2();

        public Form1()
        {
            InitializeComponent();
        }

        // Marina
        private void Form1_Load(object sender, EventArgs e)
        {
            esfera = Glu.NewQuadric();

            drawMode = BeginMode.Triangles;
            colorMode = ColorMode.Texture;

            loaded = true;
            GL.ClearColor(Color.Black);
            GL.ClearDepth(1.0f);	  									// Depth Buffer Setup
            GL.DepthFunc(DepthFunction.Lequal);                         // (Passes if the incoming depth value is less than or equal to the stored depth value.)
            GL.Enable(EnableCap.DepthTest);									// Enable Depth Testing
            GL.ShadeModel(ShadingModel.Smooth);									// Select Smooth Shading
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);			// Set Perspective Calculations To Most Accurate
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);			// GL_NICEST - The most correct, or highest quality, option should be chosen.
            // GL_PERSPECTIVE_CORRECTION_HINT - Indicates the quality of color, texture coordinate, and fog coordinate interpolation.
            // GL_POLYGON_SMOOTH_HINT - Indicates the sampling quality of antialiased polygons.

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.ColorMaterial);

            SetupViewport();

            mesh.Load("1.bin", "1.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
            mesh.BuildVBOs();

            ImageGDI.LoadFromDisk("mars.jpg", out TMU2_Handle, out TMU2_Target, out marsImageWidth, out marsImageHeight);

            string version = GL.GetString(StringName.Version);
        }

        // Buttons Anya
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            MESH_RESOLUTION = trackBar1.Value;									// Pixels Per Vertex
            mesh.Calculate(MESH_HEIGHTSCALE, MESH_RESOLUTION);
            mesh.BuildVBOs();

            glControl1.Invalidate();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            MESH_HEIGHTSCALE = trackBar2.Value / 1000.0f;// 0.01f;									// Mesh Height Scale
            mesh.Calculate(MESH_HEIGHTSCALE, MESH_RESOLUTION);
            mesh.BuildVBOs();

            glControl1.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           glControl1.Invalidate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            drawMode = BeginMode.Triangles;

            glControl1.Invalidate();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            drawMode = BeginMode.Lines;

            glControl1.Invalidate();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            drawMode = BeginMode.Points;

            glControl1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            X = -5500.0f;        // Translate screen to x direction (left or right)
            Y = -27000.0f;        // Translate screen to y direction (up or down)
            Z = -50000.0f;        // Translate screen to z direction (zoom in or out)

            rotX = 13.0f;    // Rotate screen on x axis 
            rotY = 0.0f;    // Rotate screen on y axis
            rotZ = 0.0f;    // Rotate screen on z axis

            cam.Position = OpenTK.Vector3.Zero;
            cam.Orientation = new OpenTK.Vector3((float)Math.PI, 0f, 0f);

            glControl1.Invalidate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isDrawSphere = !isDrawSphere;

            glControl1.Invalidate();
        }

        float time = 0;

        // Drawing Marina
        void DrawCenter(float x, float y, float z, float size)
        {
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(x - size, y, z);
            GL.Vertex3(x + size, y, z);
            GL.Color3(Color.Red);
            GL.Vertex3(x, y - size, z);
            GL.Vertex3(x, y + size, z);
            GL.Color3(Color.Blue);
            GL.Vertex3(x, y, z - size);
            GL.Vertex3(x, y, z + size);
            GL.End();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) //Пока контекст не создан
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
     
            if (isDrawSphere)
            {
                GL.LoadIdentity();

                OpenTK.Matrix4 lookat = cam.GetViewMatrix() * OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, glControl1.Width / (float)glControl1.Height, 0.1f, 90000.0f);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref lookat);

                GL.Translate(0, -10000, -20000);

                if (checkBox1.Checked)
                {
                    mesh.DrawGrid();
                    mesh.DrawRectengle();
                }

                GL.EnableClientState(EnableCap.VertexArray);						// Enable Vertex Arrays
                if (colorMode == ColorMode.Gradient)
                    GL.EnableClientState(EnableCap.ColorArray);						// Enable Vertex Arrays
                if (colorMode == ColorMode.Texture)
                    GL.EnableClientState(EnableCap.TextureCoordArray);				// Enable Texture Coord Arrays

                if (colorMode == ColorMode.Texture)
                    GL.Enable(EnableCap.Texture2D);									// Enable Textures


                GL.Color3(Color.White);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBOVertices);			// Bind The Buffer
                GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));		// Set The Vertex Pointer To The Vertex Buffer

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBORGB);			// Bind The Buffer
                GL.ColorPointer(3, ColorPointerType.Float, 0, new IntPtr(0));		// Set The Vertex Pointer To The Vertex Buffer

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBOTexCoords);		// Bind The Buffer
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(0));		// Set The TexCoord Pointer To The TexCoord Buffer

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.TMU0_Handle);

                GL.DrawArrays(drawMode, 0, mesh.m_nVertexCount);	// Draw All Of The Triangles At Once

                if (colorMode == ColorMode.Texture)
                    GL.Disable(EnableCap.Texture2D);

                // Disable Pointers
                GL.DisableClientState(EnableCap.VertexArray);					// Disable Vertex Arrays
                if (colorMode == ColorMode.Gradient)
                    GL.DisableClientState(EnableCap.ColorArray);					// Disable Vertex Arrays
                if (colorMode == ColorMode.Texture)
                    GL.DisableClientState(EnableCap.TextureCoordArray);

                DrawCenter(mesh.maxX - (mesh.maxX - mesh.minX) / 2, mesh.maxY, mesh.maxZ - (mesh.maxZ - mesh.minZ) / 2, 1000);
            }
            else
            {
                GL.LoadIdentity();
                OpenTK.Matrix4 lookat = OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, glControl1.Width / (float)glControl1.Height, 0.1f, 90000.0f);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref lookat);

                X = 0.0f; // Translate screen to x direction (left or right)
                Y = 0.0f; // Translate screen to y direction (up or down)
                Z = -10000.0f; // Translate screen to z direction (zoom in or out)

                GL.Translate(X, Y, Z);
                GL.Rotate(rotX, 1.0f, 0.0f, 0.0f); // Rotate on x
                GL.Rotate(rotY, 0.0f, 0.0f, 1.0f); // Rotate on y
                //GL.Rotate(rotZ, 0.0f, 0.0f, 1.0f); // Rotate on z
                GL.Enable(EnableCap.CullFace);
                // Draw green sphere
                GL.Color3(1.0f, 1.0f, 1.0f);

                GL.PushMatrix();

                GL.Enable(EnableCap.Texture2D);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, TMU2_Handle);
                Glu.QuadricTexture(esfera, true);
                Glu.Sphere(esfera, 4500.0f, 30, 30);
                GL.Disable(EnableCap.Texture2D);

                GL.PopMatrix();
                GL.Disable(EnableCap.CullFace);
            }
            GL.LoadIdentity();

            GL.Translate(0.0f, 0.0f, -1.0f); // Передвижение на одну единицу вглубь
            GL.Color3(0, 255, 0);
            text.Begin();
            text.Print("Полигонов: " + mesh.m_nVertexCount / 3, sans, Color.SpringGreen, new RectangleF(10, 25, Width, 0), TextPrinterOptions.NoCache, TextAlignment.Near);
            text.Print("Детализация: " + MESH_RESOLUTION, sans, Color.SpringGreen, new RectangleF(10, 45, Width, 0), TextPrinterOptions.NoCache, TextAlignment.Near);
            text.Print("Маштаб высоты: " + MESH_HEIGHTSCALE, sans, Color.SpringGreen, new RectangleF(10, 65, Width, 0), TextPrinterOptions.NoCache, TextAlignment.Near);
            text.End();

            GL.Flush();

            glControl1.SwapBuffers();

            time += 1000.1F;

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
        }

        // Projection Marina
        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;

            float aspect = w / h;
            GL.Viewport(0, 0, w, h);

            OpenTK.Matrix4 projection_matrix;
            OpenTK.Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 3, w / (float)h, 0.1f, 90000.0f, out projection_matrix);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection_matrix);
        }

        // MouseEvent Anya
        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            lastMousePos = new OpenTK.Vector2(e.X, e.Y);
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                OpenTK.Vector2 delta = lastMousePos - new OpenTK.Vector2(e.X, e.Y);

                if (e.Button == MouseButtons.Left)
                {
                    cam.AddRotation(delta.X / 10, delta.Y / 10);

                }

                if (e.Button == MouseButtons.Right)
                {
                    if (e.Y > lastMousePos.Y)
                        cam.Move(0f, 0f, 0.1f);
                    else
                        cam.Move(0f, 0f, -0.1f);
                }

                lastMousePos = new OpenTK.Vector2(e.X, e.Y);

                rotX += delta.Y;
                rotY += delta.X;
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
                cam.Move(0f, 0.1f, 0f);
            else
                cam.Move(0f, -0.1f, 0f);

            glControl1.Invalidate();
        }
       
        public enum ColorMode
        {
            Texture,
            Gradient
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                mesh = new Mesh();

                mesh.Load("1.bin", "1.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                mesh = new Mesh();

                mesh.Load("2.bin", "2.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                mesh = new Mesh();

                mesh.Load("3.bin", "3.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                mesh = new Mesh();

                mesh.Load("4.bin", "4.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                mesh = new Mesh();

                mesh.Load("5.bin", "5.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
                mesh = new Mesh();

                mesh.Load("6.bin", "6.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton10.Checked)
            {
                mesh = new Mesh();

                mesh.Load("7.bin", "7.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton11.Checked)
            {
                mesh = new Mesh();

                mesh.Load("8.bin", "8.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton12.Checked)
            {
                mesh = new Mesh();

                mesh.Load("9.bin", "9.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton13.Checked)
            {
                mesh = new Mesh();

                mesh.Load("10.bin", "10.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton14.Checked)
            {
                mesh = new Mesh();

                mesh.Load("11.bin", "11.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton15.Checked)
            {
                mesh = new Mesh();

                mesh.Load("12.bin", "12.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
                mesh.BuildVBOs();
            }
        }
    }
}
