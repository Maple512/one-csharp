namespace OneI.Logable.Formatting;

using System;
using System.Collections.Generic;
using OneI.Logable.Rendering;

public static class LevelFormatter
{
    private static readonly string[][] _titleCaseLevelMap = {
        new []{ "V", "Vb", "Vrb", "Verb", "Verbo", "Verbos", "Verbose" },
        new []{ "D", "De", "Dbg", "Dbug", "Debug" },
        new []{ "I", "In", "Inf", "Info", "Infor", "Inform", "Informa", "Informat", "Informati", "Informatio", "Information" },
        new []{ "W", "Wn", "Wrn", "Warn", "Warni", "Warnin", "Warning" },
        new []{ "E", "Er", "Err", "Eror", "Error" },
        new []{ "F", "Fa", "Ftl", "Fatl", "Fatal" }
    };
    private static readonly string[][] _lowerCaseLevelMap = {
        new []{ "v", "vb", "vrb", "verb", "verbo", "verbos", "verbose" },
        new []{ "d", "de", "dbg", "dbug", "debug" },
        new []{ "i", "in", "inf", "info", "infor", "inform", "informa", "informat", "informati", "informatio", "information" },
        new []{ "w", "wn", "wrn", "warn", "warni", "warnin", "warning" },
        new []{ "e", "er", "err", "eror", "error" },
        new []{ "f", "fa", "ftl", "fatl", "fatal" }
    };
    private static readonly string[][] _upperCaseLevelMap = {
        new []{ "V", "VB", "VRB", "VERB", "VERBO", "VERBOS", "VERBOSE" },
        new []{ "D", "DE", "DBG", "DBUG", "DEBUG" },
        new []{ "I", "IN", "INF", "INFO", "INFOR", "INFORM", "INFORMA", "INFORMAT", "INFORMATI", "INFORMATIO", "INFORMATION" },
        new []{ "W", "WN", "WRN", "WARN", "WARNI", "WARNIN", "WARNING" },
        new []{ "E", "ER", "ERR", "EROR", "ERROR" },
        new []{ "F", "FA", "FTL", "FATL", "FATAL" }
    };

    public static string GetLevelMoniker(LogLevel level, string? format = null)
    {
        if(level is < 0 or > LogLevel.Fatal)
        {
            return Casing.Format(level.ToString(), format);
        }

        if(format.IsNullOrEmpty())
        {
            return Casing.Format(level.ToString());
        }

        if(format.Length is not 2 and not 3)
        {
            return Casing.Format(GetLastLevelMoniker(_titleCaseLevelMap, (int)level), format);
        }

        const char zero = '0';

        var width = format[1] - zero;

        if(format.Length == 3)
        {
            width *= 10;
            width += format[2] - zero;
        }

        // 不是有效数字 
        if(width < 1)
        {
            return string.Empty;
        }

        return format[0] switch
        {
            LoggerDefinition.Formatters.Lower => level.ToString(),//GetLevelMoniker(_lowerCaseLevelMap, (int)level, width),
            LoggerDefinition.Formatters.Upper => GetLevelMoniker(_upperCaseLevelMap, (int)level, width),
            LoggerDefinition.Formatters.Title => GetLevelMoniker(_titleCaseLevelMap, (int)level, width),
            _ => Casing.Format(GetLastLevelMoniker(_titleCaseLevelMap, (int)level), format),
        };
    }

    private static string GetLevelMoniker(string[][] levelMap, int index, int width)
    {
        var level = levelMap[index];

        return level[Math.Min(width, level.Length) - 1];
    }

    /// <summary>
    /// 取最后一个，默认的
    /// </summary>
    /// <param name="levelMap"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static string GetLastLevelMoniker(string[][] levelMap, int index) => levelMap[index][^1];
}
