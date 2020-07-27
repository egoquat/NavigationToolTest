using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CDrawRenderLineDebug
{
    protected CRenderLineDebug m_renderLineSource;

    private List<CRenderLineDebug> m_listRenderLine = new List<CRenderLineDebug>();

    //@ Draw Line as Position Array Sequencial 
    // -return : Index of array.
    public int DrawNewLine_user( params Vector3[] arrV3Pos )
    {
        CRenderLineDebug renderLineNew = (CRenderLineDebug)GameObject.Instantiate(m_renderLineSource);
        renderLineNew.DrawLine_user(arrV3Pos);
        
        m_listRenderLine.Add( renderLineNew);

        return m_listRenderLine.Count-1;
    }

    //@ Clear deeply A Line Instance based on Index.
    public void DeleteDrawLine( int iSeq )
    {
        if(1>m_listRenderLine.Count)
        {
            return;
        }

        if(iSeq>=m_listRenderLine.Count)
        {
            return;
        }

        CRenderLineDebug renderLineCurr = (CRenderLineDebug)m_listRenderLine[iSeq];
        GameObject.Destroy(renderLineCurr.gameObject);
        m_listRenderLine.RemoveAt(iSeq);
    }

    //@ Clear All Instances of Lines.
    public void DeleteDrawLineAll()
    {
        if(1>m_listRenderLine.Count)
        {
            return;
        }

        foreach(CRenderLineDebug renderLine in m_listRenderLine)
        {
            GameObject.Destroy(renderLine);
        }

        m_listRenderLine.Clear();
    } // public void DeleteDrawLineAll()

    public void InitDrawRenderLine()
    {
        if (processCycle.GetInstance._modeTool)
        {
            GameObject goRenderLineSource = new GameObject();
            m_renderLineSource = goRenderLineSource.AddComponent<CRenderLineDebug>();
        }
    }
} // public class CDrawRenderLineDebug
