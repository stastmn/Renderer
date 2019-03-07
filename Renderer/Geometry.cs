using static System.Math;
namespace Renderer
{
    
    struct Vec2i 
    {
        
       public int u, v;
       
      public  int x, y;
        int[] raw;

      // исправленная версия на XY, потом переделать!
        public Vec2i(int _x, int _y)
        {
            u = 0;
            v = 0;
            x =_x;
            y = _y;
            raw = new int[2];
        }

        public static Vec2i operator +(Vec2i A, Vec2i B) { return new Vec2i(A.x + B.x, A.y + B.y); }
        public static Vec2i operator -(Vec2i A, Vec2i B) { return new Vec2i(A.x - B.x, A.y - B.y); }
        public static Vec2i operator *(Vec2i A, Vec2i B) { return new Vec2i(A.x * B.x, A.y * B.y); }
        public static Vec2i operator *(Vec2i A, float B) { return new Vec2i((int)((A.x * B)+0.5), (int)((A.y * B)+0.5)); }
    }

    struct Vec2f
    {

       public float u, v;
       public float x, y;
        float[] raw;

      

        public Vec2f(float _x, float _y)
        {
            u = 0;
            v = 0;
            x = _x;
            y = _y;
            raw = new float[2];
        }

        public static Vec2f operator +(Vec2f A, Vec2f B) { return new Vec2f(A.x + B.x, A.y + B.y); }
        public static Vec2f operator -(Vec2f A, Vec2f B) { return new Vec2f(A.x - B.x, A.y - B.y); }
        public static Vec2f operator *(Vec2f A, Vec2f B) { return new Vec2f(A.x * B.x, A.y * B.y); }
    }

    struct Vec3i
    {

        public int ivert, iuv,inorm;
        public int x, y,z;
        public int[] raw;



        public Vec3i(Vec3i _vec)
        {
            iuv = _vec.iuv;
            ivert = _vec.ivert;
            inorm = _vec.inorm;
            x = _vec.x;
            y = _vec.y;
            z = _vec.z;
            raw = _vec.raw;
        }
        public Vec3i(int _x, int _y, int _z)
        {
            iuv = 0;
            ivert = 0;
            inorm = 0;
            x = _x;
            y = _y;
            z = _z;
            raw = new int[3];
        }

        public static Vec3i operator +(Vec3i A, Vec3i B) { return new Vec3i(A.x + B.x, A.y + B.y, A.z + B.z); }
        public static Vec3i operator -(Vec3i A, Vec3i B) { return new Vec3i(A.x - B.x, A.y - B.y, A.z - B.z); }
        //public static Vec3i operator *(Vec3i A, Vec3i B) { return new Vec3i(A.x * B.y, A.x * B.y, A.z * B.z); }
        public static Vec3i operator ^(Vec3i A, Vec3i B) { return new Vec3i(A.y*B.z-A.z*B.y, A.z*B.x - A.x*B.z, A.x* B.y - A.y*B.x); }
        public static Vec3i operator *(Vec3i A, float B) { return new Vec3i((int)(A.x * B), (int)(A.y * B),(int)(A.z * B)); }
        public static float operator *(Vec3i A, Vec3i B) { return A.x * B.x + A.y * B.y + A.z * B.z; }
        float norm() { return (float)Sqrt(x * x + y * y + z * z); }
        Vec3i normalize() { this = (this) * (1 / norm());return this; }
        public static explicit operator Vec3i(Vec3f operand)
        {
            Vec3i a = new Vec3i(); // + 0.5 for rounding
            a.x = (int)(operand.x+0.5);
            a.y = (int)(operand.y+0.5);
            a.z = (int)(operand.z+0.5);
            a.iuv = (int)operand.iuv;
            a.ivert = (int)operand.ivert;
            a.inorm = (int)operand.inorm;
            a.raw = new int[3];
            //for(int i = 0; i < 3; i++) { a.raw[i] = (int)operand.raw[i]; }
            return a;
        }
    }

    struct Vec3f
    {

       public float ivert, iuv, inorm;
       public float x, y, z;
       public float[] raw;

        public Vec3f(Vec3f _vec)
        {
            iuv = _vec.iuv;
            ivert = _vec.ivert;
            inorm = _vec.inorm;
            x = _vec.x;
            y = _vec.y;
            z = _vec.z;
            raw = _vec.raw;
        }
        public Vec3f(float _x, float _y, float _z)
        {
            iuv = 0;
            ivert =0;
            inorm = 0;
            x = _x;
            y = _y;
            z = _z;
            raw = new float[3];
        }

        public static Vec3f operator +(Vec3f A, Vec3f B) { return new Vec3f(A.x + B.x, A.y + B.y, A.z + B.z); }
        public static Vec3f operator -(Vec3f A, Vec3f B) { return new Vec3f(A.x - B.x, A.y - B.y, A.z - B.z); }
        //public static Vec3f operator *(Vec3f A, Vec3f B) { return new Vec3f(A.x * B.y, A.x * B.y, A.z * B.z); }
        public static Vec3f operator ^(Vec3f A, Vec3f B) { return new Vec3f(A.y * B.z -A.z * B.y, A.z * B.x - A.x * B.z, A.x * B.y - A.y * B.x); }
        public static Vec3f operator *(Vec3f A, float B) { return new Vec3f(A.x * B, A.y * B, A.z * B); }
        public static float operator *(Vec3f A, Vec3f B) { return A.x * B.x + A.y * B.y + A.z * B.z; }
        float norm() { return (float)Sqrt(x * x + y * y + z * z); }
       public Vec3f normalize() { this =( (this) * (1 / norm())); return this; }
        public static implicit operator Vec3f(Vec3i operand)
        {
            Vec3f a = new Vec3f();
            a.x = (float)operand.x;
            a.y = (float)operand.y;
            a.z = (float)operand.z;
            a.iuv = (float)operand.iuv;
            a.ivert = (float)operand.ivert;
            a.inorm = (float)operand.inorm;
            a.raw = new float[3];
            //for (int i = 0; i < 3; i++) { a.raw[i] = (float)operand.raw[i]; }
            return a;
        }
    }



}
