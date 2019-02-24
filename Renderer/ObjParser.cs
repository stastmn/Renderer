using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Renderer
{
    public struct Vertex
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public Vertex(float x, float y, float z, float w) : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    }

    class ObjParser
    {
       // string[] lines = File.ReadAllLines(@"C:\Users\stasi\Downloads\tinyrenderer-f6fecb7ad493264ecd15e230411bfb1cca539a12\tinyrenderer-f6fecb7ad493264ecd15e230411bfb1cca539a12\obj\african_head.obj");
        
        string[] lines = File.ReadAllLines(@"Resources\african_head.obj");






        private List<Vertex> vertices = new List<Vertex>();
        public List<Vertex> Vertices { get { return vertices; } }

        List<(int, int, int)> polygons = new List<(int, int, int)>();
        public List<(int, int, int)> Polygons { get { return polygons; } }

         List<(float, float)> uvCoords = new List<(float, float)>();
        public List<(float, float)> UVCoords { get { return uvCoords; } }

        List<(int, int,int)> uvVertice = new List<(int, int, int)>();
        public List<(int, int, int)> UVVertice { get { return uvVertice; } }



        private void Parse()
        {
           
            foreach(string line in lines)
            {
                if (line.ToLower().StartsWith("v "))
                {
                    var vx = line.Split(' ').Skip(1).Select(v => float.Parse(v.Replace('.', ','))).ToArray();
                    vertices.Add(new Vertex(vx[0], vx[1], vx[2],1.0f));
                }
                else if (line.ToLower().StartsWith("f "))
                {
                    var vx = line.Split(' ', '/');

                    int f1 = Int32.Parse(vx[1]);
                    int f2 = Int32.Parse(vx[4]);
                    int f3 = Int32.Parse(vx[7]);
                    
                    int vt1 = Int32.Parse(vx[2]);
                    int vt2 = Int32.Parse(vx[5]);
                    int vt3 = Int32.Parse(vx[8]);
                    
                    polygons.Add((f1, f2, f3));
                    uvVertice.Add((vt1, vt2, vt3));
                }
                
                else if(line.ToLower().StartsWith("vt "))
                {
                    var vx = line.Split(' ').Skip(2).Select(vt => float.Parse(vt.Replace('.', ','))).ToArray(); ;
                    
                    uvCoords.Add( (vx[0], vx[1]));
                }
                
            }




        }

        public ObjParser() { Parse(); }
    }

 
}
