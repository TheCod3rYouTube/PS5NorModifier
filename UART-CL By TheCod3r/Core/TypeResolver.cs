using Spectre.Console.Cli;

namespace UART_CL_By_TheCod3r.Core;

public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver, IDisposable
{
	public object? Resolve(Type? type)
	{
		if (type is null)
		{
			return null;
		}

		return provider.GetService(type);
	}

	public void Dispose()
	{
		if (provider is IDisposable disposable)
		{
			disposable.Dispose();
		}
	}
}
