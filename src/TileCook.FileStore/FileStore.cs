using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                    new JsonSerializerSettings { ContractResolver = new FileTileInfoContractResolver() }
               );
            }
            else 
            {
                Info = new TileInfo();
            }
        }

        public ITile GetTile(int z, int x, int y)
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
        
        public ITileInfo GetTileInfo()
        {
            //return TileInfo.Clone()
            return new TileInfo()
            {
                Name = Info.Name,
                Description = Info.Description,
                MinZoom = Info.MinZoom,
                MaxZoom = Info.MaxZoom,
                Bounds = (double[])Info.Bounds.Clone(),
                Center = Info.Center != null ? (double[])Info.Center.Clone() : null,
                VectorLayers = Info.VectorLayers // To do: deep clone
            };
        }
        
        public void SetTileInfo(ITileInfo tileinfo)
        {
            // Set info
            tileinfo.Validate();
            Info.Name = tileinfo.Name;
            Info.Description = tileinfo.Description;
            Info.MinZoom = tileinfo.MinZoom;
            Info.MaxZoom = tileinfo.MaxZoom;
            Info.Bounds = (double[])tileinfo.Bounds.Clone();
            Info.Center = tileinfo.Center != null ? (double[])Info.Center.Clone() : null;
            Info.VectorLayers = tileinfo.VectorLayers; // To do: deep clone
            
            // write to disk
            string json = JsonConvert.SerializeObject(
                Info, 
                Formatting.Indented,
                new JsonSerializerSettings { ContractResolver = new FileTileInfoContractResolver() }
            );
            File.WriteAllText(Path.Combine(BasePath, "metadata.json"), json);
        }

        private string GetPath(int z, int x, int y)
        {
            string filename = y.ToString() + ".pbf";
            return Path.Combine(
                BasePath,
                z.ToString(),
                x.ToString(),
                filename
            );
        }
    }
}
