using UnityEngine;
using System.Collections;

public class CUnitTemplate
{
    public static readonly string m_Filter = ".upd";
    public static readonly string m_Forder = "unit/";
    public string m_Filename;

    //  Unit Property
    public int m_ModelCode;
    public string m_ModelName;
    public float m_AttackDamage;
    public float m_Range;
    public float m_AttackTerm;
    public float m_MoveSpeed;
    public float m_Enegy;
    public float m_Shild;
    public int m_Award;

    public bool SetUnit(int unitIndex)
    {
        if (m_Filename == null)
            return false;

        string filepath = m_Forder + m_Filename + m_Filter;
        CSVLoader loader = new CSVLoader(filepath);
        if (loader == null)
            return false;

        for (int i = 0; i < loader.LineMaxNum; i++)
        {
            if (loader.GetInt() == unitIndex)
            {
                loader.ValueIndex = 0;
                SetProperty(loader);
                loader.SetLine(0);

                return true;
            }
            loader.NextLine();
        }

        return false;
    }

    public void SetProperty(CSVLoader dataLoader)
    {
        m_ModelCode = dataLoader.GetInt();
        m_ModelName = dataLoader.GetString();
        m_AttackDamage = dataLoader.GetFloat();
        m_Range = dataLoader.GetFloat();
        m_AttackTerm = dataLoader.GetFloat();
        m_MoveSpeed = dataLoader.GetFloat();
        m_Enegy = dataLoader.GetFloat();
        m_Shild = dataLoader.GetFloat();
        m_Award = dataLoader.GetInt();
    }

    public CUnitTemplate()
    {

    }
    ~CUnitTemplate()
    {
    }
}