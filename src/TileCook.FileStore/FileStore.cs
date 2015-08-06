using System;
using System.IO;
using System.Collections.Generic;
using TileProj;
using Newtonsoft.Json.Linq;

namespace TileCook.FileStore
{
    public class FileStore : IWritableTileStore
    {

        public FileStore(string resource, string id = null)
        {
            BasePath = resource;
            Id = id ?? new DirectoryInfo(resource).Name;

            //load metadata.json
            string metadata = Path.Combine(resource, "metadata.json");
            JObject json = new JObject();
            if (File.Exists(metadata))
            {
                json = JObject.Parse(File.ReadAllText(metadata));
            }

            Name = json.Value<string>("name") ?? "";
            Description = json.Value<string>("description") ?? "";
            MinZoom = json["minzoom"] == null ? 0 : Convert.ToInt32(json["minzoom"]);
            MaxZoom = json["maxzoom"] == null ? 14 : Convert.ToInt32(json["maxzoom"]);
            if (json["bounds"] != null)
            {
                string[] bounds = json.Value<string>("bounds").Split(',');
                Bounds = new Envelope(
                    Convert.ToDouble(bounds[0]),
                    Convert.ToDouble(bounds[1]),
                    Convert.ToDouble(bounds[2]),
                    Convert.ToDouble(bounds[3])
                );
            }
            else
            {
                Bounds = new Envelope(-180, -90, 180, 90);
            }

            if (json["json"] != null)
            {
                VectorLayers = json["json"].Value<IEnumerable<VectorLayer>>("vector_layers");
            }
        }

        public string BasePath { get; private set; }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public IEnvelope Bounds { get; private set; }
        public IEnumerable<VectorLayer> VectorLayers { get; private set; }

        public VectorTile GetTile(ICoord coord)
        {
            string path = GetPath(coord);
            byte[] img = null;

            try
            {
                img = File.ReadAllBytes(path);
            }
            catch (FileNotFoundException e)
            {
                //log exception
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                //log exception
                return null;
            }
            return new VectorTile(img);
        }

        public void PutTile(ICoord coord, VectorTile tile)
        {
            string path = GetPath(coord);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, tile.Buffer);
        }

        private string GetPath(ICoord coord)
        {
            string filename = coord.Y.ToString() + ".pbf";
            string path = Path.Combine(
                BasePath,
                coord.Z.ToString(),
                coord.X.ToString(),
                filename
            );
            return path;
        }
    }
}
