using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TileCook.FileStore
{
    public class FileStore : IWritableTileStore
    {   
        private TileInfo Info;
        public string BasePath { get; private set; }
        
        public FileStore(string resource)
        {
            BasePath = resource;
            Directory.CreateDirectory(BasePath);
            
            // Get TileInfo
            string infopath = Path.Combine(BasePath, "metadata.json");
            if (File.Exists(infopath))
            {
                string json = File.ReadAllText(infopath);
                Info = JsonConvert.DeserializeObject<TileInfo>(
                    json,
                    new JsonSerializerSettings 
                    { 
                        ContractResolver = new FileTileInfoContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new List<JsonConverter>() { new StringEnumConverter() { CamelCaseText = true } }
                    }
               );
            }
            else 
            {
                Info = new TileInfo();
            }
        }

        public Tile GetTile(int z, int x, int y)
        {
            string path = GetPath(z,x,y);
            byte[] img = null;

            try
            {
                img = File.ReadAllBytes(path);
            }
            catch (System.IO.FileNotFoundException e)
            {
                //log exception
                return null;
            }
            catch (DirectoryNotFoundException e)
            {
                //log exception
                return null;
            }
            return new Tile(img);
        }

        public void PutTile(int z, int x, int y, ITile tile)
        {
            string path = GetPath(z, x, y);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, tile.Buffer);
        }
        
        public TileInfo GetTileInfo()
        {
            return Info.DeepClone();
        }
        
        public void SetTileInfo(TileInfo tileinfo)
        {
            if (!tileinfo.IsValid())
            {
                throw new ArgumentException("TileInfo is not valid", "tileinfo");
            }
            
            Info = tileinfo.DeepClone();
            
            // write to disk
            string json = JsonConvert.SerializeObject(
                Info, 
                Formatting.Indented,
                new JsonSerializerSettings 
                { 
                    ContractResolver = new FileTileInfoContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = new List<JsonConverter>() { new StringEnumConverter() { CamelCaseText = true } }
                }
            );
            File.WriteAllText(Path.Combine(BasePath, "metadata.json"), json);
        }

        private string GetPath(int z, int x, int y)
        {
            string filename = y.ToString() + "." + Info.Format.ToString().ToLower();
            return Path.Combine(
                BasePath,
                z.ToString(),
                x.ToString(),
                filename
            );
        }
    }
}
