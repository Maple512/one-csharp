namespace OneI.Logable;

using System.Threading.Tasks;

public interface ILogAppender 
{
    Task AppendAsync();
}
