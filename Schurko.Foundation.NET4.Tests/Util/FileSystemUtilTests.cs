using Schurko.Foundation.Helpers;
using Schurko.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Util
{
    [TestClass]
    public class FileSystemUtilTests
    {
        [TestMethod]
        public void FileSystemUtil_Tests()
        {
            FileSystemUtil.OpenFolder(Environment.CurrentDirectory);

            
        }
    }
}
