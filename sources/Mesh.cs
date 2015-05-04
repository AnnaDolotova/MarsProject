//Load libraries.
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Audio;
using OpenTK.Math;
using OpenTK.Input;
using OpenTK.Platform;

//Name of our project
// uint 0...4294967295
namespace DrawHeightmapGL
{
	
	//=============================================================
	//==================Class_Mesh=================================
	//=============================================================
	//	Library Open.TK.Math helps us to use arrays easily. We can use type vector, which is needed for loading data. 
    public class Mesh
    {
        int MAP_SIZE_X = 3840;     // Size of the vertex map
        int MAP_SIZE_Z = 3840;     // Size of the vertex map
        short[] g_HeightMap;    // Array which contains coordinates of all vertexes

        public int m_nVertexCount = 0;	//number of vertexes (Vertex count)							
        public OpenTK.Math.Vector3[] m_pVertices; // Vertex Data								
        public OpenTK.Math.Vector3[] m_pRGB; // Texture Coordinates
        public OpenTK.Math.Vector2[] m_pTexCoords; // Texture Coordinates								

    	public uint	m_nTextureId; // Texture ID								
 //особенность openGL. VBO дали существенный прирост производительност над непосредственном режимом визуализации 
	    // Vertex Buffer Object Names
	    public uint	m_nVBOVertices;								// Vertex VBO Name
	    public uint	m_nVBOTexCoords;							// Texture Coordinate VBO Name
	    public uint	m_nVBORGB;							// Texture Coordinate VBO Name

		    // Temporary Data
	        //Bind a named texture to a texturing target
        public uint TMU0_Handle;
        public OpenTK.Graphics.OpenGL.TextureTarget TMU0_Target;

        //Bind a named texture to a texturing target
        public uint TMU1_Handle;
        public OpenTK.Graphics.OpenGL.TextureTarget TMU1_Target;

//Width, Heigth of our MAP
        int imageWidth, imageHeight;

        public float maxX = 0;
        public float minX = 0;
        public float maxY = 0;
        public float minY = 0;
        public float maxZ = 0;
        public float minZ = 0;

        public Mesh()
        {
	//memory
            g_HeightMap = new short[MAP_SIZE_X*MAP_SIZE_Z];
        }

        private void LoadHeightmap(string filename)
        {//Класс FileStream предоставляет реализацию абстрактного члена Stream в манере, подходящей для потоковой работы с файлами. Это элементарный поток, и он может записывать или читать только один байт или массив байтов.
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
//Класс BinaryReader служит оболочкой, в которую заключается байтовый поток, управляющий вводом двоичных данных. Ниже приведен наиболее часто употребляемый конструктор этого класса:BinaryReader(Stream input)
//где input обозначает поток, из которого вводятся считываемые данные. Для чтения из входного файла в качестве параметра input может быть указан объект, создаваемый средствами класса FileStream.
//Оператор using стоит использовать всегда, когда это возможно при работе с объектами, реализующими IDisposable (Предоставляет механизм для освобождения неуправляемых ресурсов). Это гарантирует, что при возникновении исключения в коде unmanaged ресурсы будут очищены.
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    for (int i = 0; i < MAP_SIZE_X * MAP_SIZE_Z; i++)
                    {
	//ReadInt - считываем из текущего потока целое число со знаком длиной два байта и перемещаем текущую позицию в потоке на два байта вперед. try-catch если можем, то делаем то, что под траем, если нет- кэтчим ошибку.
                        g_HeightMap[i] = br.ReadInt16();
                    }
                }
                catch (Exception ex)
                {
	//возвращает название ошибки.
                    MessageBox.Show(ex.ToString());
                }
            }
        }
//загрузка текстур
        private void LoadTexture(string filename)
        {
            
            ImageGDI.LoadFromDisk("4.bmp", out TMU0_Handle, out TMU0_Target, out imageWidth, out imageHeight);
        }
