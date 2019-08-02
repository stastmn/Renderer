using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;


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

    class Model
    {
        string _fileName;
        string[] lines;
        System.Drawing.Bitmap diffuseMap, normalMap, specularMap;

        public System.Drawing.Bitmap DiffuseMap { get { return diffuseMap; } }

        private List<Vec3f> verts = new List<Vec3f>();
        public List<Vec3f> Verts { get { return verts; } }

        List<Vec3i> faces = new List<Vec3i>();
        public List<Vec3i> Faces { get { return faces; } }

         List<Vec2f> uv = new List<Vec2f>();
        public List<Vec2f> UV { get { return uv; } }

         List<Vec3i> uvVertice = new List<Vec3i>();
        public  List<Vec3i> UVVertice { get { return uvVertice; } }

         List<Vec3i> vn = new List<Vec3i>();
         public List<Vec3i> VN { get { return vn; } }

        List<Vec3f> normals = new List<Vec3f>();
        public List<Vec3f> Normals { get { return normals; } }

        private void Parse()
        {
            

            foreach (string line in lines)
            {
                if (line.ToLower().StartsWith("v "))
                {
                    var vx = line.Replace("  "," ").Split(' ').Skip(1).Select(v => float.Parse(v.Replace('.', ','))).ToArray();

                    verts.Add(new Vec3f(vx[0], vx[1], vx[2]));
                }
                else if (line.ToLower().StartsWith("f "))
                {
                    var fx = line.Split(' ', '/');

                    int f1 = Int32.Parse(fx[1]);
                    int f2 = Int32.Parse(fx[4]);
                    int f3 = Int32.Parse(fx[7]);
                    
                    int vt1 = Int32.Parse(fx[2]);
                    int vt2 = Int32.Parse(fx[5]);
                    int vt3 = Int32.Parse(fx[8]);

                    //взм н ну
                    int vn1 = Int32.Parse(fx[3]);
                    int vn2 = Int32.Parse(fx[6]);
                    int vn3 = Int32.Parse(fx[9]);


                    faces.Add(new Vec3i(f1, f2, f3));
                    uvVertice.Add(new Vec3i(vt1, vt2, vt3));
                    vn.Add(new Vec3i(vn1, vn2, vn3));
                }
                
                else if(line.ToLower().StartsWith("vt "))
                {
                    var vx = line.Replace("  "," ").Split(' ').Skip(1).Select(vt => float.Parse(vt.Replace('.', ','))).ToArray(); ;
                    
                    uv.Add( new Vec2f(vx[0], vx[1]));
                }
                else if (line.ToLower().StartsWith("vn "))
                {
                    var vx = line.Replace("  "," ").Split(' ').Skip(1).Select(vt => float.Parse(vt.Replace('.', ','))).ToArray(); ;

                    normals.Add(new Vec3f(vx[0], vx[1],vx[2]));
                }
            }




        }

        public Model(string fileName)
        {
            _fileName =  fileName.Replace(".obj", "");
            lines = File.ReadAllLines(@"Resources\" + _fileName +@"\"+ fileName);
            
            loadTexture(_fileName, "_diffuse.tga",ref diffuseMap);
            loadTexture(_fileName, "_nm_tangent.tga",ref normalMap);
            loadTexture(_fileName, "_spec.tga",ref specularMap);
            Parse();
            
        }

        private void loadTexture(string fileName,string suffix, ref System.Drawing.Bitmap img)
        {
            try
            {
                img = Paloma.TargaImage.LoadTargaImage(@"Resources\"+fileName +@"\"+fileName + suffix );
            }
            catch
            {
                switch (suffix)
                {
                    case "_diffuse.tga":
                        System.Windows.Forms.MessageBox.Show("Не удалось загрузить дифузную карту");
                        break;
                    case "_nm_tangent.tga":
                        System.Windows.Forms.MessageBox.Show("Не удалось загрузить карту нормалей");
                        break;
                    case "_spec.tga":
                        System.Windows.Forms.MessageBox.Show("Не удалось загрузить карту бликов");
                        break;
                   
                }
                
            }


        }

        public Vec3f vert (int iface, int nthvert)
        {
            return verts[faces[iface][nthvert]-1];
        }
        public Vec3f normal(int iface, int nthvert)
        {

            Color c = normalMap.GetPixelV(iface, nthvert);
            Vec3f res = new Vec3f();
            float color= 0;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0) color = c.R;
                else if (i == 1) color = c.G;
                else if (i == 2) color = c.B;
                res[2 - i] = (float)color / 255f * 2f - 1f;
            }
            int idx = Faces[iface][nthvert] ;
            return res;
        }

        public System.Drawing.Color diffuse(Vec2f uvf)
        {
            Vec2i uv = new Vec2i((int)((uvf[0] * diffuseMap.Width)+0.5), (int)((uvf[1] * diffuseMap.Height)+0.5));
            return diffuseMap.GetPixelV(uv.x, uv.y);
        }

    }

 
}
