using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TileCook.FileStore;

namespace TileCook.FileStore.Test
{
    public class FileStoreTests
    {
        [Fact]
        public void DefaultParams()
        {
            string path = Directory.GetCurrentDirectory();
            string id = new DirectoryInfo(path).Name;
            FileStore fs = new FileStore(path);
            ITileInfo info = fs.GetTileInfo();
            Assert.Equal(path, fs.BasePath);
            Assert.Equal("", info.Name);
            Assert.Equal("", info.Description);
            Assert.Equal(0, info.MinZoom);
            Assert.Equal(14, info.MaxZoom);
            Assert.Equal(new double[] {-180, -90, 180, 90}, info.Bounds);
            Assert.Null(info.Center);
            Assert.Null(info.VectorLayers);

        }
    }
}
