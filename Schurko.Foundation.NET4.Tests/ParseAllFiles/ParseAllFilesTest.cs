using Schurko.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.ParseAllFiles
{
    [TestClass]
    public class ParseAllFilesTest
    {
        [TestMethod]
        public void Parse_All_Files_Test()
        {
            string filesRootDirectory = @"C:\git\Schurko.Foundation\Schurko.Foundation";

            var allFiles = Directory.GetFiles(filesRootDirectory, "*.cs", SearchOption.AllDirectories);
            
            foreach(var file in allFiles)
            {
                FileInfo info = new FileInfo(file);
                Debug.WriteLine(info.FullName);
            }

        }
    }
}