//ффункция пересчёта данных -масштабирование
		//устанавливаем разрешение
        public void Calculate(float flHeightScale, float flResolution)
        {
            // Generate Vertex Field
            m_nVertexCount = (int)(MAP_SIZE_X * MAP_SIZE_Z * 6 / (flResolution * flResolution));

            m_pVertices = new OpenTK.Math.Vector3[m_nVertexCount];						// Allocate Vertex Data
            m_pRGB = new OpenTK.Math.Vector3[m_nVertexCount];
            m_pTexCoords = new OpenTK.Math.Vector2[m_nVertexCount];				// Allocate Tex Coord Data

            int nX, nZ, nTri, nIndex = 0;									// Create Variables
            float flX, flZ;
            for (nZ = 0; nZ < MAP_SIZE_Z - 1; nZ += (int)flResolution)
            {
                for (nX = 0; nX < MAP_SIZE_X - 1; nX += (int)flResolution)
                {
                    for (nTri = 0; nTri < 6; nTri++)
                    {
                        if (nIndex < m_nVertexCount)
                        {
                            // Using This Quick Hack, Figure The X,Z Position Of The Point
                            flX = (float)nX + ((nTri == 1 || nTri == 2 || nTri == 5) ? flResolution : 0.0f);
                            flZ = (float)nZ + ((nTri == 2 || nTri == 4 || nTri == 5) ? flResolution : 0.0f);

                            m_pVertices[nIndex] = new OpenTK.Math.Vector3();
                            // Set The Data, Using PtHeight To Obtain The Y Value
                            m_pVertices[nIndex].Z = (flX - (MAP_SIZE_X / 2)) * 10;
                            m_pVertices[nIndex].Y = ((GetHeight(g_HeightMap, (int)flX, (int)flZ)) * flHeightScale) * 10;
                            m_pVertices[nIndex].X = (flZ - (MAP_SIZE_Z / 2)) * 10;

                            m_pTexCoords[nIndex] = new OpenTK.Math.Vector2();
                            // Stretch The Texture Across The Entire Mesh
                            m_pTexCoords[nIndex].X = flX / imageWidth;
                            m_pTexCoords[nIndex].Y = flZ / imageHeight;

                        }
                        nIndex++;
                    }
                }
            }
        }
