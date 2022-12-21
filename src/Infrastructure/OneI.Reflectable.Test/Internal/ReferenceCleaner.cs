namespace OneI.Reflectable;

using Fody;
using Mono.Cecil;

public static class ReferenceCleaner
{
    public static void CleanReferences(ModuleDefinition module, BaseModuleWeaver weaver, List<string> referenceCopyLocalPaths, List<string> runtimeCopyLocalPaths, Action<string> log)
    {
        if(!weaver.ShouldCleanReference)
        {
            return;
        }

        var weaverLibName = weaver.GetType().Assembly.GetName().Name.ReplaceCaseless(".Fody", "");
        log($"Removing reference to '{weaverLibName}'.");

        var referenceToRemove = module.AssemblyReferences.FirstOrDefault(x => x.Name == weaverLibName);
        if(referenceToRemove != null)
        {
            module.AssemblyReferences.Remove(referenceToRemove);
        }

        var copyLocalFilesToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            weaverLibName + ".dll",
            weaverLibName + ".xml",
            weaverLibName + ".pdb"
        };

        referenceCopyLocalPaths.RemoveAll(refPath => copyLocalFilesToRemove.Contains(Path.GetFileName(refPath)));
        runtimeCopyLocalPaths.RemoveAll(refPath => copyLocalFilesToRemove.Contains(Path.GetFileName(refPath)));
    }

    public static string ReplaceCaseless(this string str, string oldValue, string newValue)
    {
        var builder = new StringBuilder();

        var previousIndex = 0;
        var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while(index != -1)
        {
            builder.Append(str.Substring(previousIndex, index - previousIndex));
            builder.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
        }

        builder.Append(str.Substring(previousIndex));

        return builder.ToString();
    }
}
