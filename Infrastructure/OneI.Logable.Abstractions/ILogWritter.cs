namespace OneI.Logable;

using System.Threading.Tasks;

public interface ILogWritter
{
    Task WriteAsync(ILoggerContext context);
}
