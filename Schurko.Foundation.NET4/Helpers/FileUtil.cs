using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Utilities
{
    public static class FileUtil
    {

        public static bool CreateFile(string path, string? content = null)
        {
            try
            {
                if (content != null)
                {
                    File.WriteAllText(path, content);
                }
                else
                {
                    File.Create(path);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        public static string ReadFile(string path, Encoding encoding = null)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    string.Format("Could not find path: [{0}]", path ?? string.Empty));
            }

            return File.ReadAllText(path, encoding ?? Encoding.UTF8);
        }

        /// <summary>
        /// The create directory.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// The delete directory.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        /// <summary>
        /// The directory copy.
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(string.Format("Source directory does not exist or could not be found: {0}", sourceDirName));
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            foreach (var file in dir.GetFiles())
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (!copySubDirs) return;

            foreach (var subdir in dir.GetDirectories())
            {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        public static void FileMove(string sourcePath, string destPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Move(sourcePath, destPath);
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
         
    }
}
