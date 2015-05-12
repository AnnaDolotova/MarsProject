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

        private bool loaded = false;
        private bool mouseDown = false; // observe

        float X = 0.0f;        // Translate screen to x direction
        float Y = 0.0f;        // Translate screen to y direction
        float Z = 0.0f;        // Translate screen to z direction

        float rotX = 0.0f;    // Rotate screen on x axis 
        float rotY = 0.0f;    // Rotate screen on y axis
        float rotZ = 0.0f;    // Rotate screen on z axis

        float old_x, old_y, xdiff, ydiff;        // Used for mouse event

        BeginMode drawMode;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            MESH_HEIGHTSCALE = trackBar1.Value;

            mesh.Calculate(MESH_HEIGHTSCALE, MESH_RESOLUTION); // count data
            mesh.BuildVBOs(); // build it, we will create this func

           glControl1.Invalidate(); //
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            MESH_RESOLUTION = trackBar2.Value;	
            
            mesh.Calculate(MESH_HEIGHTSCALE, MESH_RESOLUTION);
            mesh.BuildVBOs();

            glControl1.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
           glControl1.Invalidate();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

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
            X = 0.0f;        // Translate screen to x direction
            Y = 0.0f;        // Translate screen to y direction
            Z = 0.0f;        // Translate screen to z direction

            rotX = 0.0f;    // Rotate screen on x axis 
            rotY = 0.0f;    // Rotate screen on y axis
            rotZ = 0.0f;    // Rotate screen on z axis

            glControl1.Invalidate();
        }

        float time = 0;
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) //Пока контекст не создан
                return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            GL.Rotate(30, 1, 0, 0);

            GL.Translate(X, Y, Z);
            GL.Rotate(rotX, 1.0f, 0.0f, 0.0f);            // Rotate on x
            GL.Rotate(rotY, 0.0f, 1.0f, 0.0f);            // Rotate on y
            GL.Rotate(rotZ, 0.0f, 0.0f, 1.0f);            // Rotate on z


            GL.Translate(0.0f, 0.0f, -500.0f);						// Move Above The Terrain

            if (checkBox1.Checked)
            {
                mesh.DrawBorder();
                mesh.DrawRectengle();
            }


            GL.Color3(Color.White);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBOVertices);			// Bind The Buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));		// Set The Vertex Pointer To The Vertex Buffer

            //GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBORGB);			// Bind The Buffer
            GL.ColorPointer(3, ColorPointerType.Float, 0, new IntPtr(0));		// Set The Vertex Pointer To The Vertex Buffer

            //GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.m_nVBOTexCoords);		// Bind The Buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(0));		// Set The TexCoord Pointer To The TexCoord Buffer

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, mesh.TMU0_Handle);

            GL.DrawArrays(drawMode, 0, mesh.m_nVertexCount);	// Draw All Of The Triangles At Once

            DrawCenter(mesh.maxX - (mesh.maxX - mesh.minX) / 2, mesh.maxY, mesh.maxZ - (mesh.maxZ - mesh.minZ) / 2, 1000);

            GL.LoadIdentity();

            GL.Translate(0.0f, 0.0f, -1.0f); // Передвижение на одну единицу вглубь
            GL.Color3(0, 255, 0);

            GL.Flush();

            glControl1.SwapBuffers();

            time += 1000.1F;        
        }

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

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            SetupViewport();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            drawMode = BeginMode.Triangles;

            loaded = true;
            GL.ClearColor(Color.Black);
            GL.ClearDepth(1.0f);	  									// Depth Buffer Setup
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);									// Enable Depth Testing
            GL.ShadeModel(ShadingModel.Smooth);									// Select Smooth Shading
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);			// Set Perspective Calculations To Most Accurate
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);			// Set Perspective Calculations To Most Accurate

            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);

            SetupViewport();

            mesh.Load("4.bin", "4.bmp", MESH_HEIGHTSCALE, MESH_RESOLUTION);
            mesh.BuildVBOs();
        }

        private void SetupViewport()
        {
            int w = glControl1.Width;
            int h = glControl1.Height;

            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Glu.Perspective(45.0, w / (double)h, 0.1, 90000.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        private void glControl1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            old_x = e.X * 10 - X;
            old_y = e.Y * 10 + Y;

            xdiff = e.X - rotY;
            ydiff = -e.Y + rotX;
        }

        private void glControl1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void glControl1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                if (e.Button == MouseButtons.Left)
                {
                    rotY = e.X - xdiff;
                    rotX = e.Y + ydiff;
                }

                if (e.Button == MouseButtons.Right)
                {
                    X = (e.X * 10 - old_x);
                    Y = -(e.Y * 10 - old_y);
                }

                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Z += e.Delta * 10;

            //if (Z >= 258)
            //    Z = 258;

            //if (Z <= -350)
            //    Z = -350;

            glControl1.Invalidate();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