//загрузка и обработка всего и вся.

        public void Load(string binariFileName, string texFileName, float flHeightScale, float flResolution)
        {
            LoadHeightmap(binariFileName);

            LoadTexture(texFileName);

            Calculate(flHeightScale, flResolution);
        }

        short GetHeight(short[] pHeightMap, int X, int Y)      // Возвращает высоту из карты вершин
        {
            int x = X % MAP_SIZE_X;          // Проверка переменной х
            int y = Y % MAP_SIZE_Z;          // Проверка переменной y

            return pHeightMap[x + (y * MAP_SIZE_X)];      // Возвращаем значение высоты
        }

        public void BuildVBOs()
        {
            // Generate And Bind The Vertex Buffer
            GL.DeleteBuffers(1, ref m_nVBOVertices);							// Get A Valid Name
            GL.GenBuffers(1, out m_nVBOVertices);							// Get A Valid Name
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_nVBOVertices);			// Bind The Buffer
            // Load The Data
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(m_nVertexCount * 3 * sizeof(float)), m_pVertices, BufferUsageHint.StaticDraw);
            
            // Generate And Bind The Color Buffer
            GL.DeleteBuffers(1, ref m_nVBORGB);							// Get A Valid Name
            GL.GenBuffers(1, out m_nVBORGB);							// Get A Valid Name
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_nVBORGB);			// Bind The Buffer
            // Load The Data
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(m_nVertexCount * 3 * sizeof(float)), m_pRGB, BufferUsageHint.StaticDraw);

            // Generate And Bind The Texture Coordinate Buffer
            GL.DeleteBuffers(1, ref m_nVBOTexCoords);							// Get A Valid Name
            GL.GenBuffers(1, out m_nVBOTexCoords);							// Get A Valid Name
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_nVBOTexCoords);		// Bind The Buffer
            // Load The Data
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(m_nVertexCount * 2 * sizeof(float)), m_pTexCoords, BufferUsageHint.StaticDraw);
        }

        public void DrawRectengle()
        {
            #region  rectangle
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.SkyBlue);

            //  задняя грань
            GL.Vertex3(minX - 1000, maxY + 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, maxY + 1000, minZ - 1000);

            GL.Vertex3(maxX + 1000, maxY + 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, minY - 1000, minZ - 1000);

            GL.Vertex3(maxX + 1000, minY - 1000, minZ - 1000);
            GL.Vertex3(minX - 1000, minY - 1000, minZ - 1000);

            GL.Vertex3(minX - 1000, minY - 1000, minZ - 1000);
            GL.Vertex3(minX - 1000, maxY + 1000, minZ - 1000);

            //  верхняя грань
            GL.Vertex3(minX - 1000, maxY + 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, maxY + 1000, minZ - 1000);

            GL.Vertex3(maxX + 1000, maxY + 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, maxY + 1000, maxZ + 1000);

            GL.Vertex3(maxX + 1000, maxY + 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, maxY + 1000, maxZ + 1000);

            GL.Vertex3(minX - 1000, maxY + 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, maxY + 1000, minZ - 1000);

            //  передняя грань
            GL.Vertex3(minX - 1000, maxY + 1000, maxZ + 1000);
            GL.Vertex3(maxX + 1000, maxY + 1000, maxZ + 1000);

            GL.Vertex3(maxX + 1000, maxY + 1000, maxZ + 1000);
            GL.Vertex3(maxX + 1000, minY - 1000, maxZ + 1000);

            GL.Vertex3(maxX + 1000, minY - 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, minY - 1000, maxZ + 1000);

            GL.Vertex3(minX - 1000, minY - 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, maxY + 1000, maxZ + 1000);
          
            //  Нижняя грань
            GL.Vertex3(minX - 1000, minY - 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, minY - 1000, minZ - 1000);

            GL.Vertex3(maxX + 1000, minY - 1000, minZ - 1000);
            GL.Vertex3(maxX + 1000, minY - 1000, maxZ + 1000);

            GL.Vertex3(maxX + 1000, minY - 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, minY - 1000, maxZ + 1000);

            GL.Vertex3(minX - 1000, minY - 1000, maxZ + 1000);
            GL.Vertex3(minX - 1000, minY - 1000, minZ - 1000);

            GL.End();
            #endregion
        }

        public void DrawGrid()
        {
            #region  grid
            GL.Begin(BeginMode.Lines);
            GL.Color3(Color.LightBlue);

            for (int i = (int)minX-5000; i < maxX+5000; i+=1000 )
            {
                GL.Vertex3(i, minY - 4000, minZ-5000);
                GL.Vertex3(i, minY - 4000, maxZ+5000);
            }

            for (int j = (int)minZ-5000; j < maxZ+5000; j += 1000)
            {
                GL.Vertex3(minX-5000, minY - 4000, j);
                GL.Vertex3(maxX+5000, minY - 4000, j);
            }

            GL.End();
            #endregion
        }

        public void DrawBorder()
        {
            #region  rectangle
            GL.Begin(BeginMode.Quads);

            //  задняя грань
            GL.Color3(Color.FromArgb(23, 32, 89));
            GL.Vertex3(minX-6000, maxY+6000, minZ-6000);
            GL.Color3(Color.FromArgb(186, 197, 253));
            GL.Vertex3(maxX+6000, maxY+6000, minZ-6000);
            GL.Color3(Color.FromArgb(186, 197, 253));
            GL.Vertex3(maxX+6000, minY-6000, minZ-6000);
            GL.Color3(Color.FromArgb(23, 32, 89));
            GL.Vertex3(minX-6000, minY-6000, minZ-6000);

           

            //  Нижняя грань
            GL.Color3(Color.FromArgb(99, 109, 206));
            GL.Vertex3(minX-6000, minY-6000, minZ-6000);
            GL.Color3(Color.FromArgb(250, 250, 250));
            GL.Vertex3(maxX+6000, minY-6000, minZ-6000);
            GL.Color3(Color.FromArgb(250, 250, 250));
            GL.Vertex3(maxX+6000, minY-6000, maxZ);
            GL.Color3(Color.FromArgb(99, 109, 206));
            GL.Vertex3(minX-6000, minY-6000, maxZ);


            GL.End();
            #endregion
        }

    }
}
