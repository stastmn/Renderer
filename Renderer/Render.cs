using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Math;

namespace Renderer
{
    interface IShader
    {
        Vec4f vertex(int iface,int nthvert) ;
        bool fragment(ref Vec3f bar,ref Color color);

    }
        
    
    class Render
    {
        static public Matrix ModelView;
        static public Matrix ViewPort;
        static public Matrix Projection;



        ///<summary>
        ///Алгоритм Брезенхема \n
        /// Draw a line
        /// </summary>
        public static void line(Vec2i p0, Vec2i p1, ref Bitmap image, Color color)
        {

            if (p0.x < 0 || p1.x < 0 || p0.y < 0 || p1.y < 0 || p0.x > image.Width || p1.x > image.Width || p0.y > image.Height || p1.y > image.Height) throw new Exception("Coordinates Ount Of Range");

            bool steep = false;//Транспанируем если отклон большой
            if (Abs(p1.x - p0.x) < Abs(p1.y - p0.y))
            {
                Swap(ref p0.x, ref p0.y);
                Swap(ref p1.x, ref p1.y);
                steep = true;
            }

            if (p0.x > p1.x)
            {
                Swap(ref p1.x, ref p0.x);
                Swap(ref p1.y, ref p0.y);
            }



            int deltaX = p1.x - p0.x;
            int deltaY = p1.y - p0.y;

            int k = Abs(deltaY) * 2;
            int error = 0;
            int y = p0.y;
            int dir = (p0.y < p1.y) ? 1 : -1;

            for (int x = p0.x; x <= p1.x; x++)
            {
                image.SetPixelV(steep ? y : x, steep ? x : y, color);
                error += k;

                if (error > deltaX)
                {
                    y += dir;
                    error -= deltaX * 2;
                }




            }
        }
       static Vec3f cross(Vec3f v1, Vec3f v2)
        {
            return new Vec3f (v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        }
        public static Vec3f barycentric(Vec2f A, Vec2f B, Vec2f C, Vec2f P)
        {
            Vec3f[] s = new Vec3f[2];
            for(int i = 1; i>-1; i--)
            {
                s[i][0] = C[i] - A[i];
                s[i][1] = B[i] - A[i];
                s[i][2] = A[i] - P[i];
                
            }
            Vec3f u =cross( s[0] , s[1]);
            if (Abs(u[2]) > 0)
                return new Vec3f(1 - (u.x + u.y) / u.z, u.y / u.z, u.x / u.z);
            return new Vec3f(-1, 1, 1);
        }

        /// <summary>
        /// Draw a triangle
        /// </summary>
        public static void triangle(Vec3i t0, Vec3i t1, Vec3i t2, float ity0, float ity1, float ity2, ref Bitmap image, float intensity, int[] zbuffer)
        {
            
            
            if (t0.y == t1.y && t0.y == t2.y) //for degenerate triagles
                return;

            //sort the vertices
            if (t0.y > t1.y) { Swap(ref t0, ref t1); Swap(ref ity0, ref ity1); }
            if (t0.y > t2.y) { Swap(ref t0, ref t2); Swap(ref ity0, ref ity2); }
            if (t1.y > t2.y) { Swap(ref t1, ref t2); Swap(ref ity1, ref ity2); }
            
            int totalHeight = t2.y - t0.y;


            for (int i = 0; i < totalHeight; i++)
            {
                bool secondHalf = i > (t1.y - t0.y) || t1.y == t0.y;
                int segmentHeight = secondHalf ? t2.y - t1.y : t1.y - t0.y;
                float alpha = (float)i / (float)totalHeight;

                float beta = (float)(i - (secondHalf ? t1.y - t0.y : 0)) / (float)segmentHeight;
                Vec3i a = t0 + (Vec3i)(new Vec3f(t2 - t0) * alpha);
                Vec3i b = secondHalf ? t1 + (Vec3i)(new Vec3f(t2 - t1) * beta) : t0 + (Vec3i)(new Vec3f(t1 - t0) * beta);
                float ityA = ity0 + (ity2 - ity0) * alpha;
                float ityB = secondHalf ? ity1 + (ity2 - ity1) * beta : ity0 + (ity1 - ity0) * beta;


                if (a.x > b.x) {Swap(ref a, ref b); Swap(ref ityA, ref ityB); }
                for (int j = a.x; j <= b.x; j++)
                {
                   
                    float phi = b.x == a.x ? 1f : (j - a.x) / (float)(b.x - a.x);
                    Vec3i p =(Vec3i) (new Vec3f(a) + (( new Vec3f(b - a) )* phi));
                    float ityP = ityA + ((ityB - ityA) * phi);

                    int idx = p.x + p.y * image.Width;

                   // Color col =  Program.model.DiffuseMap.GetPixelV(uvP.x , uvP.y);
                    if (zbuffer[idx] < p.z)
                    {
                        zbuffer[idx] = p.z;
                        image.SetPixelV(p.x, p.y,colorMultiply(Color.White,ityP));

                    }
                    
                }
            }

        }

        public static void triangle(ref Vec4f[] pts, IShader shader, ref Bitmap image, ref Bitmap zbuffer)
        {
            Vec2f bboxmax = new Vec2f(float.MinValue, float.MinValue);
            Vec2f bboxmin = new Vec2f(float.MaxValue, float.MaxValue);
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    bboxmin[j] = Min(bboxmin[j], pts[i][j] / pts[i][3]);
                    bboxmax[j] = Max(bboxmax[j], pts[i][j] / pts[i][3]);
                    
                }
            }
            Vec2i P = new Vec2i();
            Color color = new Color();
            for(P.x = (int)(bboxmin.x);P.x<= bboxmax.x; P.x++)
            {
                for(P.y= (int)(bboxmin.y); P.y <= bboxmax.y; P.y++)
                {   //////
                    Vec3f c = barycentric( proj(pts[0] / pts[0][3]), proj(pts[1] / pts[1][3]), proj(pts[2] / pts[2][3]), proj( P) );
                    float z = pts[0][2] * c.x + pts[1][2] * c.y + pts[2][2] * c.z;
                    float w = pts[0][3] * c.x + pts[1][3] * c.y + pts[2][3] * c.z;
                    
                    int frag_depth = Max(0, Min(255, (int)((z / w) + .5)));

                    if (c.x < 0 || c.y < 0 || c.z < 0 || zbuffer.GetPixelV(P.x, P.y).R > frag_depth)
                        continue;
                    

                        bool discard = shader.fragment(ref c, ref color);
                        if (!discard)
                        {
                            zbuffer.SetPixelV(P.x, P.y, CreateColor(frag_depth));
                            image.SetPixelV(P.x, P.y, color);
                        }
                    
                }
            }

        }

