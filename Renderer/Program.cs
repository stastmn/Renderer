using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;
using System.Diagnostics;


namespace Renderer
{
    
   
    
    static class Program
    {
        static public Model model;
        static Bitmap image = new Bitmap(800, 800);
        

        static int mapHeight = image.Height;
        static int mapWidth = image.Width;
        static int depth = 255;

        static Vec3f light_dir = new Vec3f(1, 1, 1);
        static Vec3f eye = new Vec3f(1, 1, 1);
        static Vec3f centre = new Vec3f(0, 0, 0);
        static Vec3f up = new Vec3f(0,1,0);

        static Matrix z ;
        struct GouraudShader : IShader
        {
            Vec3f varying_Intensity;
            Matrix varying_uv ;
            //int poly;
            public Vec4f vertex(int iface, int nthvert)
            {
                //varying_uv = new Matrix(2, 3);
                varying_uv.set_col(nthvert, model.UV[model.UVVertice[iface][nthvert] -1]);
                Vec4f gl_Vertex = model.vert(iface, nthvert);//read vertex
                gl_Vertex = m2v4(Render.ViewPort * Render.Projection * Render.ModelView * v2m(gl_Vertex));
                varying_Intensity[nthvert] = Math.Max(0f, model.normal(iface, nthvert) * light_dir);
                return gl_Vertex;
            }

            public bool fragment(ref Vec3f bar, ref Color color)
            {
                float intensity = varying_Intensity * bar;
                Vec2f uv = varying_uv * bar;
                color = Render.colorMultiply(model.diffuse(uv) , intensity);

                //color = Render.colorMultiply(Color.FromArgb(255, 255, 255), intensity);
                // if (intensity < 0.001) return true;
                /*if (poly == 100) { poly = 0;return true; }
                poly++;*/
                return false;

            }

            public GouraudShader(int u) : this()
            {
                this.varying_uv = new Matrix(2, 3);
                //poly =0;
            }
        }

        struct Shader : IShader
        {
            //Vec3f varying_Intensity;
            Matrix varying_uv;
           public Matrix uniform_M;
            public Matrix uniform_MIT;
            //int poly;
            public Vec4f vertex(int iface, int nthvert)
            {
                //varying_uv = new Matrix(2, 3);
                varying_uv.set_col(nthvert, model.UV[model.UVVertice[iface][nthvert] - 1]);
                Vec4f gl_Vertex = model.vert(iface, nthvert);//read vertex
                gl_Vertex = m2v4(Render.ViewPort * Render.Projection * Render.ModelView * v2m(gl_Vertex));
               // varying_Intensity[nthvert] = Math.Max(0f, model.normal(iface, nthvert) * light_dir);
                return gl_Vertex;
            }

            public bool fragment(ref Vec3f bar, ref Color color)
            {
                Vec2f uv = varying_uv * bar;
                Vec3f n = Render.projj(uniform_MIT * embed(model.normal((int)uv.x, (int)uv.y))).normalize();
                Vec3f l = Render.projj(uniform_M * embed(light_dir)).normalize();
                float intensity = Math.Max(0f, n * l);
                color = Render.colorMultiply(model.diffuse(uv), intensity);

                //color = Render.colorMultiply(Color.FromArgb(255, 255, 255), intensity);
                // if (intensity < 0.001) return true;
                /*if (poly == 100) { poly = 0;return true; }
                poly++;*/
                return false;

            }

            public Shader(int u) : this()
            {
                this.varying_uv = new Matrix(2, 3);
                this.uniform_M = new Matrix();
                this.uniform_MIT = new Matrix();
                //poly =0;
            }
        }

        static int[] zbuffer = new int[mapHeight * mapWidth];


        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Bitmap zBufferI = new Bitmap(800, 800);
            /*for(int i = 0; i < 800; i++)
            {
                for(int j = 0; j < 800; j++)
                {
                    image.SetPixel(i, j, Color.Black);
                }
            }*/
            String arg = null;
            if (arg == null)
            {
               model = new Model("african_head.obj");
                //model = new Model("diablo3_pose.obj");

            }

            foreach (var a in zbuffer)
            {
                zbuffer[a] = int.MinValue;
            }

            Render.lookAt(eye, centre, up);
            Render.viewport(mapWidth / 8, mapHeight / 8, mapWidth * 3 / 4, mapHeight * 3 / 4);
            Render.projection(-1f / (eye - centre).norm());
            z = (Render.ViewPort * Render.Projection * Render.ModelView);
            light_dir.normalize();
            // DrawModel(ref model);
       
                Shader shader = new Shader(1);
            shader.uniform_M = Render.Projection * Render.ModelView;
            Matrix InversTranspose = new Matrix();InversTranspose[0] = new List<float>{ 0.949f,0f,-0.316f,0f}; InversTranspose[1] = new List<float> { -0.0953f, 0.953f, -0.286f, 0f }; InversTranspose[2] = new List<float> { 0.302f, 302f, 0.905f, 0.302f }; InversTranspose[3] = new List<float> { 0f, 0f, 0f, 1f };
            shader.uniform_MIT = InversTranspose;
                int count = model.Faces.Count - 1;
                for (int i = 0; i <count ; i++)
                {
                    Vec4f[] screen_coords = new Vec4f[3];

                    for (int j = 0; j < 3; j++)
                    {

                        screen_coords[j] = shader.vertex(i, j);

                    }
                    
                    
                    Render.triangle(ref screen_coords, (IShader)shader, ref image, ref zBufferI);


                }
                
            

