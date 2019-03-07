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
            
           
                Bitmap zBuff = new Bitmap(800, 800);

                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHidght; j++)
                    {
                        float intencity = ((float)zbuffer[i + j * mapHidght] / 250);
                        if (intencity > 0)
                            zBuff.SetPixelV(i, j, Color.FromArgb((int)(255 * intencity), (int)(255 * intencity), (int)(255 * intencity)));
                    }


                }



            
            void DrawModel(Model objParser)
            {
                

                for (int i = 0; i < objParser.Faces.Count; i++)
                {

                    Vec3i poly = objParser.Faces[i];


                    Vec3f a = new Vec3f((objParser.Verts[poly.x - 1].x), (objParser.Verts[poly.x - 1].y), objParser.Verts[poly.x - 1].z);
                    Vec3i aa = new Vec3i((int)((objParser.Verts[poly.x - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.x - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.x - 1].z + 1) * depth / 2));

                    Vec3f b = new Vec3f((objParser.Verts[poly.y - 1].x), (objParser.Verts[poly.y - 1].y), objParser.Verts[poly.y - 1].z);
                    Vec3i bb = new Vec3i((int)((objParser.Verts[poly.y - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.y - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.y - 1].z + 1) * depth / 2));

                    Vec3f c = new Vec3f((objParser.Verts[poly.z - 1].x), (objParser.Verts[poly.z - 1].y), objParser.Verts[poly.z - 1].z);
                    Vec3i cc = new Vec3i((int)((objParser.Verts[poly.z - 1].x + 1) * mapWidth / 2), (int)((objParser.Verts[poly.z - 1].y + 1) * mapHidght / 2), (int)((objParser.Verts[poly.z - 1].z + 1) * depth / 2));


                    Vec3i textureVertice = objParser.UVVertice[i];
                    
                    Vec2i uv0 = new Vec2i((int)(objParser.UV[textureVertice.x - 1].x * model.DiffuseMap.Width ),   (int)((objParser.UV[textureVertice.x - 1].y)* model.DiffuseMap.Height));
                    Vec2i uv1 = new Vec2i((int)(objParser.UV[textureVertice.y - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice.y - 1].y)* model.DiffuseMap.Height));
                    Vec2i uv2 = new Vec2i((int)(objParser.UV[textureVertice.z - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice.z - 1].y)* model.DiffuseMap.Height));
                    


                    Vec3f n = (c - a) ^ (b - a);
                    n = n.normalize();
                    
                    float intensity = n * light_dir;
                    if (intensity > 0)
                    {                 
                        Render.triangle(aa, bb, cc,uv0,uv1,uv2, ref image, intensity,zbuffer);
                    }
                   

                }
            }
            

          
           // image.Save("collorRender.jpg");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(image));
            
            
           
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
