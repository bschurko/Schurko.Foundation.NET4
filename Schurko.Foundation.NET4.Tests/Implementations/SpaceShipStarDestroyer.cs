using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    /// <summary>
    /// The spaceship star destroyer.
    /// </summary>
    [Export(typeof(ISpaceship))]
    [ExportMetadata("Speed", "Slow")]
    [ExportMetadata("Class", SpaceShipClass.Military)]
    public class SpaceshipStarDestroyer : ISpaceship
    {
        #region ISpaceship Members
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "Star Destroyer";
            }
        }
        #endregion
    }
}
