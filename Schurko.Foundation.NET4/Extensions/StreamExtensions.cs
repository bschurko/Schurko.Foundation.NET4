
using System.IO;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class StreamExtensions
    {
        public static void CopyStream(
          this Stream sourceStream,
          Stream destinationStream,
          int bufferSize)
        {
            InternalCopyStream(sourceStream, destinationStream, bufferSize);
        }

        internal static void InternalCopyStream(
          Stream sourceStream,
          Stream destinationStream,
          int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            for (int count = sourceStream.Read(buffer, 0, bufferSize); count > 0; count = sourceStream.Read(buffer, 0, bufferSize))
                destinationStream.Write(buffer, 0, count);
        }
    }
}
