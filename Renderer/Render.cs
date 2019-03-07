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

    class Render
    {

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


        /// <summary>
        /// Draw a triangle
        /// </summary>
        public static void triangle(Vec3i t0, Vec3i t1, Vec3i t2, Vec2i uv0, Vec2i uv1, Vec2i uv2, ref Bitmap image, float intensity, int[] zbuffer)
        {
            
            
            if (t0.y == t1.y && t0.y == t2.y) //for degenerate triagles
                return;

            //sort the vertices
            if (t0.y > t1.y) { Swap(ref t0, ref t1); Swap(ref uv0, ref uv1); }
            if (t0.y > t2.y) { Swap(ref t0, ref t2); Swap(ref uv0, ref uv2); }
            if (t1.y > t2.y) { Swap(ref t1, ref t2); Swap(ref uv1, ref uv2); }
            
            int totalHeight = t2.y - t0.y;


            for (int i = 0; i < totalHeight; i++)
            {
                bool secondHalf = i > (t1.y - t0.y) || t1.y == t0.y;
                int segmentHeight = secondHalf ? t2.y - t1.y : t1.y - t0.y;
                float alpha = (float)i / (float)totalHeight;

                float beta = (float)(i - (secondHalf ? t1.y - t0.y : 0)) / (float)segmentHeight;
                Vec3i a = t0 + (Vec3i)(new Vec3f(t2 - t0) * alpha);
                Vec3i b = secondHalf ? t1 + (Vec3i)(new Vec3f(t2 - t1) * beta) : t0 + (Vec3i)(new Vec3f(t1 - t0) * beta);
                Vec2i uvA = uv0 + (uv2 - uv0) * alpha;
                Vec2i uvB = secondHalf ? uv1 + (uv2 - uv1) * beta : uv0 + (uv1 - uv0) * beta;


                if (a.x > b.x) {Swap(ref a, ref b); Swap(ref uvA, ref uvB); }
                for (int j = a.x; j <= b.x; j++)
                {
                   
                    float phi = b.x == a.x ? 1f : (j - a.x) / (float)(b.x - a.x);
                    Vec3i p =(Vec3i) (new Vec3f(a) + (( new Vec3f(b - a) )* phi));
                    Vec2i uvP = uvA + ((uvB - uvA) * phi);

                    int idx = p.x + p.y * image.Width;

                    Color col =  Program.model.DiffuseMap.GetPixelV(uvP.x , uvP.y);
                    if (zbuffer[idx] < p.z)
                    {
                        zbuffer[idx] = p.z;
                        image.SetPixelV(p.x, p.y, Color.FromArgb((int)(col.R*intensity),(int)(col.G*intensity),(int)(col.B*intensity)));

                    }
                    
                }
            }

        }


     
        private static void Swap(ref int a, ref int b)
        {
            int tmp = a;
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


