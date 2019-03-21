using static System.Math;
using System.Collections.Generic;
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
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new System.Exception("Index is out of range ");
                }

            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new System.Exception("Index is out of range ");
                }
            }
        }
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
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    default:
                        throw new System.Exception("Index is out of range ");
                }

            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new System.Exception("Index is out of range ");
                }
            }
        }
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
        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new System.Exception("Index is out of range ");
                }

            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new System.Exception("Index is out of range ");
                }
            }
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
        public float norm() { return (float)Sqrt(x * x + y * y + z * z); }
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
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new System.Exception("Index is out of range ");
                }
                    
            }
            set
            {
                switch (index)
                {
                    case 0:
                         x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new System.Exception("Index is out of range ");
                }
            }
        }
    }


    
    class Matrix
    {
        const int DEFAULT_ALLOC = 4;
        public int Rows { get; }
        public int Cols { get; }
        List<List<float>> m;

        public Matrix(int r, int c)
        {

            Rows = r;
            Cols = c;
            m = new List<List<float>>(r);
            for (int i = 0; i < r; i++)
            {
                m.Add( new List<float>(c) );
                for (int j = 0; j < c; j++)
                {
                    m[i].Add(0);
                }
            }
        }

        public Matrix() : this(DEFAULT_ALLOC, DEFAULT_ALLOC) { }

        public Matrix identity(int dimensions)
        {
            Matrix E = new Matrix(dimensions, dimensions);
            for (int i = 0; i < dimensions; i++)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    E[i][j] = (i == j ? 1f : 0f);
                }
            }
            return E;
        }

        public List<float> this[int index]
        {
            get
            {
                return m[index];
            }
            set
            {
                m[index] = value;
            }
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows) throw new System.Exception("Non - multiplilable martices");

            Matrix result = new Matrix(a.Rows, b.Cols);
            for(int i = 0; i < a.Rows; i++)
            {
                for(int j = 0; j < b.Cols; j++)
                {
                    result[i][j] = 0;
                    for(int k = 0; k < a.Cols; k++)
                    {
                        result.m[i][j] += a.m[i][k] * b.m[k][j];
                    }
                }
            }
            return result;
        }

        public Matrix transpose()
        {
            Matrix result = new Matrix(Rows, Cols);
            for(int i =0; i < Rows; i++)
                for(int j = 0; j < Cols; j++)
                    result[j][i] = this[i][j];

            return result;
        }

        public Matrix invers()
        {
            if (Cols != Rows) throw new System.Exception("Non - inversible martix");
            Matrix result = new Matrix(Rows, Cols * 2);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    result[i][j] = m[i][j];
            for (int i = 0; i < Rows; i++)
                result[i][i + Cols] = 1;
            for(int i = 0; i < Rows - 1; i++)
            {
                for (int j = result.Cols - 1; j>=0; j--)
                    result[i][j] /= result[i][i];

                for (int k = i + 1; k < Rows; k++)
                {
                    float coef = result[k][i];
                    for(int j = 0; j < result.Cols; j++)
                    {
                        result[k][j] -= result[i][j] * coef;
                    }
                }
            }

            // normalize the last row
            for (int j = result.Cols - 1; j >= Rows - 1; j--)
                result[Rows - 1][j] /= result[Rows - 1][Rows - 1];
            // second pass
            for (int i = Rows - 1; i > 0; i--)
            {
                for (int k = i - 1; k >= 0; k--)
                {
                    float coeff = result[k][i];
                    for (int j = 0; j < result.Cols; j++)
                    {
                        result[k][j] -= result[i][j] * coeff;
                    }
                }
            }

            Matrix truncate = new Matrix(Rows, Cols);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    truncate[i][j] = result[i][j + Cols];
            return truncate;
        }

        
    }

    

            
}
