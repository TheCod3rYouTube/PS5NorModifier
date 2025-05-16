using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace UART_CL_By_TheCod3r.Core;

public class TypeRegistrar(IServiceCollection builder) : ITypeRegistrar
{
	public ITypeResolver Build()
	{
		return new TypeResolver(builder.BuildServiceProvider());
	}

	public void Register(Type service, Type implementation)
	{
		builder.AddSingleton(service, implementation);
	}

	public void RegisterInstance(Type service, object implementation)
	{
		builder.AddSingleton(service, implementation);
	}

	public void RegisterLazy(Type service, Func<object> func)
	{
		ArgumentNullException.ThrowIfNull(func);

		builder.AddSingleton(service, (provider) => func());
	}
}
