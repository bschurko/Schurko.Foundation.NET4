using System.ComponentModel.Composition;
using Schurko.Foundation.IoC.MEF;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    // THIS DOES NOT WORK IN IOC !!! Do not try it.
    [Export(typeof(IGearbox))]
    [Export(typeof(IGearSelector))]
    [ExportMetadata("Default", "IGearSelector")]
    public class ThreeSpeedAutoGearBox : IGearbox, IGearSelector
    {
        private IGearSelector gearSelector = DependencyInjector.Resolve<IGearSelector>();

        #region IGearbox Members

        public IGearSelector GearSelector
        {
            get
            {
                return gearSelector;
            }
        }

        public int CurrentGear { get; private set; }

        public int NumberOfGears
        {
            get { return 3; }
        }

        public int SelectGear(int speed)
        {
            CurrentGear = gearSelector.GetBestGear(this, speed);

            return CurrentGear;
        }

        #endregion

        #region IGearSelector Members

        public int GetBestGear(IGearbox gearbox, int speed)
        {
            if (speed <= 10) return 1;
            if (speed <= 40) return 2;
            return 3;
        }

        #endregion
    }
}
