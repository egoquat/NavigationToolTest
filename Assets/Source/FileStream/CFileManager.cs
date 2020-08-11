using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class CachingFile
{
    public string m_Name;
    public Stream m_Stream;

    public CachingFile(string FileName, Stream FileData)
    {
        m_Name = FileName;
        m_Stream = FileData;
    }
}

public sealed class CFileManager
{
    public static CFileManager GetInstance
    {
        get { return Singleton<CFileManager>.GetInstance; }

    }

    public List<string> LoadFileTexts(string pathName)
    {
        Stream ms = LoadFile(pathName);
        if (ms == null)
        {
            return null;
        }

        StreamReader sr = new StreamReader(ms);
        if (true == sr.EndOfStream)
        {
            return null;
        }

        List<string> buffers = new List<string>();
        while (!sr.EndOfStream)
        {
            string linebuffer = sr.ReadLine();
            buffers.Add(linebuffer);
        }

        return buffers;
    }

    public Stream LoadFile(string pFileName)
    {
        string FilePath = m_FolderName + pFileName;
        Stream fs = FindCachingFile(pFileName);
        if (null != fs)
        {
            return fs;
        }

        try
        {
            fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            if (null == fs)
            {
                return null;
            }
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
        catch (Exception e)
        {
            Debug.Log(e);

            return null;
        }

		byte[] data = new byte[fs.Length];
        fs.Read(data, 0, data.Length);
        MemoryStream ms = new MemoryStream(data);                   
        MemoryStream tReturn  = new MemoryStream(data);
        fs.Close();
        AddCachingFile(pFileName, ms);

        return tReturn;
  
    }

    public bool SaveFile(string pFileName, Stream pStream)
    {        
        byte[] data = new byte[pStream.Length];
        pStream.Read(data, 0, data.Length);
        return SaveFile(pFileName, data);
       
    }

    public bool SaveFile(string pFileName, byte[] pData)
    {
        if(pFileName == null)
            return false;
        if (pData == null)
            return false;
        
        string FilePath = m_FolderName + pFileName;
        CreateFileForder(FilePath);
        FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
		ChageCachingData(pFileName,pData);
        fs.Write(pData, 0, pData.Length);
        fs.Close();

        return true;
       
    }

    public void CachingFileRealse()
    {
        lock (_lock)
        {
            //  스트림 닫기 추가
            foreach (CachingFile File in m_CachingFile_List)
            {
                File.m_Stream.Close();
            }
            m_CachingFile_List.Clear();
        }
    }
    
    static public string[] GetFileListInDirectory(string pFileName, string pPatten, string pFilter)
    {
        if (pFileName == null || pFilter == null)
            return null;
        string path = m_FolderName + pFileName;
        string pattern = pPatten + "*" + pFilter;
		string[] temp_str;

		try
		{
        	temp_str = Directory.GetFiles(path, pattern);
		}
		catch(Exception e)
		{
            Debug.LogError("Exception caught in CFileManager.GetFileListInDirectory(). " + e.Message);
			return null;
		}
        for (int i = 0; i < temp_str.Length; i++)
        {
            temp_str[i] = temp_str[i].Remove(0, path.Length );
            temp_str[i] = temp_str[i].Remove(temp_str[i].Length - pFilter.Length);
        }
        return temp_str;
    }
	
	static public void CreateFileForder(string FolderPath)
    {
        //  문자열확인후 생성 폴더 있는지 확인후 없으면 생성
        for (int i = FolderPath.Length; i > 0; --i)
        {
            if (true == FolderPath[i - 1].Equals('\\') ||
                true == FolderPath[i - 1].Equals('/'))
            {
                string testfolder = FolderPath.Substring(0, i - 1);
                if (!Directory.Exists(testfolder))
                    Directory.CreateDirectory(testfolder);
            }
        }
    }
    
    private void AddCachingFile(string pName, Stream pStream)
    {
        CachingFile item = new CachingFile(pName, pStream);

        lock (_lock)
        {
            //  파일정보 추가후 최대 캐싱갯수 넘으면 맨앞 삭제
            m_CachingFile_List.Add(item);
            if (m_MaxCachingNum < m_CachingFile_List.Count)
            {
                //  맨앞 지움
                m_CachingFile_List[0].m_Stream.Close();
                m_CachingFile_List.RemoveAt(0);
            }
        }

    }

    private Stream FindCachingFile(string pFileName)
    {
        lock (_lock)
		{
	        for (int i = 0; i < m_CachingFile_List.Count; i++)
	        {
	            if (m_CachingFile_List[i].m_Name == pFileName)
	            {
					
	                    byte[] data = new byte[m_CachingFile_List[i].m_Stream.Length];
	                    m_CachingFile_List[i].m_Stream.Read(data, 0, data.Length);
	                    m_CachingFile_List[i].m_Stream.Seek(0, SeekOrigin.Begin);
	                    MemoryStream ms = new MemoryStream(data);
					
					return ms;
	            }
	        }
		}
       

        return null;
    }
	
	private bool ChageCachingData(string pFileName, byte[] data)
	{
		lock(_lock)
		{
			for (int i = 0; i < m_CachingFile_List.Count; i++)
	        {
	            if (m_CachingFile_List[i].m_Name == pFileName)
	            {
					m_CachingFile_List[i].m_Stream.Close();
					m_CachingFile_List[i].m_Stream = new MemoryStream(data);
					return true;
						
                }
            }
		}
	
		return false;
	}
    
    CFileManager() { }
    ~CFileManager()
    {
        CachingFileRealse();
    }

    private static readonly string m_FolderName = "Data/";
    private static readonly int m_MaxCachingNum = 10;
    private List<CachingFile> m_CachingFile_List = new List<CachingFile>(m_MaxCachingNum);
    private object _lock = new object();
    
}
