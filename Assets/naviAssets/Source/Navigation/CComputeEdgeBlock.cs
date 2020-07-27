using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//@ Manage Edge
public class CComputeEdgeBlock
{
    //@ Edge
    class EDGE__
    {
        public Vector2 _v2Pnt0;
        public Vector2 _v2Pnt1;

        public void SetEdge(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
        {
            _v2Pnt0.x = v3Pnt0_.x;
            _v2Pnt0.y = v3Pnt0_.z;

            _v2Pnt1.x = v3Pnt1_.x;
            _v2Pnt1.y = v3Pnt1_.z;
        } // public void SetEdge(Vector3 v3Pnt0_, Vector3 v3Pnt1_)

        public void SetEdge(Vector2 v2Pnt0_, Vector2 v2Pnt1_)
        {
            _v2Pnt0 = v2Pnt0_;
            _v2Pnt1 = v2Pnt1_;
        } // public void SetEdge(Vector2 v2Pnt0_, Vector2 v2Pnt1_)

        //@ Check cross edge
        public bool DoesCrossEdge2D(EDGE__ edgeRight)
        {
            return CMATH.lineCross2D(_v2Pnt0, _v2Pnt1, edgeRight._v2Pnt0, edgeRight._v2Pnt1);
        } // public bool IsCrossEdge(EDGE__)

        public bool DoesCrossEdge2D(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
        {
            Vector2 v2Pnt0_ = new Vector2(v3Pnt0_.x, v3Pnt0_.z);
            Vector2 v2Pnt1_ = new Vector2(v3Pnt1_.x, v3Pnt1_.z);

            return CMATH.lineCross2D(_v2Pnt0, _v2Pnt1, v2Pnt0_, v2Pnt1_);
        }

        public bool DoesCrossEdge2D(Vector2 v2Pnt0_, Vector2 v2Pnt1_)
        {
            return CMATH.lineCross2D(_v2Pnt0, _v2Pnt1, v2Pnt0_, v2Pnt1_);
        }
        //@ Check cross edge // 

        public EDGE__(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
        {
            _v2Pnt0.x = v3Pnt0_.x;
            _v2Pnt0.y = v3Pnt0_.z;

            _v2Pnt1.x = v3Pnt1_.x;
            _v2Pnt1.y = v3Pnt1_.z;
        } // public EDGE__(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
    }; // struct EDGE__

    //@ To visible optimization shortest way to Goal 
    List<EDGE__> m_listEdgeBlock_ = new List<EDGE__>();
    List<int> m_listSeqBlockTri = new List<int>();

    //@ Debug Process
    List<Vector3> m_listLines = new List<Vector3>();

    CDrawRenderLineDebug m_managerRenderLineDebug = new CDrawRenderLineDebug();
    int m_iRenderLine = -1;

    //@ External Function
    //@ Get/Set
    public int getCntEdgeBlock()
    {
        return m_listEdgeBlock_.Count;
    }

    public int getCntTriBlock()
    {
        return m_listSeqBlockTri.Count;
    }

    void AddToBlockEdge(EDGE__ edgeInsert)
    {
        bool bExistSameEdge = false;

        foreach (EDGE__ edgeBlockIter in m_listEdgeBlock_)
        {
            if (edgeBlockIter == edgeInsert)
            {
                bExistSameEdge = true;
                break;
            }
        } // foreach (EDGE__ edgeBlockIter in m_listEdgeBlock_)

        //@ check overlap
        if (false == bExistSameEdge)
        {
            m_listEdgeBlock_.Add(edgeInsert);
        } // if (false == bExistSameEdge)
    } // void AddToBlockEdge(EDGE__ edgeInsert)

    public void AddToBlockEdge(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
    {
        EDGE__ edgeBlock = new EDGE__(v3Pnt0_, v3Pnt1_);

        AddToBlockEdge(edgeBlock);
    }

    public void AddToBlockEdge_Tri(CTRI tri_B, int iSeqTri)
    {
        if (m_listSeqBlockTri.Count > 0)
        {
            if (-1 < m_listSeqBlockTri.BinarySearch(iSeqTri))
            {
                return;
            }
        } // if (m_listSeqBlockTri.Count > 0)

        EDGE__ edge01 = new EDGE__(tri_B._arrv3PT[0], tri_B._arrv3PT[1]);
        EDGE__ edge12 = new EDGE__(tri_B._arrv3PT[1], tri_B._arrv3PT[2]);
        EDGE__ edge20 = new EDGE__(tri_B._arrv3PT[2], tri_B._arrv3PT[0]);

        AddToBlockEdge(edge01);
        AddToBlockEdge(edge12);
        AddToBlockEdge(edge20);
    } // void AddToBlockEdgeTri(CTRI tri_B)

    public void SetDrawAllEdge()
    {
        if (m_listLines.Count < 1)
        {
            return;
        }

        Vector3[] arrv3Pos = new Vector3[m_listLines.Count];
        m_listLines.CopyTo(arrv3Pos);

        m_iRenderLine = m_managerRenderLineDebug.DrawNewLine_user(arrv3Pos);
    } // public void SetDrawAllEdge()

    public void SetUndrawAllEdge()
    {
        m_managerRenderLineDebug.DeleteDrawLine(m_iRenderLine);
    }
    //@ Debug Process // 

    //@ by inear search to checkup line cross.
    public bool crossBlockIteration(Vector2 v2Pnt0_, Vector2 v2Pnt1_)
    {
        foreach (EDGE__ edgeCurr in m_listEdgeBlock_)
        {
            bool bCrossEdge = edgeCurr.DoesCrossEdge2D(v2Pnt0_, v2Pnt1_);
            if (true == bCrossEdge)
            {
                return true;
            }
        } // foreach (EDGE__ edgeCurr in m_listEdgeBlock_)

        return false;
    } // public bool crossBlockIteration()

    //@ by inear search to checkup line cross.
    public bool crossBlockIteration(Vector3 v3Pnt0_, Vector3 v3Pnt1_)
    {
        foreach (EDGE__ edgeCurr in m_listEdgeBlock_)
        {
            bool bCrossEdge = edgeCurr.DoesCrossEdge2D(v3Pnt0_, v3Pnt1_);
            if (true == bCrossEdge)
            {
                return true;
            }
        } // foreach (EDGE__ edgeCurr in m_listEdgeBlock_)

        return false;
    } // public bool crossBlockIteration()

    //@ Process
    public void InitEdgeBlock()
    {
        m_managerRenderLineDebug.InitDrawRenderLine();
    }

    public void Release_Edge()
    {
        SetUndrawAllEdge();

        m_listEdgeBlock_.Clear();
        m_listSeqBlockTri.Clear();

        m_listLines.Clear();
    }
} // public class CComputeEdgeBlock
