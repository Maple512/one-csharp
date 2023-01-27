namespace OneE.MSBuildTaskClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// The task example1.
    /// </summary>
    public class TaskExample1 : Task
    {
        [Required]
        public string SettingClassName { get; set; }

        [Required]
        public string SettingNamespaceName { get; set; }

        [Required]
        public ITaskItem[] SettingFiles { get; set; }

        [Output]
        public string ClassNameFile { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "任务开始", DateTime.Now);

            //Read the input files and return a IDictionary<string, object> with the properties to be created.
            //Any format error it will return not succeed and Log.LogError properly
            var (success, settings) = ReadProjectSettingFiles();
            if(!success)
            {
                return !Log.HasLoggedErrors;
            }
            //Create the class based on the Dictionary
            success = CreateSettingClass(settings);

            return !Log.HasLoggedErrors;
        }

        private (bool, IDictionary<string, object>) ReadProjectSettingFiles()
        {
            var values = new Dictionary<string, object>();
            foreach(var item in SettingFiles)
            {
                var lineNumber = 0;

                var settingFile = item.GetMetadata("FullPath");
                foreach(var line in File.ReadLines(settingFile))
                {
                    lineNumber++;

                    var lineParse = line.Split(':');
                    if(lineParse.Length != 3)
                    {
                        Log.LogError(subcategory: null,
                                     errorCode: "APPS0001",
                                     helpKeyword: null,
                                     file: settingFile,
                                     lineNumber: lineNumber,
                                     columnNumber: 0,
                                     endLineNumber: 0,
                                     endColumnNumber: 0,
                                     message: "Incorrect line format. Valid format prop:type:defaultvalue");
                        return (false, null);
                    }

                    var value = GetValue(lineParse[1], lineParse[2]);
                    if(!value.Item1)
                    {
                        return (value.Item1, null);
                    }

                    values[lineParse[0]] = value.Item2;
                }
            }

            return (true, values);
        }

        private (bool, object) GetValue(string type, string value)
        {
            try
            {
                switch(type)
                {
                    // So far only few types are supported values.
                    case "string":
                        return (true, value);
                    case "int":
                        return (true, int.Parse(value));
                    case "long":
                        return (true, long.Parse(value));
                    case "guid":
                        return (true, Guid.Parse(value));
                    case "bool":
                        return (true, bool.Parse(value));
                    default:
                        Log.LogError($"Type not supported -> {type}");
                        return (false, null);
                }

            }
            catch
            {
                Log.LogError($"It is not possible parse some value based on the type -> {type} - {value}");
                return (false, null);
            }
        }

        private bool CreateSettingClass(IDictionary<string, object> settings)
        {
            try
            {
                ClassNameFile = $"{SettingClassName}.generated.cs";
                File.Delete(ClassNameFile);
                var settingsClass = new StringBuilder(1024);
                // open namespace
                settingsClass.Append($@" using System;
 namespace {SettingNamespaceName} {{
  public class {SettingClassName} {{
");
                //For each element in the dictionary create a static property
                foreach(var keyValuePair in settings)
                {
                    var typeName = GetTypeString(keyValuePair.Value.GetType().Name);
                    settingsClass.Append($"    public readonly static {typeName}  {keyValuePair.Key} = {GetValueString(keyValuePair, typeName)};\r\n");
                }
                // close namespace and class
                settingsClass.Append(@"  }
}");
                File.WriteAllText(ClassNameFile, settingsClass.ToString());
            }
            catch(Exception ex)
            {
                //This logging helper method is designed to capture and display information from arbitrary exceptions in a standard way.
                Log.LogErrorFromException(ex, showStackTrace: true);
                return false;
            }

            return true;
        }

        private string GetTypeString(string typeName)
        {
            switch(typeName)
            {
                case "String":
                    return "string";
                case "Boolean":
                    return "bool";
                case "Int32":
                    return "int";
                case "Int64":
                    return "long";
                default:
                    return typeName;
            }
        }

        private static object GetValueString(KeyValuePair<string, object> keyValuePair, string typeName)
        {
            switch(typeName)
            {
                case "Guid":
                    return $"Guid.Parse(\"{keyValuePair.Value}\")";
                case "string":
                    return $"\"{keyValuePair.Value}\"";
                case "bool":
                    return $"{keyValuePair.Value.ToString().ToLower()}";
                default:
                    return keyValuePair.Value;
            }
        }
    }
}
