
using System;
using System.ComponentModel.Composition.Primitives;


#nullable enable
namespace Schurko.Foundation.IoC.MEF
{
    public class ResolveEntityException : Exception
    {
        public string ContractName { get; private set; }

        public ComposablePartCatalog Catalog { get; private set; }

        internal ResolveEntityException(
          string failedContract,
          ComposablePartCatalog catalog,
          string message,
          Exception innerException)
          : base(message, innerException)
        {
            ContractName = failedContract;
            Catalog = catalog;
        }
    }
}
