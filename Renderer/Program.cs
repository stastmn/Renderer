using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;



namespace Renderer
{
    static class Program
    {
       static public Model model = new Model("african_head.obj");
       static Bitmap image = new Bitmap(800, 800);
       

       static int mapHidght = image.Height;
       static int mapWidth = image.Width;
       static int depth = 256;
       static Vec3f light_dir = new Vec3f(0, 0, -4).normalize();
       static Vec3f eye = new Vec3f(0, 0, 4);
       static Vec3f centre = new Vec3f(0, 0, 0); 

       static Vec3f camera = new Vec3f(0, 0, 3);// странно, посмотреть

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            int[] zbuffer = new int[mapHidght * mapWidth];
            foreach (var a in zbuffer)
            {
                zbuffer[a] = int.MinValue;
            }

            
            
            
            DrawModel(model);

            #region returning zBuffer


            Bitmap zBufferImage = new Bitmap(800, 800);
            for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHidght; j++)
                    {
                        float intencity = ((float)zbuffer[i + j * mapHidght] / (depth*1.5f)  );
                        if (intencity > 0)
                            zBufferImage.SetPixelV(i, j, Color.FromArgb((int)(255 * intencity), (int)(255 * intencity), (int)(255 * intencity)));
                    }


                }
            #endregion




            void DrawModel(Model objParser)
            {
                

                for (int i = 0; i < objParser.Faces.Count; i++)
                {
                    Matrix ModelView = Render.lookAt(eye, centre, new Vec3f(0, 1, 1));
                    Matrix Projection = new Matrix().identity(4);
                    Matrix ViewPort = viewport(mapWidth / 8, mapHidght / 8, mapWidth * 3 / 4, mapHidght * 3 / 4);
                    Projection[3][2] = -1f / (eye - centre).norm();

                    Matrix z = (ViewPort * Projection * ModelView);
                    Vec3i poly = objParser.Faces[i];
                    Vec3i textureVertice = objParser.UVVertice[i];
                    

                    Vec3f[] worldCoords = new Vec3f[3];
                    Vec3i[] screenCoords = new Vec3i[3];
                    Vec2i[] uvCoords = new Vec2i[3];
                    for (int k = 0; k < 3; k++)
                    {
                        worldCoords[k] = new Vec3f(objParser.Verts[poly[k] - 1].x, objParser.Verts[poly[k] - 1].y, objParser.Verts[poly[k] - 1].z);
                        screenCoords[k] = (Vec3i)m2v(ViewPort * Projection * ModelView * v2m(worldCoords[k]));
                        uvCoords[k] = new Vec2i((int)(objParser.UV[textureVertice[k] - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice[k] - 1].y) * model.DiffuseMap.Height));
                    }

                  


                    Vec3f n = (worldCoords[2] - worldCoords[0]) ^ (worldCoords[1] - worldCoords[0]);
                    n = n.normalize();
                    
                    float intensity = n * light_dir;
                    if (intensity > 0)
                    {
                        Render.triangle(screenCoords[0], screenCoords[1], screenCoords[2], uvCoords[0],uvCoords[1],uvCoords[2], ref image, intensity,zbuffer);
                    }
                   

                }
            }
            

          
            //image.Save("lul.jpg");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(image));
            
            
           
        }

        public static Vec3f m2v(Matrix m)
        {
            return new Vec3f(m[0][0] / m[3][0], m[1][0] / m[3][0], m[2][0] / m[3][0]);
        }

        public static Matrix v2m(Vec3f v)
        {
            Matrix m = new Matrix(4, 1);
            m[0][0] = v.x;
            m[1][0] = v.y;
            m[2][0] = v.z;
            m[3][0] = 1;
            return m;
        }

        public static Matrix viewport(int x,int y,int w,int h)
        {
            Matrix m =  new Matrix().identity(4);
            m[0][3] = x + w/2;
            m[1][3] = y + h / 2;
            m[2][3] = depth / 2;

            m[0][0] = w / 2;
            m[1][1] = h / 2;
            m[2][2] = x + depth/ 2;
            return m;
        }

    }

    public static class BitmapExtension
    {

        ///<summary>
        ///Перегрузка метода для задания пикселя с перевернутой осью Y
        /// 
        /// </summary>
        public static void SetPixelV(this Bitmap bmp, int x, int y, Color color)
        {
            if (x < 0 ||  y < 0 || x > bmp.Width  || y > bmp.Height ) throw new Exception("Coordinates Ount Of Range");
            bmp.SetPixel( x, (bmp.Height - y-1), color);
        }

        ///<summary>
        ///Перегрузка метода для взятия пикселя с перевернутой осью Y
        /// 
        /// </summary>
        public static Color GetPixelV(this Bitmap bmp, int x, int y)
        {
            if (x < 0 || y < 0 || x > bmp.Width || y > bmp.Height) throw new Exception("Coordinates Ount Of Range");
           return bmp.GetPixel(x, (bmp.Height - y - 1));
        }

    }

}
