using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    /// <summary>
    /// The default string processor.
    /// </summary>
    public class DefaultStringProcessor : IStringProcessor
    {
        /// <summary>
        /// The process string.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ProcessString(string inputString)
        {
            return inputString;
        }
    }
}
