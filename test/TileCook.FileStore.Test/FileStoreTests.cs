using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TileCook.FileStore;
using TileProj;

namespace TileCook.FileStore.Test
{
    public class FileStoreTests
    {
        [Fact]
        public void DefaultParams()
        {
            string path = Directory.GetCurrentDirectory();
            string id = new DirectoryInfo(path).Name;
            FileStore fs = new FileStore(
                path,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            Assert.Equal(path, fs.BasePath);
            Assert.Equal(id, fs.Id);
            Assert.Equal("", fs.Name);
            Assert.Equal("", fs.Description);
            Assert.Equal(0, fs.MinZoom);
            Assert.Equal(14, fs.MaxZoom);
            Assert.Equal(new Envelope(-180, -90, 180, 90), fs.Bounds);
            Assert.Null(fs.VectorLayers);

        }
        
        [Fact]
        public void InvalidResource_Throws()
        {
            Assert.Throws<ArgumentException>(() => new FileStore("PathDoesNotExist"));
        }
    }
}
