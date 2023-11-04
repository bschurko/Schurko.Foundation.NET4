using System.IO;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class DirectoryExtensions
    {
        public static bool IsEmpty(this DirectoryInfo dInfo) => dInfo.GetDirectories().Length == 0 && dInfo.GetFiles().Length == 0;
    }
}
