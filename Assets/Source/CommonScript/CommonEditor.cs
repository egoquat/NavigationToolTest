using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

public static class CommonEditor
{
    public static void DisplayWindowOk(string title, string content)
    {
        EditorUtility.DisplayDialog(title, content, "ok");
    }

    public static bool DisplayWindowYesOrNo(string title, string content)
    {
        return EditorUtility.DisplayDialog(title, content, "yes", "no");
    }

    public static bool DisplayWindowOkOrCancel(string title, string content)
    {
        return EditorUtility.DisplayDialog(title, content, "ok", "cancel");
    }

    public static string DisplayWindowFolderSelect(string titleDisplayWindow, string pathFolder)
    {
        string pathToSelect = EditorUtility.OpenFolderPanel(titleDisplayWindow, pathFolder, "");
        return string.IsNullOrEmpty(pathToSelect) ? string.Empty : pathToSelect.Replace("\\", "/"); ;
    }

    public static string DisplayWindowFileSelect(string titleDisplayWindow, string pathFolder)
    {
        string pathToSelect = EditorUtility.OpenFilePanel(titleDisplayWindow, pathFolder, "");
        return string.IsNullOrEmpty(pathToSelect) ? string.Empty : pathToSelect.Replace("\\", "/"); ;
    }

    public static void OpenExplorerFolder(string pathFolder)
    {
        if ((false == string.IsNullOrEmpty(pathFolder)) && (true == Directory.Exists(pathFolder) || true == File.Exists(pathFolder)))
        {
            string path = pathFolder.Replace("/", "\\");
            System.Diagnostics.Process.Start("Explorer.exe", "/Select," + path);
        }
        else
        {
            string pathExtract = Path.GetDirectoryName(pathFolder);
            if (true == Directory.Exists(pathExtract))
            {
                System.Diagnostics.Process.Start("Explorer.exe", "/Select," + pathExtract);
            }
            else
            {
                Debug.LogError("OpenExplorerFolder : Can't open path (" + (pathFolder) + ")");
            }
        }
    }

    public static void OpenExplorerFile(string pathFile)
    {
        if ((false == string.IsNullOrEmpty(pathFile)) && (true == Directory.Exists(pathFile) || true == File.Exists(pathFile)))
        {
            string path = pathFile.Replace("/", "\\");
            System.Diagnostics.Process.Start("Explorer.exe", path);
        }
    }
}
