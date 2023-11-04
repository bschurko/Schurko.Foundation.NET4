using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Utilities
{
    /// <summary>
    /// Utilities for Environment. Includes operating system.
    /// </summary>
    public static class EnvironmentUtility
    {
        /// <summary>
        /// Gets the bitness of the current operating system. Either 32 or 64.
        /// </summary>
        public static int GetOperatingSystemBitness()
        {
            return Environment.Is64BitOperatingSystem ? 64 : 32;
        }

        /// <summary>
        /// Gets the architecture of the current operating system. Either "x64" or "x86".
        /// </summary>
        public static string GetOperatingSystemArchitecture()
        {
            return Environment.Is64BitOperatingSystem ? "x64" : "x86";
        }

        /// <summary>
        /// Gets the bitness of the current process. Either 32 or 64.
        /// </summary>
        public static int GetProcessBitness()
        {
            return Environment.Is64BitProcess ? 64 : 32;
        }

        /// <summary>
        /// Gets the architecture of the current process. Either "x64" or "x86".
        /// </summary>
        public static string GetProcessArchitecture()
        {
            return Environment.Is64BitProcess ? "x64" : "x86";
        }

        /// <summary>
        /// Gets the root directory information of the system drive. The system drive is considered the one on which the windows directory is located.
        /// </summary>
        public static string GetSystemDrive()
        {
            string windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            return Path.GetPathRoot(windowsPath);
        }
    }
}
