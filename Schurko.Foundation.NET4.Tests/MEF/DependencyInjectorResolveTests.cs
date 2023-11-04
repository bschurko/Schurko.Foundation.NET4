using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.IoC.MEF;
using Schurko.Foundation.Tests.Implementations;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.MEF
{
    /// <summary>
    /// The dependency injector resolve tests.
    /// </summary>
    [TestClass]
    public class DependencyInjectorResolveTests
    {
        /// <summary>
        /// The dependency injector resolve default class.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveDefaultClass()
        {
            // Should Resolve DefaultStringProcessor
            var myObject = DependencyInjector.Resolve<IStringProcessor>(() => new DefaultStringProcessor());

            Assert.IsTrue(myObject is DefaultStringProcessor);
        }

        /// <summary>
        /// The dependency injector resolve default class.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveDefaultClassUsingGeneric()
        {
            // Should Resolve DefaultStringProcessor
            var myObject = DependencyInjector.Resolve<IStringProcessor, DefaultStringProcessor>();

            Assert.IsTrue(myObject is DefaultStringProcessor);
        }

        /// <summary>
        /// The dependency injector resolve default class.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveDefaultClassUsingMeta()
        {
            // Should Resolve DefaultStringProcessor
            var myObject = DependencyInjector.Resolve<IIntProcessor>();

            Assert.IsTrue(myObject is DefaultIntProcessor);
        }

        /// <summary>
        /// The dependency injector resolve default class.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveOverrideClassUsingMeta()
        {
            // Should Resolve DefaultStringProcessor
            var myObject = DependencyInjector.Resolve<IDoubleProcessor>();

            Assert.IsTrue(myObject is OverrideDoubleProcessor);
        }

        /// <summary>
        /// The dependency injector resolve override.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveOverride()
        {
            var myCar = DependencyInjector.Resolve<IVehicle>(() => new VehicleTrabant());

            Assert.IsTrue(myCar is VehicleFerarri);
        }

        /// <summary>
        /// The dependency injector resolve meta.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterStringString()
        {
            var myFastSpaceShip =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter(s => (string)s.Metadata["Speed"] == "Fast") // This Filter is Func<TServiceType, bool> allowing for a very flexible filter
                                  .Resolve();

            Assert.IsTrue(myFastSpaceShip is SpaceshipTieFighter);

            var mySlowSpaceShip = DependencyInjector.AllWithMetaData<ISpaceship>().Filter("Speed", "Slow").Resolve();

            Assert.IsTrue(mySlowSpaceShip is SpaceshipStarDestroyer);
        }

        /// <summary>
        /// Enum filter
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterStringEnumSingle()
        {
            var myCivilianSpaceShip =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Class", SpaceShipClass.Civilian)
                                  .Resolve();

            Assert.IsTrue(myCivilianSpaceShip is SpaceshipCruiser);
        }

        /// <summary>
        /// Enum filter resolve many.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterStringEnumList()
        {
            var myWarships =
                DependencyInjector.AllWithMetaData<ISpaceship>().Filter("Class", SpaceShipClass.Military).ResolveAll();

            Assert.IsTrue(myWarships.Count() == 2);
        }

        /// <summary>
        /// Multi filters.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterMultiFilter()
        {
            var myFastWarship =
                DependencyInjector.AllWithMetaData<ISpaceship>() // Get the Lazy<T> types with meta data ( not yet instantiated )
                                  .Filter("Class", SpaceShipClass.Military) // Apply a filter, returning Lazy<T> with meta data still
                                  .Filter("Speed", "Fast") // So we can chain
                                  .Resolve(); // Call to Resolve return FirstOrDefault<T>() OR ResolveAll returns IEnumerable<T>

            Assert.IsTrue(myFastWarship is SpaceshipTieFighter);

        }

        /// <summary>
        /// Enum based filter with default, should resolve default.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterResolveDefaultSingle()
        {
            var myDefaultShip =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Class", SpaceShipClass.Hybrid)
                                  .Resolve(() => new SpaceTaxi());

            Assert.IsTrue(myDefaultShip is SpaceTaxi);

        }

        /// <summary>
        /// Enum based filter with default, should resolve default.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterResolveDefaultSingleWithGenericType()
        {
            var myDefaultShip =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Class", SpaceShipClass.Hybrid)
                                  .Resolve<ISpaceship, SpaceTaxi>();

            Assert.IsTrue(myDefaultShip is SpaceTaxi);

        }

        /// <summary>
        /// Simple Filter resolve ( String, String ) with a default Many, should resolve defaults.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterResolveDefaultMany()
        {
            var myDefaultShips =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Speed", "Lightning")
                                  .ResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            Assert.IsTrue(myDefaultShips.Count() == 2);
        }

        /// <summary>
        /// Simple Filter resolve ( String, String ) with a default Many, should resolve defaults.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterResolveFilterMissingMany()
        {
            var myDefaultShips =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("NOT_THERE", "Lightning")
                                  .ResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            Assert.IsTrue(myDefaultShips.Count() == 2);
        }

        /// <summary>
        /// Simple Filter resolve ( String, String ) with a default Many, should resolve defaults.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorResolveMetaSimpleFilterResolveFilterMissingSingle()
        {
            var myDefaultShip =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("NOT_THERE", "Lightning")
                                  .Resolve(() => new SpaceshipCruiser());

            Assert.IsTrue(myDefaultShip is SpaceshipCruiser);
        }

        /// <summary>
        /// Appends classes to list after resolve
        /// </summary>
        [TestMethod]
        public void DependencyInjectorAppend()
        {
            var myListOfClasses = DependencyInjector.ResolveAll<ISpaceship>()
                                                    .AppendToResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            var testArray = myListOfClasses.ToArray();

            Assert.IsTrue(
                testArray.Length == 5 &&
                testArray[3] is SpaceTaxi &&
                testArray[4] is SpaceshipCruiser);
        }

        /// <summary>
        /// Appends classes to list after resolve, uses Meta first
        /// </summary>
        [TestMethod]
        public void DependencyInjectorAppendWithMeta()
        {
            var myListOfClasses =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Speed", "Fast")
                                  .ResolveAll()
                                  .AppendToResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            var testArray = myListOfClasses.ToArray();

            Assert.IsTrue(
                    testArray.Length == 3 &&
                    testArray[1] is SpaceTaxi &&
                    testArray[2] is SpaceshipCruiser);
        }

        /// <summary>
        /// Prepends Classes to resolved list
        /// </summary>
        [TestMethod]
        public void DependencyInjectorPrepend()
        {
            var myListOfClasses = DependencyInjector.ResolveAll<ISpaceship>()
                                                    .PrefixToResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            var testArray = myListOfClasses.ToArray();

            Assert.IsTrue(
                testArray.Length == 5 &&
                testArray[0] is SpaceTaxi &&
                testArray[1] is SpaceshipCruiser);
        }

        /// <summary>
        /// Prepends Classes to resolved list, test uses Meta first.
        /// </summary>
        [TestMethod]
        public void DependencyInjectorPrependWithMeta()
        {
            var myListOfClasses =
                DependencyInjector.AllWithMetaData<ISpaceship>()
                                  .Filter("Speed", "Fast")
                                  .ResolveAll()
                                  .PrefixToResolveAll(() => new SpaceTaxi(), () => new SpaceshipCruiser());

            var testArray = myListOfClasses.ToArray();

            Assert.IsTrue(
                    testArray.Length == 3 &&
                    testArray[0] is SpaceTaxi &&
                    testArray[1] is SpaceshipCruiser);
        }

        /// <summary>
        /// More than one of the Interface is exported but we are calling jusr Resolve.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ResolveEntityException), AllowDerivedTypes = true)]
        public void DependencyInjectorShouldThrowExceptionAsMoreThanOneTypeAvailable()
        {
            var myShip = DependencyInjector.Resolve<ISpaceship>(() => new SpaceTaxi());

            Assert.IsTrue(myShip is SpaceTaxi);
        }

        // Default Gearbox with Default Selector

        // this does not work wityh IOC.
        //[TestMethod]
        //public void DependencyInjectorResolveNonAndDefault()
        //{
        //    // We have a class that is a Non Default IGearbox and Default IGearSelector
        //    var gearBox = DependencyInjector.Resolve<IGearbox>();

        //    Assert.IsTrue(gearBox is ThreeSpeedAutoGearBox);
        //    Assert.IsTrue(gearBox.GearSelector is ThreeSpeedAutoGearBox);
        //}

        // Resolve Gearbox with Default Selector


    }
}
