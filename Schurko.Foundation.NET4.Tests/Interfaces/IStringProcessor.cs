namespace Schurko.Foundation.Tests.Interfaces
{
    /// <summary>
    /// The StringProcessor interface. [UNIT TEST ONLY]
    /// </summary>
    public interface IStringProcessor
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
        string ProcessString(string inputString);
    }
}
