using UnityEngine;
using System.Collections;
using System.IO;

public class CSVLoader 
{
    public bool NextLine()
    {
        if (m_LineIndex < m_LineMaxNum)
        {
            m_CurrentLine = m_Line[m_LineIndex++].Split(m_DivValue, System.StringSplitOptions.None);
            m_ValueMaxNum = (uint)m_CurrentLine.GetLength(0);
            m_ValueIndex = 0;
            return true;
        }

        return false;
    }

    public string GetString()
    {
        if (m_CurrentLine != null)
        {
            if (m_ValueIndex < m_ValueMaxNum)
            {
                return m_CurrentLine[m_ValueIndex++]; 
            }
        }
        return null;
    }

    public int GetInt()
    {
        string str_int = GetString();
        int return_int;
        if (int.TryParse(str_int, out return_int))
        {
            return return_int;
        }

        return 0;
    }

    public float GetFloat()
    {
        string str_float = GetString();
        float return_float;
        if (float.TryParse(str_float, out return_float))
        {
            return return_float;
        }

        return 0.0f;
    }

    public void SetLine(int index)
    {
        if (index < m_LineMaxNum)
        {
            m_CurrentLine = m_Line[index].Split(m_DivValue, System.StringSplitOptions.None);
            m_ValueMaxNum = (uint)m_CurrentLine.GetLength(0);
            m_ValueIndex = 0;
        }
    }

    public CSVLoader(string fileName)
    {
        Init(fileName);
    }

    ~CSVLoader()
    {
    }

    protected void Init(string fileName)
    {
        CFileManager file = CFileManager.GetInstance;
        Stream ms = file.LoadFile(fileName);
        if (ms == null)
            return;
        StreamReader sr = new StreamReader(ms);
            
        while(!sr.EndOfStream)
        {
            sr.ReadLine();
            m_LineMaxNum++;
        }
        ms.Seek(0, SeekOrigin.Begin);
        m_Line = new string[m_LineMaxNum];
        for (int i = 0; i < m_LineMaxNum; i++)
        {
            m_Line[i] = sr.ReadLine();
        }

        m_CurrentLine = m_Line[m_LineIndex++].Split(m_DivValue, System.StringSplitOptions.None);
        m_ValueMaxNum = (uint)m_CurrentLine.GetLength(0);
    }

    string[] m_Line = null;
    string[] m_CurrentLine = null;
    uint m_LineMaxNum = 0;
    public uint LineMaxNum
    {
        get { return m_LineMaxNum; }
    }
    uint m_LineIndex = 0;   //row
    uint m_ValueMaxNum = 0; //column
    public uint ValueMaxNum
    {
        get { return m_ValueMaxNum; }
    }
    uint m_ValueIndex = 0;
    public uint ValueIndex
    {
        set
        {
            if (value < m_ValueIndex)
                m_ValueIndex = value;
        }
    }

    string[] m_DivValue = new string[] { "," };
}