            var form = new Form1(image);
          
            Application.Run(form);




            //image.Save("kaleidoscope.jpg");
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            */

            /*
            for (int i = 0; i < 15; i++)
            {
                image = new Bitmap(800, 800);
                zbuffer = new int[mapHidght * mapWidth];
                eye.x += 1;
                DrawModel(ref model);
                form.Render(ref image);
            }*/







        }
        static void updateImage() {
            image = new Bitmap(800, 800);
            zbuffer = new int[mapHeight * mapWidth];
        }
        public static void rotateImageX(ref Form1 form,int x)
        {
            eye.x += x;
            updateImage();
            DrawModel(ref model);
            form.Render(ref image);
        }
        public static void rotateImageY(ref Form1 form,int y)
        {
            updateImage();
            eye.y += y;
            DrawModel(ref model);
            form.Render(ref image);
        }

        public static void rotateImageZ(ref Form1 form,int z)
        {
            updateImage();
            eye.z += z;
            DrawModel(ref model);
            form.Render(ref image);
        }


        public static Vec3f m2v(Matrix m)
        
        {
            return new Vec3f(m[0][0] / m[3][0], m[1][0] / m[3][0], m[2][0] / m[3][0]);
        }

        public static Vec4f m2v4(Matrix m)
        {
            return new Vec4f(m[0][0] , m[1][0] , m[2][0] , m[3][0] );
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
        public static Matrix v2m(Vec4f v)
        {
            Matrix m = new Matrix(4, 1);
            m[0][0] = v.x;
            m[1][0] = v.y;
            m[2][0] = v.z;
            m[3][0] = v.k;
            return m;
        }

        public static Vec4f embed(Vec3f vec)
        {
            Vec4f ret = new Vec4f();
            for(int i =0; i < 4; i++)
            {
                if (i > 2)
                    ret[i] = 1;
                else ret[i] = vec[i];

            }
            return ret;
        }
        

        public static void DrawModel(ref Model objParser)
        {

            Render.lookAt(eye, centre, new Vec3f(0, 1, 0));
            Render.viewport(mapWidth / 8, mapHeight / 8, mapWidth * 3 / 4, mapHeight * 3 / 4);
            Render.projection(-1f / (eye - centre).norm());
            light_dir.normalize();
            
            Matrix z = (Render.ViewPort * Render.Projection * Render.ModelView);

            for (int i = 0; i < objParser.Faces.Count; i++)
            {
                Vec3i poly = objParser.Faces[i];
                Vec3i textureVertice = objParser.UVVertice[i];
                Vec3i norm = objParser.VN[i];


                Vec3f[] worldCoords = new Vec3f[3];
                Vec3i[] screenCoords = new Vec3i[3];
                Vec2i[] uvCoords = new Vec2i[3];
                float[] intensityy = new float[3];

                for (int k = 0; k < 3; k++)
                {////запихнуть все в метод шейдера
                    worldCoords[k] = new Vec3f(objParser.Verts[poly[k] - 1].x, objParser.Verts[poly[k] - 1].y, objParser.Verts[poly[k] - 1].z);
                    screenCoords[k] = (Vec3i)m2v(z * v2m(worldCoords[k]));
                    uvCoords[k] = new Vec2i((int)(objParser.UV[textureVertice[k] - 1].x * model.DiffuseMap.Width), (int)((objParser.UV[textureVertice[k] - 1].y) * model.DiffuseMap.Height));
                    intensityy[k] = light_dir * objParser.Normals[norm[k] - 1];
                }




                Vec3f n = (worldCoords[2] - worldCoords[0]) ^ (worldCoords[1] - worldCoords[0]);
                n = n.normalize();

                float intensity = n * light_dir;
                if (true)
                {
                   // sw.WriteLine(screenCoords[0].x + " " + screenCoords[0].y + " " + screenCoords[0].z + " /" + screenCoords[1].x + " " + screenCoords[1].y + " " + screenCoords[1].z +" /"+ screenCoords[2].x + " " + screenCoords[2].y + " " + screenCoords[2].z + " /"  + intensityy[0] + " " + intensityy[1] + " " + intensityy[2]);
                    Render.triangle(screenCoords[0], screenCoords[1], screenCoords[2], intensityy[0], intensityy[1], intensityy[2], ref image, intensity, zbuffer);
                }

                
            }
        }

        #region returning zBuffer

        public static Bitmap returnZBuffer()
        {
            Bitmap zBufferImage = new Bitmap(800, 800);

            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    float intencity = ((float)zbuffer[i + j * mapHeight] / (depth * 1.5f));
                    if (intencity >= 0)
                        zBufferImage.SetPixelV(i, j, Color.FromArgb((int)(255 * intencity), (int)(255 * intencity), (int)(255 * intencity)));
                    else
                        zBufferImage.SetPixelV(i, j, Color.FromArgb((int)(255 /-intencity), (int)(255 / -intencity), (int)(255 / -intencity)));
                }
            }
            return zBufferImage;
        }
            #endregion
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
