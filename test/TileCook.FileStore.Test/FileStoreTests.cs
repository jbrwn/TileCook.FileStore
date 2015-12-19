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
            Assert.Equal(path, fs.BasePath);
        }
    }
}
