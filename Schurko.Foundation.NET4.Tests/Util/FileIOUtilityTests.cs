
using Schurko.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Schurko.Foundation.Tests.Util
{
    [TestClass]
    public class FileIOUtilityTests
    {
        [TestMethod]
        public void FileUtil_Create_Test_File_Tests()
        {
            string fileName = "test-text.txt";
            string fileContents = "Testing Text";
            var currentDir = Environment.CurrentDirectory;
            var newDir = $"{currentDir}/TestContainer/";
            FileUtil.CreateDirectory(newDir);
            var newFile = $"{currentDir}/TestContainer/{fileName}";
            FileUtil.CreateFile(newFile, fileContents);
            var secondaryDir = $"{currentDir}/TestContainer2/";
            FileUtil.CreateDirectory(secondaryDir);
            FileUtil.DirectoryCopy(newDir, secondaryDir);

            if (File.Exists(secondaryDir + fileName))
            {
                string value = FileUtil.ReadFile(secondaryDir + fileName);
                Assert.IsTrue(fileContents.Equals(value));
            }
        }
    }
}
