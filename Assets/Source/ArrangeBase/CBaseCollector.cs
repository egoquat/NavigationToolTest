using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Consist of 1 more triangles on ground basement.
public class CBaseCollector
{
    #region constparameter
    protected static readonly int null_idxbase = -1;
    protected static readonly int sizefont3D = 20;
    protected static readonly Color colorfont3d = Color.yellow;
    #endregion // #region constparameter

    //@ Container.
    public List<CBASE__> m_listBase__ = new List<CBASE__>();
    protected List<CTRI> m_listTrisAll_copy = new List<CTRI>();
    protected List<int> m_listTris_Instant = new List<int>();

    protected int m_iID_Base_DrawForDEBUG = CMATH.INT_MAX_DIV2_4;
    protected int m_iID_Instant_DrawForDEBUG = CMATH.INT_MAX_DIV4;

    protected Color m_colorInstantSelect = Color.Lerp(Color.red, Color.white, 0.5f);
    protected Color m_colorBase__ = Color.Lerp(Color.black, Color.magenta, 0.5f);

    protected float m_fEpsilon_forZfight = CMATH.FEPSILON_F2 + CMATH.FEPSILON_F2;

    //@
    #region fornavitool
    protected CDrawRenderMesh m_drawRenderMesh = null;
    protected CDrawText3D m_drawText3D = null;
    #endregion // fornavitool

    //@ Get/Set
    public int getCountBase()
    {
        return m_listBase__.Count;
    }

    public int getCountTris_Instant()
    {
        return m_listTris_Instant.Count;
    }

    //@ Get/Set
    public List<int> getListInstantTri()
    {
        return m_listTris_Instant;
    }

    public List<CBASE__> getListBase()
    {
        if (m_listBase__.Count < 1)
        {
            return null;
        }

        List<CBASE__> listBase = new List<CBASE__>();

        for (int iSeqBase = 0; iSeqBase < m_listBase__.Count; ++iSeqBase)
        {
            listBase.Add(m_listBase__[iSeqBase]);
        }

        return listBase;
    }

    //@ Collect All BaseTower List which calculated Perfect Square.
    static public bool CollectAll_perfectSqre_exceptGoalTri(CNAVICELL[] arrNavicells, 
                                                            CTriCollector tricollector,
                                                            ref List<CBASE__> listBase)
    {
        int iCntNaviCell = arrNavicells.Length;
        

        if (iCntNaviCell < 1)
        {
            Debug.Log("if(iCntNaviCell < 1)");
            return false;
        }

        List<int> listIdxTrisGoal = tricollector.m_listTris_naviGoal;
        List<int> listIdxTrisStart = tricollector.m_listTris_naviStart;
        List<int> listTriIR = new List<int>();
        CNAVICELL.CADJ_TRI[] arrAdjs_ = tricollector.arrayAllTriAdjacent;

        tricollector.collectAll_IsoscellesRightTriangles(ref listTriIR);

        List<int> listTri_Instant__ = new List<int>();
        bool bValidatedBase_perfectSquare = false;
        foreach (int iIdxTri in listTriIR)
        {
            CNAVICELL.CADJ_TRI adjTri = arrAdjs_[iIdxTri];
            CTRI triCurr = tricollector.getTri(iIdxTri);
            bValidatedBase_perfectSquare = false;

            //@ process skip
            if (true == triCurr.inclinedPlane())
            {
                continue;
            }

            if (-1 < listIdxTrisGoal.BinarySearch(iIdxTri))
            {
                continue;
            }

            if (-1 < listIdxTrisStart.BinarySearch(iIdxTri))
            {
                continue;
            }

            int iEdge_Hyptenuse = triCurr.GetHypotenuseEdge();
            if (CTRI.NULL_TRI_EDGE == iEdge_Hyptenuse)
            {
                continue;
            }

            int iTri_Hypotenuse = adjTri._arrTrisAdjacent[iEdge_Hyptenuse];
            if (iTri_Hypotenuse == CNAVICELL.NULL_CELL)
            {
                continue;
            }

            CTRI triHyptenuse = tricollector.getTri(iTri_Hypotenuse);
            if (triHyptenuse.IsisoscelesRightAngle() == true)
            {
                bValidatedBase_perfectSquare = true;
                listTri_Instant__.Add(iIdxTri);
                listTri_Instant__.Add(iTri_Hypotenuse);
            }

            if (true == bValidatedBase_perfectSquare)
            {
                CBASE__ base__New = new CBASE__();
                for (int iTri_ = 0; iTri_ < listTri_Instant__.Count; ++iTri_)
                {
                    int iSeqTri_ = listTri_Instant__[iTri_];
                    CTRI triPicked = tricollector.getTri(iSeqTri_);
                    base__New.InsertNewTri(iSeqTri_, triPicked, false);
                }
                bool bAlreadyExisted = false;
                foreach (CBASE__ base__Sub in listBase)
                {
                    if (true == base__Sub.IsSimilar(base__New))
                    {
                        bAlreadyExisted = true;
                        break;
                    }
                } 

                if (false == bAlreadyExisted)
                {
                    base__New.CalculateBase__(iIdxTri);
                    listBase.Add(base__New);
                } 

                listTri_Instant__.Clear();
            } // if(true==bValidatedBase_perfectSquare)
        } // foreach(int iIdxTri in listTriIR)

        return true;
    } // static public bool CollectAll_perfectSqre_exceptGoalTri()


