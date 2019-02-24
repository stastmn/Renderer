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
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Bitmap image = new Bitmap(800, 800);
            Color white = Color.FromArgb(255, 255, 255, 255);
            Color red = Color.FromArgb(255, 255, 0, 0);
            Color green = Color.FromArgb(255, 0, 255, 0);
            Color blue = Color.FromArgb(255, 0, 0, 255);
            int mapHidght = image.Height;
            int mapWidth = image.Width;
            int depth = 256;

            int[] zbuffer = new int[mapHidght * mapWidth];
            foreach (var a in zbuffer)
            {
                zbuffer[a] = int.MinValue;
            }

            ObjParser model = new ObjParser();
            Vec3f light_dir = new Vec3f(0, 0, -1);
            
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
            
            


            void DrawModel(ObjParser objParser)
            {
                Bitmap texture = Paloma.TargaImage.LoadTargaImage(@"Resources\african_head_diffuse.tga");

                for (int i = 0; i < objParser.Polygons.Count; i++)
                {
                    int poly1 = objParser.Polygons[i].Item1;
                    int poly2 = objParser.Polygons[i].Item2;
                    int poly3 = objParser.Polygons[i].Item3;

                    Vec3f a = new Vec3f((objParser.Vertices[poly1 - 1].X), (objParser.Vertices[poly1 - 1].Y), objParser.Vertices[poly1 - 1].Z);
                    Vec3i aa = new Vec3i((int)((objParser.Vertices[poly1 - 1].X + 1) * mapWidth / 2), (int)((objParser.Vertices[poly1 - 1].Y + 1) * mapHidght / 2), (int)((objParser.Vertices[poly1 - 1].Z + 1) * depth / 2));

                    Vec3f b = new Vec3f((objParser.Vertices[poly2 - 1].X), (objParser.Vertices[poly2 - 1].Y), objParser.Vertices[poly2 - 1].Z);
                    Vec3i bb = new Vec3i((int)((objParser.Vertices[poly2 - 1].X + 1) * mapWidth / 2), (int)((objParser.Vertices[poly2 - 1].Y + 1) * mapHidght / 2), (int)((objParser.Vertices[poly2 - 1].Z + 1) * depth / 2));

                    Vec3f c = new Vec3f((objParser.Vertices[poly3 - 1].X), (objParser.Vertices[poly3 - 1].Y), objParser.Vertices[poly3 - 1].Z);
                    Vec3i cc = new Vec3i((int)((objParser.Vertices[poly3 - 1].X + 1) * mapWidth / 2), (int)((objParser.Vertices[poly3 - 1].Y + 1) * mapHidght / 2), (int)((objParser.Vertices[poly3 - 1].Z + 1) * depth / 2));


                    Vec3f n = (c - a) ^ (b - a);
                    n = n.normalize();
                    
                    float intensity = n * light_dir;
                    if (intensity > 0)
                    {
                            Render.triangle(aa, bb, cc, ref image, Color.FromArgb((int)(255 * intensity), (int)(255 * intensity), (int)(255 * intensity)),zbuffer);
                       /* Random rnd = new Random();
                        Color rand = Color.FromArgb((int)(rnd.Next(256) * intensity),(int) (rnd.Next(256)* intensity),(int) (rnd.Next(256)*intensity));
                        Render.triangle(aa, bb, cc, ref image, rand,zbuffer);*/
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

     
    }

}
