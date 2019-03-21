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
       static Vec3f light_dir = new Vec3f(0, 0, -1);
       static Vec3f camera = new Vec3f(0, 0, 3);

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
                    
                    Matrix Projection = new Matrix().identity(4);
                    Matrix ViewPort = viewport(mapWidth / 8, mapHidght / 8, mapWidth * 3 / 4, mapHidght * 3 / 4);
                    Projection[3][2] = -1f / camera.z;


                    Vec3i poly = objParser.Faces[i];
                    
                    // поменять вектора аа бб и тд на л1 л2 
                    // и запихнуть это в массивы 

                        Vec3f a = new Vec3f((objParser.Verts[poly.x - 1].x), (objParser.Verts[poly.x - 1].y), objParser.Verts[poly.x - 1].z);
                        Vec3i aa = new Vec3i((int)((objParser.Verts[poly.x - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.x - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.x - 1].z + 1) * depth / 2));

                        Vec3f b = new Vec3f((objParser.Verts[poly.y - 1].x), (objParser.Verts[poly.y - 1].y), objParser.Verts[poly.y - 1].z);
                        Vec3i bb = new Vec3i((int)((objParser.Verts[poly.y - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.y - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.y - 1].z + 1) * depth / 2));

                        Vec3f c = new Vec3f((objParser.Verts[poly.z - 1].x), (objParser.Verts[poly.z - 1].y), objParser.Verts[poly.z - 1].z);
                        Vec3i cc = new Vec3i((int)((objParser.Verts[poly.z - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.z - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.z - 1].z + 1) * depth / 2));

                    Vec3f l = m2v(ViewPort * Projection * v2m(poly));
                    Vec3i l2 =(Vec3i) m2v(ViewPort * Projection * v2m(a));
                    Vec3i l3 = (Vec3i)m2v(ViewPort * Projection * v2m(b));
                    Vec3i l4 =(Vec3i) m2v(ViewPort * Projection * v2m(c));

                    Vec3i textureVertice = objParser.UVVertice[i];

                        Vec2i uv0 = new Vec2i((int)(objParser.UV[textureVertice.x - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice.x - 1].y) * model.DiffuseMap.Height));
                        Vec2i uv1 = new Vec2i((int)(objParser.UV[textureVertice.y - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice.y - 1].y) * model.DiffuseMap.Height));
                        Vec2i uv2 = new Vec2i((int)(objParser.UV[textureVertice.z - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice.z - 1].y) * model.DiffuseMap.Height));
                    


                    Vec3f n = (c - a) ^ (b - a);
                    n = n.normalize();
                    
                    float intensity = n * light_dir;
                    if (intensity > 0)
                    {
                        
                        Render.triangle(l2, l3, l4,uv0,uv1,uv2, ref image, intensity,zbuffer);
                    }
                   

                }
            }
            

          
           // image.Save("perspectiveRender.jpg");
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