    //@ Find Base
    protected bool AlreadyBaseExisted(CBASE__ base__)
    {
        foreach (CBASE__ baseCurr in m_listBase__)
        {
            if (true == baseCurr.IsSimilar(base__))
            {
                return true;
            }
        }

        return false;
    } 


    //@ Search Triangle
    public CBASE__ findBaseByTri(int iIdxTri)
    {
        CBASE__ baseFind = null;
        int iBaseFind = findBasebyTri(iIdxTri);

        if (-1 != iBaseFind)
        {
            baseFind = m_listBase__[iBaseFind];
        }

        return baseFind;
    }

    protected int findBasebyTri(int iIdxTri)
    {
        bool bFindOut = false;
        int iIdxObstFind = -1;

        for (int iSeqOb = 0; iSeqOb < m_listBase__.Count; ++iSeqOb)
        {
            CBASE__ baseCurr = m_listBase__[iSeqOb];
            bFindOut = baseCurr.FindinTri_onAB(iIdxTri);
            if (true == bFindOut)
            {
                iIdxObstFind = iSeqOb;
                break;
            }
        }

        return iIdxObstFind;
    }

    protected bool beExistedBase_(int iIdxTri)
    {
        if (-1 < findBasebyTri(iIdxTri))
        {
            return true;
        }
        return false;
    }

    protected void DrawInstantTris(List<int> listTris_Instant)
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        m_drawRenderMesh.DeleteDrawMesh_Range(m_iID_Instant_DrawForDEBUG, m_iID_Instant_DrawForDEBUG + m_listTrisAll_copy.Count);

