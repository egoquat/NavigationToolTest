using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CToolModuleNavimesh
{
    #region constparameter
    public static readonly int DFLT_IDX_RENDERNAVIMESH = CMATH.INT_MAX_DIV32168;
    public static readonly int DFLT_IDX_RENDERNAVI_TRIS_IR = DFLT_IDX_RENDERNAVIMESH - 1;
    public static readonly int DFLT_IDX_RENDERTRI_TYPE = CMATH.INT_MAX_DIV8;
    #endregion // #region constparameter

    //@ 시작에서 골 경로를 보여주는 디버그 드로우를 위한 네비 셀
    private CNAVICELL[] m_arrSequenceCell;

    //@ DrawRenderMesh for tools
    private CDrawRenderMesh m_drawRenderMesh_navi = new CDrawRenderMesh();
    private CDrawRenderMesh m_drawRenderMesh_triCollector = new CDrawRenderMesh();
    private CDrawRenderLineDebug m_drawRenderLine_triCollector = new CDrawRenderLineDebug();

    private int m_iSequenceRenderLine_forDebug = -1;
    private int m_iSequenceRenderMesh_forDebug = DFLT_IDX_RENDERNAVIMESH;

    private CDrawText3D m_drawText3DStartCell = null;
    private CDrawText3D m_drawText3D_Levels = null;

    private Color m_colorText3D = Color.yellow;
    private int m_iFontSizeText3D = 20;
    Vector3 m_v3PosAdjustText3D = new Vector3(0.2f, 0.4f, 0.0f);
    private int m_iFontSizeText_Level = 8;

    //@ Draw for Debug
    private int m_iBasisUniqueNum_RenderMesh = CMATH.INT_MAX_DIV2_8;
    private bool m_bDrawLevelCellsAll_ = false;

    private List<int> getListSequenceOrganized()
    {
        List<int> listSequence = new List<int>();
        if (null == m_arrSequenceCell)
        {
            return listSequence;
        }

        foreach (CNAVICELL cellCurr in m_arrSequenceCell)
        {
            int iSeqTri = cellCurr.seqCell;
            listSequence.Add(iSeqTri);
        }

        return listSequence;
    }

    //@ 
    public void UndrawStartCellToGoal()
    {
        m_drawRenderMesh_navi.DeleteDrawMesh_All();
    }
    private void SetDrawCellsIndexAll(CNAVICELL[] arrNavicells)   // 셀 레벨 텍스트를 셀 위에 그린다
    {
        if (null == m_drawText3D_Levels)
        {
            return;
        }

        m_drawText3D_Levels.clear3DTextAll();

        Vector3 v3Position;
        Vector3 v3PosAdjust = new Vector3(0.1f, 0.01f, -0.1f);
        Quaternion quaterRotText3D = new Quaternion();
        quaterRotText3D.eulerAngles = new Vector3(90.0f, 180.0f, 0.0f);
        foreach (CNAVICELL navicell in arrNavicells)
        {
            v3Position = navicell.getPos_cellCenter() + v3PosAdjust;
            m_drawText3D_Levels.setNew3DText(v3Position,
                                quaterRotText3D,
                                m_iFontSizeText_Level,
                                Color.red,
                                "" + navicell.leveltogoal + "(" + navicell.seqCell + ")",
                                false);
        }
    } // private void SetDrawCellsIndexAll()

    public void _drawText3D_SeqCells(CTriCollector tricollector, List<int> listCellsIdx)
    {
        if (null == m_drawText3DStartCell || listCellsIdx.Count < 1)
        {
            return;
        }

        m_drawText3DStartCell.clear3DTextAll();

        if (listCellsIdx.Count > 0)
        {
            Quaternion quaternionCoreText3D = new Quaternion();

            Vector3 v3Position;

            int iSequencialNumber = 0;
            foreach (int iIdxTri in listCellsIdx)
            {
                if (iIdxTri < 0)
                {
                    continue;
                }

                v3Position = tricollector.getTri(iIdxTri).GetCenterTri();
                v3Position = v3Position + m_v3PosAdjustText3D;

                m_drawText3DStartCell.setNew3DText(v3Position,
                                        quaternionCoreText3D,
                                        m_iFontSizeText3D,
                                        m_colorText3D,
                                        "" + (iSequencialNumber++) + "(" + iIdxTri + ")",
                                        true);

            } // for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)
        }
    } // private void _drawText3D_SeqCells()

    private void _SetundrawNaviCells_Text3D()
    {
        if (null != m_drawText3DStartCell)
        {
            m_drawText3DStartCell.clear3DTextAll();
        }

        if (null != m_drawText3D_Levels)
        {
            m_drawText3D_Levels.clear3DTextAll();
        }
    } // private void _SetundrawNaviCells_Text3D()

    private void setComponentRenderMesh()
    {
        if (null == processCycle.GetInstance.m_drawText3D_src)
        {
            Debug.Log("ERROR. check m_drawText3D_src link.");
            return;
        }

        m_drawText3DStartCell = (CDrawText3D)GameObject.Instantiate(processCycle.GetInstance.m_drawText3D_src);
        m_drawText3D_Levels = (CDrawText3D)GameObject.Instantiate(processCycle.GetInstance.m_drawText3D_src);
    } 

    public void InitNaviMeshTool()
    {
        if (true == processCycle.GetInstance._modeTool)
        {
            setComponentRenderMesh();
            m_drawRenderMesh_navi.InitDrawRenderMesh();
            m_drawRenderMesh_triCollector.InitDrawRenderMesh();
            m_drawRenderLine_triCollector.InitDrawRenderLine();
        }
    }

    public void ReleaseNaviMeshTool()
    {
        _SetundrawNaviCells_Text3D();
        UndrawStartCellToGoal();

        _desetAllTris_RenderLine_Debug();
        undrawAllTri();

        m_arrSequenceCell = null;
    }

    public bool getDrawLevelCells()
    {
        return m_bDrawLevelCellsAll_;
    }

    //@ For DEBUG, Set Draw way StartCell to Goal
    public void drawStartCellToGoal(CNavigation navigation, CTriCollector tricollector)
    {
        UndrawStartCellToGoal();
        _SetundrawNaviCells_Text3D();

        if (null == navigation.m_arrNavicells) return;

        bool bResultOrganized = navigation.collectWay_allStartsToGoal(tricollector, ref m_arrSequenceCell);

        if (false == bResultOrganized)
        {
            return;
        }

        CNAVICELL[] arrNavicells = navigation.m_arrNavicells;

        List<int> listSeqTri = getListSequenceOrganized();
        int iBasisUniqueNum = m_iBasisUniqueNum_RenderMesh;

        for (int iTriAdj = 0; iTriAdj < listSeqTri.Count; ++iTriAdj)
        {
            int iSeqUniqueNum = iBasisUniqueNum + listSeqTri[iTriAdj];
            CTRI triAdj_ = arrNavicells[listSeqTri[iTriAdj]].trilinkcell;

            m_drawRenderMesh_navi.DrawNewRendermesh(iSeqUniqueNum,
                                                triAdj_._arrv3PT,
                                                Color.Lerp(Color.gray, Color.black, 0.5f),
                                                CMATH.FEPSILON_F3 + CMATH.FEPSILON_F3,
                                                CDrawRenderMesh.scaleadjust_render_070);
        }

        _drawText3D_SeqCells(tricollector, tricollector.m_listTris_naviStart);
    } // private void drawStartCellToGoal()

    public void drawLevelAllCells(CNavigation navigation, bool bDrawTextIdxAll)
    {
        m_bDrawLevelCellsAll_ = bDrawTextIdxAll;

        if (true == m_bDrawLevelCellsAll_)
        {
            if (null == navigation.m_arrNavicells) return;

            SetDrawCellsIndexAll(navigation.m_arrNavicells);
        }
        else
        {
            m_drawText3D_Levels.clear3DTextAll();
        }
    }


    //@ Triangle Collection Composition all renderLine of triangles.
    public void _drawRenderLine_AllTris_Debug( CTriCollector triCollector )
    {
        if (null == m_drawRenderLine_triCollector)
        {
            return;
        }

        List<CTRI> listTriangle = triCollector.getTris();

        if (listTriangle.Count < 1)
        {
            return;
        }

        if (m_iSequenceRenderLine_forDebug >= 0)
        {
            return;
        }


        int iLengthMapping = listTriangle.Count * 3;
        Vector3[] arrv3VBmapping = new Vector3[iLengthMapping];

        for (int iSeqTri = 0; iSeqTri < listTriangle.Count; ++iSeqTri)
        {
            CTRI triCurr = listTriangle[iSeqTri];
            for (int iPnt = 0; iPnt < 3; ++iPnt)
            {
                int iSeqPnt = iSeqTri * 3 + iPnt;
                Vector3 v3PntCurr = triCurr._arrv3PT[iPnt];
                arrv3VBmapping[iSeqPnt] = v3PntCurr;
            }
        }

        m_iSequenceRenderLine_forDebug = m_drawRenderLine_triCollector.DrawNewLine_user(arrv3VBmapping);

    } // public void _drawRenderLine_AllTris_Debug()


    public void _desetAllTris_RenderLine_Debug()
    {
        if (null == m_drawRenderLine_triCollector)
        {
            return;
        }

        if (m_iSequenceRenderLine_forDebug < 0)
        {
            return;
        }

        m_drawRenderLine_triCollector.DeleteDrawLine(m_iSequenceRenderLine_forDebug);
        m_iSequenceRenderLine_forDebug = -1;
    }

    //@ Construct All Triangles RenderLines 
    public void drawAllTrisOneDrawcall(List<CTRI> listTriangle, Color colorTriangles)
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        if (listTriangle.Count < 1)
        {
            return;
        }

        int iLengthMapping = listTriangle.Count * 3;
        Vector3[] arrv3VBmapping = new Vector3[iLengthMapping];

        for (int iSeqTri = 0; iSeqTri < listTriangle.Count; ++iSeqTri)
        {
            CTRI triCurr = listTriangle[iSeqTri];
            for (int iPnt = 0; iPnt < 3; ++iPnt)
            {
                int iSeqPnt = iSeqTri * 3 + iPnt;
                Vector3 v3PntCurr = triCurr._arrv3PT[iPnt];

                arrv3VBmapping[iSeqPnt] = v3PntCurr;
            }
        }

        m_iSequenceRenderMesh_forDebug = m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERNAVIMESH,
                                                                            arrv3VBmapping,
                                                                            colorTriangles,
                                                                            CMATH.FEPSILON_F3,
                                                                            -1.0f);

    } // public void drawAllTrisOneDrawcall()

    public void undrawAllTrisOneDrawcall()
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        m_drawRenderMesh_triCollector.DeleteDrawMesh(m_iSequenceRenderMesh_forDebug);
        m_iSequenceRenderMesh_forDebug = -1;
    }


    //@ Construct All Triangles RenderLines 
    public void drawRenderTris_selectlist(List<CTRI> listTriangle, List<int> listTrisIdx, Color colorTris)
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        if (listTriangle.Count < 1 || listTrisIdx.Count < 1)
        {
            Debug.Log("drawAllTrisOneDrawcall():listTriangle.Count < 1 || listTrisIdx.Count < 1");
            return;
        }

        int iLengthMapping = listTrisIdx.Count * 3;
        Vector3[] arrv3VBmapping = new Vector3[iLengthMapping];

        for (int iSeqTri = 0; iSeqTri < listTrisIdx.Count; ++iSeqTri)
        {
            int iTriIdxCurr = listTrisIdx[iSeqTri];

            CTRI triCurr = listTriangle[iTriIdxCurr];
            for (int iPnt = 0; iPnt < 3; ++iPnt)
            {
                int iSeqPnt = iSeqTri * 3 + iPnt;
                Vector3 v3PntCurr = triCurr._arrv3PT[iPnt];

                arrv3VBmapping[iSeqPnt] = v3PntCurr;
            }
        } // for (int iSeqTri = 0; iSeqTri < listTrisIdx.Count; ++iSeqTri )

        m_iSequenceRenderMesh_forDebug = m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERNAVI_TRIS_IR,
                                                                                        arrv3VBmapping,
                                                                                        colorTris,
                                                                                        0.001f,
                                                                                        -1.0f);

    } // public void drawRenderTris_selectlist()

    public void undrawRenderTris_selectlist()
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        m_drawRenderMesh_triCollector.DeleteDrawMesh(DFLT_IDX_RENDERNAVI_TRIS_IR);
    }

    //@ Debug Draw for Tri Eachtype
    public void drawAllTri_eachFunctional(CTriCollector triCollector, bool bRepresentBlocks)
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        undrawAllTri();

        List<int> listTrisPicked_naviGoal = triCollector.m_listTris_naviGoal;
        List<int> listTrisPicked_naviBlock = triCollector.m_listTris_naviBlock;
        List<int> listTrisPicked_naviBlockRoad = triCollector.m_listTris_naviBlockRoad;
        List<int> listTrisPicked_naviStart = triCollector.m_listTris_naviStart;

        // Blocks
        if (true == bRepresentBlocks)
        {
            if (listTrisPicked_naviBlock.Count > 0)
            {
                foreach (int iIdxTri in listTrisPicked_naviBlock)
                {
                    CTRI triRenderDebug = triCollector.getTri(iIdxTri);
                    m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERTRI_TYPE + iIdxTri,
                                                    triRenderDebug._arrv3PT,
                                                    Color.Lerp(Color.blue, Color.black, 0.5f),
                                                    CMATH.FEPSILON_F2 * 3.0f,
                                                    CDrawRenderMesh.scaleadjust_render_095);
                }
            }

            // BlockRoads
            if (listTrisPicked_naviBlockRoad.Count > 0)
            {
                foreach (int iIdxTri in listTrisPicked_naviBlockRoad)
                {
                    CTRI triRenderDebug = triCollector.getTri(iIdxTri);
                    m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERTRI_TYPE + iIdxTri,
                                                    triRenderDebug._arrv3PT,
                                                    Color.Lerp(Color.blue, Color.white, 0.2f),
                                                    CMATH.FEPSILON_F2 * 3.0f,
                                                    CDrawRenderMesh.scaleadjust_render_095);
                }
            }
        } // if (true == bRepresentBlocks)

        // Goals                                    
        if (listTrisPicked_naviGoal.Count > 0)
        {
            foreach (int iIdxTri in listTrisPicked_naviGoal)
            {
                CTRI triRenderDebug = triCollector.getTri(iIdxTri);
                m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERTRI_TYPE + iIdxTri,
                                                    triRenderDebug._arrv3PT,
                                                    Color.red,
                                                    CMATH.FEPSILON_F2 * 3.0f,
                                                    CDrawRenderMesh.scaleadjust_render_093);
            }
        }




        // Starts
        if (listTrisPicked_naviStart.Count > 0)
        {
            foreach (int iIdxTri in listTrisPicked_naviStart)
            {
                CTRI triRenderDebug = triCollector.getTri(iIdxTri);
                m_drawRenderMesh_triCollector.DrawNewRendermesh(DFLT_IDX_RENDERTRI_TYPE + iIdxTri,
                                                    triRenderDebug._arrv3PT,
                                                    Color.yellow,
                                                    CMATH.FEPSILON_F2 * 3.0f,
                                                    CDrawRenderMesh.scaleadjust_render_093);
            }
        }
    } // public void drawAllTri_eachFunctional()

    public void undrawAllTri()
    {
        if (null == m_drawRenderMesh_triCollector)
        {
            return;
        }

        m_drawRenderMesh_triCollector.DeleteDrawMesh_All();
    }

    void Awake()
    {

    }
} // public class CToolModuleNavimesh
