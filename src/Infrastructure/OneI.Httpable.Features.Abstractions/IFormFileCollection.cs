namespace OneI.Httpable;

public interface IFormFileCollection : IReadOnlyList<IFormFile>
{
    IFormFile? this[string name] { get; }

    IFormFile? GetFile(string name);

    IReadOnlyList<IFormFile> GetFiles(string name);
}