        foreach (int iterIdxTri in listTris_Instant)
        {
            CTRI triRenderDebug = m_listTrisAll_copy[iterIdxTri];

            //@ Just For Draw Debug Triangles
            m_drawRenderMesh.DrawNewRendermesh(m_iID_Instant_DrawForDEBUG + iterIdxTri,
                                                    triRenderDebug._arrv3PT,
                                                    m_colorInstantSelect,
                                                    m_fEpsilon_forZfight,
                                                    CDrawRenderMesh.scaleadjust_render_093);
        }
    }

    virtual public bool delete_Base_byTri(int iIdxTri)
    {
        int iSeqBase = findBasebyTri(iIdxTri);

        if (-1 < iSeqBase)
        {
            m_listBase__.RemoveAt(iSeqBase);

            return true;
        }

        return false;
    }

    //@ Process
    public void Clear_InstantTriangles_All()
    {
        m_listTris_Instant.Clear();

        if (null == m_drawRenderMesh)
        {
            return;
        }

        m_drawRenderMesh.DeleteDrawMesh_Range(m_iID_Instant_DrawForDEBUG, m_iID_Instant_DrawForDEBUG + m_listTrisAll_copy.Count);
    }

    public bool exist_inInstant(int iIdxTri)
    {
        return (-1 < m_listTris_Instant.BinarySearch(iIdxTri));
    }

    //@ Process delete Tri, which is going to be one base.
    public bool deleteInstantTriOnAB(int iIdxTri)
    {
        if (m_listTris_Instant.Count < 1)
        {
            return false;
        }

        bool bRemovedInstantTris = m_listTris_Instant.Remove(iIdxTri);

        if (true == bRemovedInstantTris)
        {
            m_listTris_Instant.Sort();

            if (null != m_drawRenderMesh)
            {
                m_drawRenderMesh.DeleteDrawMesh(m_iID_Instant_DrawForDEBUG + iIdxTri);
                DrawInstantTris(m_listTris_Instant);
            }

            return true;
        }

        return false;
    }

    //@ Process Instant Tri, which is going to be one base.
    public bool insertNewInstantTri(int iIdxTri)
    {
        bool bDeletedInstantTriCore = deleteInstantTriOnAB(iIdxTri);

        if (true == bDeletedInstantTriCore)
        {
            return true;
        }

        if (0 > iIdxTri) return false;

        bool bSkipAlreadyExisted = false;
        if (m_listTris_Instant.Count > 0)
        {
            bSkipAlreadyExisted = (-1 < m_listTris_Instant.BinarySearch(iIdxTri));
            if (true == bSkipAlreadyExisted)
            {
                return false;
            }
        }

        bSkipAlreadyExisted = bSkipAlreadyExisted | (-1 < findBasebyTri(iIdxTri));

        if (true == bSkipAlreadyExisted)
        {
            return false;
        }

        m_listTris_Instant.Add(iIdxTri);
        m_listTris_Instant.Sort();

        DrawInstantTris(m_listTris_Instant);

        return true;
    } // virtual public bool insertNewInstantTri(int iIdxTri)

    

    //@ Construct New Base__
    public int setNewBase__()
    {
        return setNewBase_fromListTris(m_listTris_Instant);
    }

    public bool newSquareBaseFromTri(int iIdxTri, bool bOnlyPair, CTriCollector tricollector)
    {
        bool bDeletedBaseCoreSub = delete_Base_byTri(iIdxTri);

        if (true == bDeletedBaseCoreSub)
        {
            return false;
        }

        if (true == beExistedBase_(iIdxTri))
        {
            return false;
        }

        int iTriHypotenuse = tricollector.searchTriHypotenuse(iIdxTri);
        if (-1 == iTriHypotenuse)
        {
            if (true == bOnlyPair)
            {
                return false;
            }
        }
        else
        {
            m_listTris_Instant.Add(iTriHypotenuse);
        } // if (-1 == iTriHypotenuse)

        m_listTris_Instant.Add(iIdxTri);

        setNewBase_fromListTris(m_listTris_Instant);

        return true;
    }

    //@ Construct New Base__ List
    public bool setNewBase_FromListBase(List<CBASE__> listBase_)
    {
        if (m_listTrisAll_copy.Count < 1 || listBase_.Count < 1)
        {
            Debug.Log("(m_listTrisAll_copy.Count < 1)=" + (m_listTrisAll_copy.Count < 1) + "//(listBase_.Count < 1)=" + (listBase_.Count < 1));
            return false;
        }

        //@ Clear All Base
        ClearBaseAll();

        foreach (CBASE__ baseIter in listBase_)
        {
            m_listBase__.Add(baseIter);
        }

        return true;
    } // public bool setNewBase_fromListTris( List<CBASE__> listBase_ )

    //@ Construct New Base__
    protected int setNewBase_fromListTris(List<int> listTriIdx_Instant)
    {
        if (m_listTrisAll_copy.Count < 1 || listTriIdx_Instant.Count < 1)
        {
            Debug.Log(" (m_listTrisAll_copy.Count < 1)=" + (m_listTrisAll_copy.Count < 1) + ") || (listTriIdx_Instant.Count)=" + (listTriIdx_Instant.Count) + "");

            return -1;
        }

        int iFirstIdx = listTriIdx_Instant[0];
        CBASE__ baseNew = new CBASE__();

        for (int iTri_ = 0; iTri_ < listTriIdx_Instant.Count; ++iTri_)
        {
            int iSeqTri_ = listTriIdx_Instant[iTri_];
            CTRI triPicked = m_listTrisAll_copy[iSeqTri_];
            baseNew.InsertNewTri(iSeqTri_, triPicked, false);
        }

        baseNew.CalculateBase__(iFirstIdx);
        m_listBase__.Add(baseNew);

        Clear_InstantTriangles_All();

        return m_listBase__.Count;
    }
        

    virtual public void setTrisallToBase(List<CTRI> listTriAll)
    {
        DestructBaseAll();
        m_listTrisAll_copy = new List<CTRI>(listTriAll);
    }

    //@ Process
    virtual public void InitBaseCollector()
    {
        if ( true == processCycle.GetInstance._modeTool)
        {
            if (null == m_drawRenderMesh)
            {
                m_drawRenderMesh = new CDrawRenderMesh();
            }

            m_drawRenderMesh.InitDrawRenderMesh();

            if (null != processCycle.GetInstance.m_drawText3D_src)
            {
                m_drawText3D = (CDrawText3D)GameObject.Instantiate(processCycle.GetInstance.m_drawText3D_src);
            }
        }
    }

    public void ClearBaseAll()
    {
        _SetUndraw_Bases();

        m_listBase__.Clear();
        m_listTris_Instant.Clear();
    }

    public void DestructBaseAll()
    {
        _SetUndraw_Bases();

        foreach (CBASE__ base_ in m_listBase__)
        {
            base_.ReleaseBase__();
        }
        m_listBase__.Clear();
        m_listTris_Instant.Clear();
        m_listTrisAll_copy.Clear();
    }

    virtual public void _SetDrawDebug_Base__All()
    {
        _SetDraw_Bases();
    }

    virtual protected void _SetUndrawDebugBase_All()
    {

    }

    //@ Debug Draw
    protected void _SetDraw_Bases()
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        m_drawRenderMesh.DeleteDrawMesh_Range(m_iID_Instant_DrawForDEBUG, m_listTrisAll_copy.Count);     //@ Delete Instance Tris
        m_drawRenderMesh.DeleteDrawMesh_Range(m_iID_Base_DrawForDEBUG, m_listTrisAll_copy.Count);        //@ Delete Base__ Polygons

        if (getCountBase() > 0)
        {
            List<CBASE__> listBase = m_listBase__;
            for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)
            {
                CBASE__ baseCurr = listBase[iSeqBase];

                Vector3[] arrv3PntsSrc = baseCurr.getArrayPoint_src();

                m_drawRenderMesh.DrawNewRendermesh(m_iID_Base_DrawForDEBUG + iSeqBase,
                                                        arrv3PntsSrc,
                                                        m_colorBase__,
                                                        m_fEpsilon_forZfight,
                                                        CDrawRenderMesh.scaleadjust_render_093);
            }
        } // if (getCountBase() > 0)

    } // protected void _SetDraw_Bases()

    protected void _SetUndraw_Bases()
    {
        if (null != m_drawRenderMesh)
        {
            m_drawRenderMesh.DeleteDrawMesh_All();
        }

        if (null != m_drawText3D)
        {
            m_drawText3D.clear3DTextAll();
        }
    }
} // public class CBaseCollector
