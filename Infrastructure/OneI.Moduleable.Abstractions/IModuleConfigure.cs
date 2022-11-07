namespace OneI.Moduleable;

using System.Threading.Tasks;

public interface IModuleConfigure
{
    ValueTask ConfigureAsync(ModuleConfigureContext context);
}
