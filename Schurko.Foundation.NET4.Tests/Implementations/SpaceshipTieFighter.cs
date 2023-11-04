using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    /// <summary>
    /// The spaceship tie fighter.
    /// </summary>
    [Export(typeof(ISpaceship))]
    [ExportMetadata("Speed", "Fast")]
    [ExportMetadata("Class", SpaceShipClass.Military)]
    public class SpaceshipTieFighter : ISpaceship
    {
        #region ISpaceship Members
        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "Tie Fighter";
            }
        }
        #endregion
    }
}
