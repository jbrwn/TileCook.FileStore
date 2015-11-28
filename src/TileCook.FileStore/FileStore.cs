using System;
using System.IO;
using System.Collections.Generic;
using TileProj;

namespace TileCook.FileStore
{
    public class FileStore : IWritableTileStore
    {
        public string BasePath { get; private set; }
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int MinZoom { get; private set; }
        public int MaxZoom { get; private set; }
        public IEnvelope Bounds { get; private set; }
        public IEnumerable<VectorLayer> VectorLayers { get; private set; }
        
        public FileStore(string resource, string id = null, string name = null, string description = null, int? minzoom = null, int? maxzoom = null, IEnvelope bounds = null, IEnumerable<VectorLayer> vlayers = null)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("Base path resource");
            }
            if (!Directory.Exists(resource))
            {
                throw new ArgumentException("Invalid base path resource");
            }
            BasePath = resource;
            Id = id ?? new DirectoryInfo(resource).Name;
            Name = name ?? "";
            Description = description ?? "";
            MinZoom = minzoom == null ? 0 : minzoom.Value;
            MaxZoom = maxzoom == null ? 14 : maxzoom.Value;
            Bounds = bounds ?? new Envelope(-180, -90, 180, 90);
            VectorLayers = vlayers;
        }

        public VectorTile GetTile(ICoord coord)
        {
            string path = GetPath(coord);
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
