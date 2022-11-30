using Newtonsoft.Json.Linq;

namespace RocketJumper.Classes.MapData
{
    struct MapObject
    {
        public string Class;
        public int Height;
        public int Width;
        public int Gid;
        public int Id;
        public string Name;
        public int Rotation;
        public bool Visible;
        public double X;
        public double Y;

        public MapObject(JObject objectJson)
        {
            Class = objectJson["class"].ToString();
            Height = (int)objectJson["height"];
            Width = (int)objectJson["width"];
            Gid = (int)objectJson["gid"];
            Id = (int)objectJson["id"];
            Name = objectJson["name"].ToString();
            Rotation = (int)objectJson["rotation"];
            Visible = (bool)objectJson["visible"];
            X = (double)objectJson["x"];
            Y = (double)objectJson["y"];
            
        }
    }
}