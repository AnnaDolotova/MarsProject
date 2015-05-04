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

        public int m_nVertexCount = 0;	 //number of vertexes (Vertex count)						
        public OpenTK.Math.Vector3[] m_pVertices; //Vertex Data								
        public OpenTK.Math.Vector3[] m_pRGB; // Texture Coordinates
        public OpenTK.Math.Vector2[] m_pTexCoords; // Texture Coordinates							

    	public uint	m_nTextureId;				// Texture ID
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
        {	//memory
            g_HeightMap = new short[MAP_SIZE_X*MAP_SIZE_Z];
        }

        private void LoadHeightmap(string filename)
        {	//Класс FileStream предоставляет реализацию абстрактного члена Stream в манере, подходящей для потоковой работы с файлами. Это элементарный поток, и он может записывать или читать только один байт или массив байтов.
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
		//здесь будет функция загрузки текстур.
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

            int nX, nZ = 0;									// Create Variables
            float flX, flZ;
            for (nZ = 0; nZ < MAP_SIZE_Z - 1; nZ += (int)flResolution)
            {
                for (nX = 0; nX < MAP_SIZE_X - 1; nX += (int)flResolution)
                {
          //перерасчёт масштаба
                    
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
		//ищем высоту из карты вершин
        short GetHeight(short[] pHeightMap, int X, int Y)      // Возвращает высоту из карты вершин
        {
            int x = X % MAP_SIZE_X;          
            int y = Y % MAP_SIZE_Z;          

            return pHeightMap[x + (y * MAP_SIZE_X)];      // Возвращаем значение высоты
        }
			
		//Подставка для карты
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
			//числа приблизительные по высмотренным значениям из файла
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
