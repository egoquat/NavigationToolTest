using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public static class StreamHelper
{
    #region region_filesize
    const int _numKillo = 1 << 10;
    const int _numMega = _numKillo << 10;

    private static double ByteToKillo(long numByte)
    {
        return ((double)numByte / (double)_numKillo);
    }

    private static double ByteToMega(long numByte)
    {
        return ((double)numByte / (double)_numMega);
    }

    public static long GetFileSizeByte(string pathFile)
    {
        long sizeFileByte = -1;
        if (true == File.Exists(pathFile))
        {
            FileInfo fileBundle = new FileInfo(pathFile);
            sizeFileByte = fileBundle.Length;
        }
        else
        {
            Debug.LogError("StreamHelper.GetFileSizeByte Path not found : " + pathFile);
        }

        //Debug.Log("FileSize Byte : " + sizeFileByte + " / Mega : " + ByteToMega(sizeFileByte) + " / Killo : " + ByteToKillo(sizeFileByte) + " / FilePath : " + pathFile); //testdebug

        return sizeFileByte;
    }

    public static long GetFileSizeKilloByte(string pathFile)
    {
        return (long)ByteToKillo(GetFileSizeByte(pathFile));
    }

    public static long GetFileSizeMegaByte(string pathFile)
    {
        return (long)ByteToMega(GetFileSizeByte(pathFile));
    }
    #endregion // region_filesize

    #region region_streambytes
    public static byte[] GetBytesFromObject(UnityEngine.Object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    public static byte[] GetBytesFromFile(string pathFile)
    {
        bool existFile = File.Exists(pathFile);

        if (false == existFile)
        {
            Debug.LogError("GetBytesFromFile (false == existFile) pathFile : " + pathFile);
            return null;
        }

        return File.ReadAllBytes(pathFile);
    }

    #endregion

    #region region_iteration
    private static string GetCuttingExt(string ext)
    {
        if (true == string.IsNullOrEmpty(ext))
        {
            return string.Empty;
        }
        else
        {
            string newExt = ext;
            if (newExt.Substring(0, 1) != ".")
            {
                newExt = "." + newExt;
            }
            return newExt;
        }
    }

    private static string[] GetCuttingExts(params string[] extTable)
    {
        string[] skipExts = null;
        if (null != extTable && extTable.Length > 0)
        {
            List<string> listSkipExts = new List<string>();
            extTable.Each(e =>
            {
                string newExt = GetCuttingExt(e);
                if (false == newExt.IsNullOrEmpty())
                {
                    listSkipExts.Add(newExt);
                }
            });
            if (listSkipExts.Count > 0)
            {
                skipExts = listSkipExts.ToArray();
            }
        }

        return skipExts;
    }

    public static void AllFilesAction(string pathFolderAbsolute, bool recursively, Action<string> actionFile)
    {
        foreach (string file in Directory.GetFiles(pathFolderAbsolute))
        {
            string f = file.Replace("\\", "/");
            actionFile(f);
        }

        if (true == recursively)
        {
            foreach (string subDir in Directory.GetDirectories(pathFolderAbsolute))
            {
                AllFilesAction(subDir, recursively, actionFile);
            }
        }
    }

    public static void GetAllFilesPath(string pathFolderAbsolute, bool recursively, List<string> pathAbsoluteFiles)
    {
        AllFilesAction(pathFolderAbsolute, recursively, f =>
        {
            pathAbsoluteFiles.Add(f);
        });
    }

    public static string[] GetAllFilesPathWithCondition(string pathFolder, bool recursively, string conditionExt, params string[] exceptExts)
    {
        if (true == conditionExt.IsNullOrEmpty()) { return null; }
        string[] allFiles = GetAllFilesPathWithExcept(pathFolder, recursively, exceptExts);
        if (true == allFiles.IsNullOrEmpty()) { return null; }

        string conditionExt__ = GetCuttingExt(conditionExt);
        if (true == conditionExt__.IsNullOrEmpty()) { return null; }

        return allFiles.Where(p =>
        {
            string ext = Path.GetExtension(p);
            return (0 == string.Compare(ext, conditionExt__, true));
        }).Select(p => { return p.Replace("\\", "/"); }).ToArray();
    }

    public static string[] GetAllFilesPathWithExcept(string pathFolderAbsolute, bool recursively, params string[] exceptExts)
    {
        List<string> pathAllFiles = new List<string>();

        string[] skipExts = GetCuttingExts(exceptExts);

        AllFilesAction(pathFolderAbsolute, recursively, f =>
        {
            bool skip = false;

            if (null != skipExts)
            {
                string ext = Path.GetExtension(f);
                if (false == string.IsNullOrEmpty(ext))
                {
                    skip = Array.Exists(skipExts, e => 0 == string.Compare(e, ext, true));
                }
            }

            if (false == skip)
            {
                pathAllFiles.Add(f);
            }
        });
        return pathAllFiles.ToArray();
    }

    public static int GetCountAllFiles(string pathFolderAbsolute, bool recursively, params string[] exceptExt)
    {
        int counting = 0;
        AllFilesAction(pathFolderAbsolute, recursively, f =>
        {
            if (null != exceptExt && exceptExt.Length > 0)
            {
                string ext = Path.GetExtension(f).Substring(1);
                bool except = false;
                foreach (string e in exceptExt)
                {
                    if (0 == string.Compare(ext, e, true))
                    {
                        except = true;
                        break;
                    }
                }
                if (false == except) counting++;
            }
            else
            {
                counting++;
            }
        });

        return counting;
    }

    #endregion // region_iteration
}