        static public Vec2f proj( Vec4f v)
        {
            Vec2f ret = new Vec2f();
            for (int i = 0; i <2; i++) { ret[i] = v[i];  }
            return ret;
        }
        static public Vec3f projj(Vec4f v)
        {
            Vec3f ret = new Vec3f();
            for (int i = 0; i < 3; i++) { ret[i] = v[i]; }
            return ret;
        }
        static public Vec2f proj( Vec2i v)
        {
            Vec2f ret = new Vec2f();
            for (int i = 0; i < 2; i++) { ret[i] = v[i]; }
            return ret;
        }

        public static Matrix lookAt (Vec3f eye, Vec3f centre , Vec3f up)
        {
            Vec3f z = (eye - centre).normalize();
            Vec3f x = (up ^ z).normalize();
            Vec3f y = (z ^ x).normalize();
            
             ModelView = new Matrix().identity(4);
            
            for(int i =0; i < 3; i++)
            {
                ModelView[0][i] = x[i];
                ModelView[1][i] = y[i];
                ModelView[2][i] = z[i];
                ModelView[i][3] = -centre[i];
            }
            return  ModelView ;
        }

        public static Matrix viewport(int x, int y, int w, int h)
        {
            ViewPort = new Matrix().identity(4);
            ViewPort[0][3] = x + w / 2f;
            ViewPort[1][3] = y + h / 2f;
            ViewPort[2][3] = 255f / 2f;

            ViewPort[0][0] = w / 2;
            ViewPort[1][1] = h / 2;
            ViewPort[2][2] =  255f / 2;
            return ViewPort;
        }

        public static void projection(float coeff)
        {
            Projection = new Matrix().identity(4);
            
            Projection[3][2] = coeff;
        }

        public static Color colorMultiply(Color res, float intensity)
        {
            intensity = (intensity > 1f ? 1f : (intensity < 0f ? 0f : intensity));
            res = Color.FromArgb((int)((res.R * intensity)+0.5), (int)((res.G * intensity)+0.5), (int)((res.B * intensity)+0.5));
            return res;
        }
        public static Color CreateColor( float intensity)
        {
            intensity = (intensity > 255 ? 255 : (intensity < 0 ? 0 : intensity));
           Color res = Color.FromArgb((int)(intensity + .5), (int)(intensity + .5), (int)(intensity + .5));
            return res;
        }
        private static void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }
        private static void Swap(ref float a, ref float b)
        {
            float tmp = a;
            a = b;
            b = tmp;
        }
        private static void Swap(ref Vec2i a, ref Vec2i b)
        {
            Vec2i c = a;
            a = b;
            b = c;
        }
        private static void Swap(ref Vec2f a, ref Vec2f b)
        {
            Vec2f c = a;
            a = b;
            b = c;
        }
        private static void Swap(ref Vec3i a, ref Vec3i b)
        {
            Vec3i c = a;
            a = b;
            b = c;
        }

    }



}


