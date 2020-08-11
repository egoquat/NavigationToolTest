using System.IO;
using System.Collections;
using UnityEngine;


public static class CommonPath
{
    public static bool IsExistFileOrDirectory(string path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }

    public static bool IsSamePath(string path01, string path02)
    {
        return (0 == string.Compare(path01.Replace("\\", "/"), path02.Replace("\\", "/"), true));
    }

    public static bool IsMatchedFileOnPath(string fileName, string path)
    {
        return (0 == string.Compare(
            path.Replace("\\", "/").Substring((path.Length - fileName.Length)), fileName, true));
    }

    public static bool IsContainsPath(string path01, string path02)
    {
        string path01__ = path01.Replace("\\", "/").ToLower();
        string path02__ = path02.Replace("\\", "/").ToLower();
        return (path01__.Contains(path02__) || path02__.Contains(path01__));
    }

    public static string GetPathExceptFile(string path)
    {
        string path__ = path.Replace("\\", "/");
        if (path__.IndexOf("/") < 0) return path__;
        return path__.Substring(0, path__.LastIndexOf("/"));
    }

    public static string GetFileExceptPath(string path)
    {
        string path__ = path.Replace("\\", "/");
        if (path__.IndexOf("/") < 0) return path__;
        return path__.Substring(path__.LastIndexOf("/") + 1);
    }
}
