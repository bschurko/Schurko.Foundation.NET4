using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

#nullable enable
namespace Schurko.Foundation.IoC.DI
{
  public static class IoC
  {
    private static IServiceProvider _hostServices;
    private static HostApplicationBuilder _hostBuilder;
    private static IServiceCollection _serviceCollection;

    static IoC()
    {
      HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder();
      Schurko.Foundation.IoC.DI.IoC._serviceCollection = applicationBuilder.Services;
      Schurko.Foundation.IoC.DI.IoC._hostBuilder = applicationBuilder;
    }

    public static T GetService<T>(this IHost host) => host.Services.GetRequiredService<T>();

    public static IHost InitDependency(Action<IServiceCollection> createDependencies)
    {
      createDependencies(Schurko.Foundation.IoC.DI.IoC._serviceCollection);
      return Schurko.Foundation.IoC.DI.IoC._hostBuilder.Build();
    }
  }
}
