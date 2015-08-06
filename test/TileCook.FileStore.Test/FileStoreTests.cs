using System;
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
            FileStore fs = new FileStore(@"c:\test");
            Assert.Equal(@"c:\test", fs.BasePath);
            Assert.Equal(0, fs.MinZoom);
            Assert.Equal(14, fs.MaxZoom);
            Assert.Equal("test", fs.Id);
            Assert.Equal("", fs.Name);
            Assert.Equal("", fs.Description);
        }
    }
}
