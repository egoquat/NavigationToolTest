using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CNavigation {
    //@ To access position and triangle index for improvement of object movement
    public class CCellWayto
    {
        public int _idxnavicell;
        public int _ileveltoGoal;

        public float _ratioPerpen;

        public Vector3 _posStart = Vector3.zero;
        public Vector3 _postoDest = Vector3.zero;

        public Vector3 _posEdgeFrom0 = Vector3.zero;
        public Vector3 _posEdgeFrom1 = Vector3.zero;

        public Vector3 _posEdgeTo0 = Vector3.zero;
        public Vector3 _posEdgeTo1 = Vector3.zero;
        public Vector3 _posCenterCell = Vector3.zero;
        public Vector3 _dirEntrytoNext = Vector3.zero;

        public void setPntnavi(CCellWayto otherpnt)
        {
            _idxnavicell = otherpnt._idxnavicell;
            _ileveltoGoal = otherpnt._ileveltoGoal;
            _posStart = otherpnt._posStart;
            _postoDest = otherpnt._postoDest;

            _posEdgeFrom0 = otherpnt._posEdgeFrom0;
            _posEdgeFrom1 = otherpnt._posEdgeFrom1;

            _posEdgeTo0 = otherpnt._posEdgeTo0;
            _posEdgeTo1 = otherpnt._posEdgeTo1;

            _posCenterCell = otherpnt._posCenterCell;
            _dirEntrytoNext = otherpnt._dirEntrytoNext;
        }

        public void setPntnavi(int idxnavicell,
                                int ileveltoGoal,
                                Vector3 postostart,
                                Vector3 postoDest,
                                Vector3 posEdgeFrom0,
                                Vector3 posEdgeFrom1,
                                Vector3 posEdgeTo0,
                                Vector3 posEdgeTo1,
                                Vector3 posCenterCell,
                                Vector3 direntrytogoal)
        {
            _idxnavicell = idxnavicell;
            _ileveltoGoal = ileveltoGoal;
            _posStart = postostart;
            _postoDest = postoDest;

            _posEdgeFrom0 = posEdgeFrom0;
            _posEdgeFrom1 = posEdgeFrom1;

            _posEdgeTo0 = posEdgeTo0;
            _posEdgeTo1 = posEdgeTo1;

            _posCenterCell = posCenterCell;
            _dirEntrytoNext = direntrytogoal;
        }

        public bool crossoverTest(Vector3 posTestStart, Vector3 posTestEnd)
        {
            bool bcrossOver = CMATH.crossOverDest(_posCenterCell, _postoDest, posTestEnd);
            return bcrossOver;
        }

        public bool IntersectedPosToEdge(Vector3 posTestStart, Vector3 posTestEnd, ref Vector3 posIntersected)
        {
            Vector3 posP0 = posTestStart;
            Vector3 posP1 = posTestEnd;
            Vector3 posP2 = new Vector3(posTestStart.x, posTestStart.y + 0.2f, posTestStart.z);

            Vector3[] arrPosLine = new Vector3[2];
            arrPosLine[0] = new Vector3(_posEdgeTo0.x, _posEdgeTo0.y, _posEdgeTo0.z);
            arrPosLine[1] = new Vector3(_posEdgeTo1.x, _posEdgeTo1.y, _posEdgeTo1.z);

            //CMATH.rescaleVertices(ref arrPosLine, 1.0f);
            bool bIntersected = CMATH.findposIntersected(arrPosLine[0], arrPosLine[1], posP0, posP1, posP2, false, ref posIntersected);
            _ratioPerpen = (posIntersected - _posEdgeTo0).magnitude / (_posEdgeTo1 - _posEdgeTo0).magnitude;
            return bIntersected;
        }

        public CCellWayto(CCellWayto otherpnt)
        {
            setPntnavi(otherpnt);
        }

        public CCellWayto()
        {

        }
    }

    #region navigationmodule

    #region constparameter
    public static readonly int NULL_DEPTHLEVEL = -1;
    public static readonly float DFLT_EDGECROSS_ESTIMATE = CMATH.FEPSILON_F2;
    public static readonly float DFLT_LIMIT_DIFFER_CHANGERATE_XY = CMATH.FEPSILON_F2;
    public static readonly float DFLT_ADJUST_SCALE_FOR_VISIBLECHECK = 0.99f;
    #endregion // #region constparameter

    //@ Properties
    protected bool m_bDidBuildGoal = false;                   // Did manager mapped for navigation or not.
    protected int m_iDepthLevel_Navi = NULL_DEPTHLEVEL;       // Farthest depth Level From GOAL.
    protected int m_iLevelDepth_Navi_Standard = NULL_DEPTHLEVEL;

    public CNAVICELL[] m_arrNavicells;                          //네비 셀들의 배열

    //@ Manager Edge
    [System.NonSerialized]
    public CComputeEdgeBlock m_computeEdgeBlock = new CComputeEdgeBlock(); // 가시성 고려 최단 거리 이동 모듈

    //@Get/Set
    //@ -Return:Farthest depth Level From GOAL.
    public int getLevelCells() { return m_iDepthLevel_Navi; }

    //@ -Return:Did mapped all navigation or not.
    public bool didBuildupGoal() { return m_bDidBuildGoal; }

    public int getCountCells() { return m_arrNavicells.Length; }
    public bool getCell(uint iIdxCell, ref CNAVICELL naviCell)
    {
        if (iIdxCell > m_arrNavicells.Length)
        {
            return false;
        }

        naviCell = m_arrNavicells[iIdxCell];
        return true;
    }

    public CNAVICELL getCell(int iIdxCell)
    {
        return m_arrNavicells[iIdxCell];
    }

    public CNAVICELL[] getNaviCellsAll()
    {
        return m_arrNavicells;
    }

    public bool isGoalCell(int iSeqCell)
    {
        return m_arrNavicells[iSeqCell].IsGoalCell();
    }

    //@ -Return:pre-calculated shortest cost middle edge position (way to goal) (when mappingnavigation).
    //  Every navicell have just only one shortest way to GOAL.
    public CNAVICELL.E_CELL_STATUS getPosCurrentWayToGoal(int iSeqCell_current, ref CCellWayto pntNavi_out)
    {
        if (iSeqCell_current > (int)m_arrNavicells.Length || iSeqCell_current < 0)
        {
            Debug.Log("!!!ERROR iSeqCell_current>(int)m_arrNavicells.size()||iSeqCell_current<0");

            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;
        }

        CNAVICELL naviCell = m_arrNavicells[iSeqCell_current];
        Vector3 v3PosDest = Vector3.zero;

        bool bGetPos_nextWay = naviCell.getPos_nextWay(ref v3PosDest);

        if (false == bGetPos_nextWay || Vector3.zero == v3PosDest)
        {
            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;
        }

        Vector3 posEdge0 = new Vector3(), posEdge1 = new Vector3();
        naviCell.getPntsOfEdgeBestway(ref posEdge0, ref posEdge1);

        pntNavi_out.setPntnavi(naviCell.seqCell,
                                naviCell.leveltogoal,
                                Vector3.zero,
                                v3PosDest,
                                posEdge0,
                                posEdge1,
                                posEdge0, 
                                posEdge1,
                                naviCell.getPos_cellCenter(),
                                naviCell.dirCenterToGoal);

        return CNAVICELL.E_CELL_STATUS.CELL_STATUS_ROAD;
    } // public bool getPosWayToGoal(int iSeqCell_current, ref CCellWayto pntNavi_out)

    //@ -Return:pre-calculated shortest cost middle edge position (way to goal) (when mappingnavigation).
    //  Every navicell have just only one shortest way to GOAL.
    public CNAVICELL.E_CELL_STATUS getPosNextWayTo(int iSeqCell_current, ref CCellWayto pntNavi_out)
    {
        if (iSeqCell_current > (int)m_arrNavicells.Length || iSeqCell_current < 0)
        {
            Debug.Log("!!!ERROR iSeqCell_current>(int)m_arrNavicells.size()||iSeqCell_current<0");

            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;
        }

        CNAVICELL naviCellcurr = m_arrNavicells[iSeqCell_current];
        Vector3 v3PosDest = new Vector3();
        Vector3 v3PosStart = new Vector3();

        naviCellcurr.getPos_nextWay(ref v3PosStart);

        int iIdxCellCurrent = naviCellcurr.seqCell;
        int iIdxCellNextTo = naviCellcurr.getIdxCell_nextWay();
        if (CNAVICELL.NULL_CELL == iIdxCellNextTo)
        {
            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;
        }

        CNAVICELL naviCellNext = m_arrNavicells[iIdxCellNextTo];
        if (naviCellNext.IsGoalCell())
        {
            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL;
        }

        bool bGetPos_nextWay = naviCellNext.getPos_nextWay(ref v3PosDest);

        if (false == bGetPos_nextWay)
        {
            return CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;
        }

        Vector3 posEdgeTo0 = new Vector3(), posEdgeTo1 = new Vector3(), posEdgeFrom0 = new Vector3(), posEdgeFrom1 = new Vector3();
        naviCellcurr.getPntsOfEdgeBestway(ref posEdgeFrom0, ref posEdgeFrom1);
        naviCellNext.getPntsOfEdgeBestway(ref posEdgeTo0, ref posEdgeTo1);
        Vector3 v3dirEntryToNextway = naviCellNext.getDirEntryToGoal(iIdxCellCurrent);
        pntNavi_out.setPntnavi(iIdxCellNextTo,
                                naviCellcurr.leveltogoal,
                                v3PosStart,
                                v3PosDest,
                                posEdgeFrom0,
                                posEdgeFrom1,
                                posEdgeTo0, 
                                posEdgeTo1,
                                naviCellNext.getPos_cellCenter(),
                                v3dirEntryToNextway);

        return CNAVICELL.E_CELL_STATUS.CELL_STATUS_ROAD;
    } // public bool getPosWayToGoal(int iSeqCell_current, ref CCellWayto pntNavi_out)

    //@ -Return:
    public bool getPosWayTo_Portal(int iSeqCell_current, Vector2[] arrv2PntObjBoundary, ref CCellWayto pntNavi_out)
    {
        if (iSeqCell_current > (int)m_arrNavicells.Length || iSeqCell_current < 0)
        {
            Debug.Log("iSeqCell_current>(int)m_arrNavicells.size()||iSeqCell_current<0");
            return false;
        }

        CNAVICELL naviCellCurr = m_arrNavicells[iSeqCell_current];

        //@ 가시성을 고려한 최단 거리의 위치값 설정.
        Vector3 v3PosDest = new Vector3();
        bool bGetPos_nextWay = setNaviPortal_forShortestWay(arrv2PntObjBoundary, ref naviCellCurr);

        if (false == bGetPos_nextWay)
        {
            return false;
        }

        pntNavi_out._idxnavicell = naviCellCurr.seqCell;
        pntNavi_out._postoDest = v3PosDest;

        return true;
    }

    //  Every navicell have just only one shortest way to GOAL.
    public bool getPosWayToGoal(int iSeqCell_current, ref Vector3 v3PosDest)
    {
        if (iSeqCell_current > (int)m_arrNavicells.Length || iSeqCell_current < 0)
        {
            Debug.Log("iSeqCell_current>(int)m_arrNavicells.size()||iSeqCell_current<0");
            return false;
        }

        CNAVICELL naviCell = m_arrNavicells[iSeqCell_current];
        if (naviCell.IsBlockCell())
        {
            return false;
        }

        return naviCell.getPos_nextWay(ref v3PosDest);
    }

    //@ -Return:
    public bool getPosWayTo_Portal(int iSeqCell_current, Vector2[] arrv2PntObjBoundary, ref Vector3 v3PosDest)
    {
        if (iSeqCell_current > (int)m_arrNavicells.Length || iSeqCell_current < 0)
        {
            Debug.Log("iSeqCell_current>(int)m_arrNavicells.size()||iSeqCell_current<0");
            return false;
        }

        CNAVICELL naviCellCurr = m_arrNavicells[iSeqCell_current];
        if (naviCellCurr.IsBlockCell())
        {
            return false;
        }

        //@ 가시성을 고려한 최단 거리의 위치값 설정.
        setNaviPortal_forShortestWay(arrv2PntObjBoundary, ref naviCellCurr);

        return naviCellCurr.getPos_nextWay(ref v3PosDest);
    } // public bool getPosWayTo_Portal(	int iSeqCell_current, ref Vector3 v3PosDest )

    //@ Get adjacent Index Triangles validated.
    public void getAdjacentTris(int iSeqCell, ref List<int> listTrisAdj)
    {
        if (iSeqCell > m_arrNavicells.Length)
        {
            return;
        }

        CNAVICELL naviCellCurr = m_arrNavicells[iSeqCell];

        for (int iAdj = 0; iAdj < naviCellCurr.m_arriAdjCells.Length; ++iAdj)
        {
            int iAdjTri = naviCellCurr.m_arriAdjCells[iAdj];

            if (iAdjTri != CNAVICELL.NULL_CELL)
            {
                listTrisAdj.Add(iAdjTri);
            }
        }
    }

    // Collect all blocks start to goal. And then buildup shortest way start to goal.
    // 처음 스타트 부터 골까지의 모든 네비 블럭을 모은다(가장 짧은 길을 택한다)
    static public void collectWayToGoalLinear(CNAVICELL[] arrNavicells_,
                                                CNAVICELL cellStart_,
                                                ref List<CNAVICELL> listSequenceCell_,
                                                ref CComputeEdgeBlock processorEdge_)
    {
        bool bOrganizedComplete = false;
        Vector3 v3PntEdge0 = new Vector3();
        Vector3 v3PntEdge1 = new Vector3();

        CNAVICELL cellWay = cellStart_;

        bOrganizedComplete = false;

        listSequenceCell_.Add(cellWay); //First unit is startcell.

        //@ first collect all blocks start -> goal. 
        while (false == bOrganizedComplete)
        {
            CTRI triWaytoGoal = cellWay.trilinkcell;

            //@ if Adjacent cell is NULL, add new EDGE list.    //이웃셀이 없다면 추가한다
            for (int iSeqAdj = 0; iSeqAdj < cellWay.m_arriAdjCells.Length; ++iSeqAdj)
            {
                int iIdxAdj = cellWay.m_arriAdjCells[iSeqAdj];
                CTRI.E_TRI_EDGE eEdge = (CTRI.E_TRI_EDGE)iSeqAdj;

                //@ Checkup adjacent Edge Tri    //
                if (CNAVICELL.NULL_CELL == iIdxAdj)
                {
                    triWaytoGoal.GetEdgePoint(eEdge, ref v3PntEdge0, ref v3PntEdge1);
                    processorEdge_.AddToBlockEdge(v3PntEdge0, v3PntEdge1);       //-> Add Edge to Block List
                }
                else
                {
                    CNAVICELL cellAdj = arrNavicells_[iIdxAdj];

                    //@ Adj cell is block cell, then into list.
                    if (cellAdj.IsBlockCell())
                    {
                        processorEdge_.AddToBlockEdge_Tri(cellAdj.trilinkcell, iIdxAdj); //-> Add Edge to Block List
                    } 
                } 
            } // for (int iAdj = 0; iAdj < naviCellNextWay.m_arriAdjCells.Length; ++iAdj)

            int iIdxCellBestWay = cellWay.getIdxCell_nextWay();

            if (CNAVICELL.NULL_CELL == iIdxCellBestWay)
            {
                return;
            }

            cellWay = arrNavicells_[iIdxCellBestWay];

            listSequenceCell_.Add(cellWay); // Last unit must be goalcell.

            if (true == cellWay.IsGoalCell())
            {
                bOrganizedComplete = true;
            }
        } // while (false == bOrganizedComplete)
    } // protected void collectWayToGoalLinear()

    //protected float differChangeRateBetweenVec3(Vector3 v3Way0, Vector3 v3Way1, Vector3 v3Way2)
    protected float differChangeRateBetweenVec3(Vector3 v3Edge01, Vector3 v3Edge02)
    {
        Vector2 v2Edge01 = CMATH.ConvertToV2_Y(v3Edge01);
        Vector2 v2Edge02 = CMATH.ConvertToV2_Y(v3Edge02);

        Vector2 v2Change01 = new Vector2(v2Edge01.sqrMagnitude, v3Edge01.y * v3Edge01.y);
        Vector2 v2Change02 = new Vector2(v2Edge02.sqrMagnitude, v3Edge02.y * v3Edge02.y);

        float fChangeRate01 = v2Change01.x / v2Change01.y;
        float fChangeRate02 = v2Change02.x / v2Change02.y;

        return Mathf.Abs(fChangeRate02 - fChangeRate01);
    } // protected float differChangeRateBetweenVec3(Vector3 v3Way0, Vector3 v3Way1, Vector3 v3Way2)


    //@ search intersected point // faster then 3.x~4.x times average, prepare RayIntersect.
    public int intersectRay_inAdjs(int iSeqCell, Vector3 v3RayOrigin, Vector3 v3RayDir, ref Vector3 posIntersected_out)
    {
        if (CNAVICELL.NULL_CELL == iSeqCell)
        {
            return CNAVICELL.NULL_CELL;
        }

        CNAVICELL navicell = m_arrNavicells[iSeqCell];

        int iIdxIntersectedCell = CNAVICELL.NULL_CELL;
        bool bIntersectedCell = false;

        bIntersectedCell = CMATH.IntersectRayTri_GetPos(
                                                    v3RayOrigin,
                                                    v3RayDir,
                                                    navicell.m_arrv3PT,
                                                    ref posIntersected_out);
        if (true == bIntersectedCell)
        {
            iIdxIntersectedCell = iSeqCell;
        }
        else
        {
            foreach (int iIterCell in navicell.m_arriAdjCells)
            {
                if (CNAVICELL.NULL_CELL != iIterCell)
                {
                    bIntersectedCell = m_arrNavicells[iIterCell].trilinkcell.IntersectRay(v3RayOrigin, v3RayDir, ref posIntersected_out);
                    if (true == bIntersectedCell)
                    {
                        iIdxIntersectedCell = iIterCell;

                        break;
                    }

                } // if (CNAVICELL.NULL_CELL != iSeqCell)
            } // foreach(int iSeqCellNavi in navicell.m_arriAdjCells)
        } // if (true == bIntersectedCell)

        return iIdxIntersectedCell;
    } // public bool intersectRay_inAdjs(int iSeqCell, Vector3 v3RayOrigin, Vector3 v3RayDir, ref Vector3 posIntersected_out)

    //@ Organize shortest way block edges only Current cell
    public bool setNaviPortal_forShortestWay(Vector2[] arrv2PntObjBoundary, ref CNAVICELL cellWayStart)
    {
        if (true == cellWayStart.useWaytoPortal)
        {
            return true;
        }

        List<CNAVICELL> listSequenceCell = new List<CNAVICELL>();

        collectWayToGoalLinear(m_arrNavicells, cellWayStart, ref listSequenceCell, ref m_computeEdgeBlock);
        if (listSequenceCell.Count < 2)
        {
            return true;
        }


        Vector3 v3PntPrev = new Vector3();
        Vector3 v3PntCurr = new Vector3();
        Vector3 v3PntNext = new Vector3();

        int iLenSeq = listSequenceCell.Count;
        int iSeqIteration = 0;
        CNAVICELL cellGOAL = listSequenceCell[iLenSeq - 1];
        CNAVICELL cellNext, cellPrev;

        bool bCrosswithBlock = false;
        bool bSetNewPortalPosition = false;

        Vector3[] arrv3Pnt_Cell_start = new Vector3[cellWayStart.m_arrv3PT.Length];
        Array.Copy(cellWayStart.m_arrv3PT, arrv3Pnt_Cell_start, cellWayStart.m_arrv3PT.Length);
        CMATH.rescaleVertices(ref arrv3Pnt_Cell_start, DFLT_ADJUST_SCALE_FOR_VISIBLECHECK);

        foreach (CNAVICELL cellCurr in listSequenceCell)
        {
            int iSeqCurr = iSeqIteration++;

            if (0 == iSeqCurr)
            {
                continue;
            }

            if (cellGOAL == cellCurr)
            {
                cellWayStart.SetPortalPosition(cellGOAL.seqCell, cellGOAL.getPos_cellCenter());
                bSetNewPortalPosition = true;
                break;
            } 

            cellPrev = listSequenceCell[iSeqCurr - 1];
            cellNext = listSequenceCell[iSeqCurr + 1];

            cellCurr.getPos_BestWay_onEdge(ref v3PntCurr);
            cellNext.getPos_BestWay_onEdge(ref v3PntNext);
            cellPrev.getPos_BestWay_onEdge(ref v3PntPrev);

            bCrosswithBlock = false;
            foreach (Vector3 v3PntCell in arrv3Pnt_Cell_start)
            {
                if (m_computeEdgeBlock.crossBlockIteration(v3PntCell, v3PntNext))
                {
                    bCrosswithBlock = true;
                    break;
                }

            }

            //@ Set Portal
            if (true == bCrosswithBlock)
            {
                cellWayStart.SetPortalPosition(cellCurr.seqCell, v3PntCurr);

                bSetNewPortalPosition = true;
                break;
            }

            Vector2 v2PntNext = CMATH.ConvertToV2_Y(v3PntNext);
            foreach (Vector2 v2PntObj in arrv2PntObjBoundary)
            {
                //@ Cross At least 1 mo
                if (true == m_computeEdgeBlock.crossBlockIteration(v2PntObj, v2PntNext))
                {
                    bCrosswithBlock = true;
                    break;
                }
            }

            //@ Set Portal -2
            if (true == bCrosswithBlock)
            {
                cellWayStart.SetPortalPosition(cellCurr.seqCell, v3PntCurr);

                bSetNewPortalPosition = true;
                break;
            }

        } // foreach (CNAVICELL cellCurr in listSequenceCell)

        listSequenceCell.Clear();
        listSequenceCell = null;

        return bSetNewPortalPosition;
    } // public bool setNaviPortal_forShortestWay(int iSeqCell)

    // Collect all blocks requestcell to goal. And then buildup shortest way start to goal.
    // 요청셀 부터 골까지의 길 셀만을 모은다(가장 짧은 길을 택한다)
    public void collectWayToGoalLinear(int iCellStart,
                                       ref List<CNAVICELL> listSequenceCell_)
    {
        if (m_arrNavicells.Length <= iCellStart || 0 > iCellStart)
        {
            return;
        }

        bool bCollectComplete = false;
        CNAVICELL cellWay = m_arrNavicells[iCellStart];

        bCollectComplete = false;
        listSequenceCell_.Add(cellWay);     // collect start cell first.

        //@ first collect all blocks start -> goal. 
        while (false == bCollectComplete)
        {
            int iIdxCellBestWay = cellWay.getIdxCell_nextWay();

            if (CNAVICELL.NULL_CELL == iIdxCellBestWay)
            {
                return;
            }

            cellWay = m_arrNavicells[iIdxCellBestWay];

            listSequenceCell_.Add(cellWay); // Last unit must be goalcell.

            if (true == cellWay.IsGoalCell())
            {
                bCollectComplete = true;
            }
        } // while (false == bCollectComplete)
    } // public void collectWayToGoalLinear

    //@ Organize shortest way block edges startcell to goalcell.   
    public bool collectWay_allStartsToGoal(CTriCollector tricollector, ref CNAVICELL[] arrSequenceCellforPortal)
    {
        if (tricollector.m_listTris_naviStart.Count < 1)
        {
            return false;
        }

        if (tricollector.m_listTris_naviGoal.Count < 1)
        {
            return false;
        }

        arrSequenceCellforPortal = null;

        Vector3 v3PntStart_ = new Vector3();
        Vector3 v3PntEnd_ = new Vector3();

        Vector3 v3PntPortal = new Vector3();

        List<CNAVICELL> listSequenceCell = new List<CNAVICELL>();

        //@ each start cell
        foreach (int iSeqTriStart in tricollector.m_listTris_naviStart)
        {
            CNAVICELL cellWayStart = m_arrNavicells[iSeqTriStart];

            //@ First Collect all blocks start to goal. And then buildup shortest way start to goal.
            collectWayToGoalLinear(m_arrNavicells, cellWayStart, ref listSequenceCell, ref m_computeEdgeBlock);

            //@ Organize Start -> Goal
            if (listSequenceCell.Count > 2)
            {
                int iLenSeq = listSequenceCell.Count;
                int iSeqIteration = 0, iSeqPortalSpot = 0;
                v3PntStart_ = listSequenceCell[0].poscenterofcell;
                CNAVICELL cellGOAL = listSequenceCell[iLenSeq - 1];
                bool bCrosswithBlock = false;
                foreach (CNAVICELL cellCurr in listSequenceCell)
                {
                    int iSeqCurr = iSeqIteration++;

                    if (cellCurr == cellGOAL)
                    {
                        break;
                    }

                    cellCurr.getPos_nextWay(ref v3PntEnd_);

                    bCrosswithBlock = m_computeEdgeBlock.crossBlockIteration(v3PntStart_, v3PntEnd_);
                    if (true == bCrosswithBlock)
                    {
                        iSeqPortalSpot = iSeqCurr - 1;

                        if (listSequenceCell.Count <= iSeqPortalSpot || iSeqPortalSpot < 0)
                        {
                            continue;
                        }

                        listSequenceCell[iSeqPortalSpot].getPos_nextWay(ref v3PntPortal);

                        ////@ Some problem this, have to solve.
                        //for (int iCellSeq = iSeqPortalSpot__ + 1; iCellSeq < iSeqPortalSpot; ++iCellSeq)
                        //{
                        //    listSequenceCell[iCellSeq].SetPortalPosition(iSeqPortalSpot__, v3PntPortal);
                        //} //for (int iCellSeq = iSeqPortalSpot; iCellSeq < iSeqCurr - 1; ++iCellSeq)

                        v3PntStart_ = v3PntPortal;
                    } // if (true == bCrosswithBlock)

                } // foreach (CNAVICELL cellCurr in listSequenceCell)

                int iSeqCopyTo = 0;
                if (null == arrSequenceCellforPortal)
                {
                    arrSequenceCellforPortal = new CNAVICELL[listSequenceCell.Count];
                }
                else
                {
                    iSeqCopyTo = arrSequenceCellforPortal.Length;
                    Array.Resize(ref arrSequenceCellforPortal, arrSequenceCellforPortal.Length + listSequenceCell.Count);
                } // if (null == arrSequenceCellforPortal)

                listSequenceCell.CopyTo(arrSequenceCellforPortal, iSeqCopyTo);
                listSequenceCell.Clear();

            } // if(listSequenceCell.Count > 2)
        } // foreach(int iSeqTriStart in m_listCellsIdxStart)

        listSequenceCell.Clear();
        listSequenceCell = null;

        return true;
    } // protected bool collectWay_allStartsToGoal()

    //@ Process
    //@ Construct All Navigation Cells from Collected Triangles(listTris_). 
    //  Main process of Construct is linked all adjacent triangles iteration every triangles.
    public virtual void InitializeNavi()
    {

    }

    public virtual void ConstructNavi(List<CTRI> listTris_)
    {
        ConstructTrisToCells(listTris_);
        m_bDidBuildGoal = false;

        m_computeEdgeBlock.InitEdgeBlock();
    }

    //@ Destruct All Array NaviCells
    public virtual void DestructNavi()
    {
        if (false == m_bDidBuildGoal)
        {
            return;
        }

        if (null != m_arrNavicells && m_arrNavicells.Length > 0)
        {
            setClearAllPortals();

            Array.Clear(m_arrNavicells, 0, m_arrNavicells.Length);
            m_arrNavicells = null;
        }

        m_iDepthLevel_Navi = NULL_DEPTHLEVEL;
        m_bDidBuildGoal = false;
    } // public void DestructNavi()



    //@ Private 
    //@ Build all Cells relationship with adjacent Cells. (N^2) CELL * CELL
    protected void buildupAdjacentAll(ref CNAVICELL[] arrNavicells)
    {
        for (int iSeqCell = 0; iSeqCell < arrNavicells.Length; ++iSeqCell)
        {
            CNAVICELL naviCell = (arrNavicells[iSeqCell]);
            BuildAdjacentATri(ref naviCell, ref arrNavicells);
        }

        //FORTEST
        //validateAllAdjcntCells(arrNavicells);
    }

    //@ Actually build up relationship with adjacent Cells and calculate cost of the way to GOAL.
    //@ Not acceptable 1 or more NULL CELL which status is CNAVICELL.NULL_CELL
    protected bool BuildAdjacentATri(ref CNAVICELL cellLeft,
                                    ref CNAVICELL[] arrNavicells)
    {
        if (true == cellLeft.DidSetAllAdjacent())
        {
            return false;
        }

        int[] aruiIVleft = cellLeft.trilinkcell._arriIV;
        int[] aruiIVRight;
        bool bFoundShared = false;
        int iFoundShared = 0;

        int[] ariPointsShared_left = new int[3];
        int[] ariPointsShared_right = new int[3];

        int iCntCells = arrNavicells.Length;

        for (int iSeqcell = 0; iSeqcell < iCntCells; ++iSeqcell)
        {
            CNAVICELL cellRight = arrNavicells[iSeqcell];
            CTRI triRight_ = cellRight.trilinkcell;

            if (cellRight.seqCell == cellLeft.seqCell)
            {
                continue;
            }

            for (int i = 0; i < 3; ++i)
            {
                ariPointsShared_left[i] = ariPointsShared_right[i] = CNAVICELL.NULL_CELL;
            }

            aruiIVRight = triRight_._arriIV;

            iFoundShared = 0;
            int iSharedIV_left = 0, iSharedIV_right = 0;

            for (int iL = 0; iL < 3; ++iL)
            {
                for (int iR = 0; iR < 3; ++iR)
                {
                    if (aruiIVleft[iL] == aruiIVRight[iR])
                    {
                        ariPointsShared_left[iL] = aruiIVleft[iL];
                        ariPointsShared_right[iR] = aruiIVRight[iR];

                        iSharedIV_left = iSharedIV_left + iL;
                        iSharedIV_right = iSharedIV_right + iR;

                        ++iFoundShared;
                    }
                } // for( int iR=0; iR<3; ++iR )

            } // for( int iL=0; iL<3; ++iL )

            bFoundShared = (iFoundShared > 1);
            if (true == bFoundShared)
            {
                if (iFoundShared > 2)
                {
                    break;
                }

                cellLeft.SetAdjacentCell(CNAVICELL.getTypeEdge(iSharedIV_left), cellRight.seqCell);
                cellRight.SetAdjacentCell(CNAVICELL.getTypeEdge(iSharedIV_right), cellLeft.seqCell);

                if (cellLeft.DidSetAllAdjacent() == true)
                {
                    break;
                }
            } // if( iFoundShared>1 )
        } // for( it_=iterBegin; it_!=iterEnd; ++it_ )

        return true;
    } // protected bool BuildAdjacentATri



    // 함수:ConstructTrisToCells 
    // 네비게이션을 위한 모든 셀들과 인접 셀들을 생성한다
    //@ Construct all adjacent Cells for navigation.
    protected void ConstructTrisToCells(List<CTRI> listTris_)
    {
        int iCntTris = listTris_.Count;

        m_arrNavicells = new CNAVICELL[iCntTris];   // Tri카운트 만큼 네비셀 생성

        for (int iTri = 0; iTri < iCntTris; iTri++)
        {
            CTRI triCurr = listTris_[iTri];

            CNAVICELL naviCell = new CNAVICELL();
            naviCell.InitializeNaviCell();  // 각 셀들의 초기화

            //@ Compute each middle edge to Center
            naviCell.SetComponentCell(iTri, triCurr);   // Tri 기본 정보를 네비셀에 전달 그리고 각각의 미들 엣지,센터를 계산
            m_arrNavicells[iTri] = naviCell;
        }

        buildupAdjacentAll(ref m_arrNavicells); // 인접 셀 빌드업
    }

    public void setClearAllPortals()
    {
        if (null == m_arrNavicells || m_arrNavicells.Length < 1)
        {
            return;
        }

        foreach (CNAVICELL naviCell in m_arrNavicells)
        {
            naviCell.SetPortalClear();
        }
    } 

    //@ Function call every new mapping navigation before. Just clear and marked unprocessed all cells.
    protected void setClearAllcell_unprocessed(ref CNAVICELL[] arrNavicells)
    {
        for (int iSeqCell = 0; iSeqCell < arrNavicells.Length; ++iSeqCell)
        {
            arrNavicells[iSeqCell].setClear_unprocessed();
        }

        //@ Block Edge Clear
        m_computeEdgeBlock.Release_Edge();
    }

    //@ For DEBUG, check all adjacent cells.
    protected void validateAllAdjcntCells(CNAVICELL[] arrNavicells)
    {
        string szOutputDebug;
        uint uiCnt = 0;

        szOutputDebug = "Validate All Adjacent Cell\n";
        szOutputDebug += "============================================================================";
        Debug.Log(szOutputDebug);
        szOutputDebug = "";

        int iCntCells = arrNavicells.Length;
        for (int iSeqCell = 0; iSeqCell < iCntCells; ++iCntCells, ++uiCnt)
        {
            CNAVICELL cellRight = arrNavicells[iSeqCell];

            //szOutputDebug = "\n " + uiCnt + ". TriSeq(" + cellRight.m_iSeq + ")";
            //            szOutputDebug +=  "AdjacentTri :";

            for (int iAdj = 0; iAdj < CNAVICELL.countSideEdge; ++iAdj)
            {
                int iAdjcntTri = cellRight.m_arriAdjCells[iAdj];

                szOutputDebug += "/i=" + iAdjcntTri;
            }

            szOutputDebug += "\n " + uiCnt + ". TriSeq(" + cellRight.seqCell + ")//";
        }
        Debug.Log(szOutputDebug);

        //validateAdjcnt_Iteration( 0, arrNavicells );

        szOutputDebug += "Check Call by NeiborTri";
        szOutputDebug += "============================================================================";
        Debug.Log(szOutputDebug);
        szOutputDebug = "";
    }

    // 함수:Mapping Navigation()
    // Build up cost of the way from the goal iterating all cells. 
    // 1. Calculate cost of the way from the goal which iterating all cells.
    // -return:Updated Cells
    protected void mappingNavigation_(List<int> listCellsGoal, List<int> listCellsBlock, List<int> listCellsBlockroad, ref CNAVICELL[] arrNavicells)
    {
        bool bMappingComplete = false;
        int iLevelFromGoalRefer = 0, iLevelFromGoalIterator = 0;
        List<int> listCellAdjCollected = new List<int>();
        List<int> listCellsIterator_ = new List<int>();

        //@ Set all block cell
        for (int iSeqCell = 0; iSeqCell < listCellsBlock.Count; ++iSeqCell)
        {
            CNAVICELL naviCellCurr = m_arrNavicells[listCellsBlock[iSeqCell]];
            naviCellCurr.SetBlockCell();
        }

        //@ Set all blockroad cell
        for (int iSeqCell = 0; iSeqCell < listCellsBlockroad.Count; ++iSeqCell)
        {
            CNAVICELL naviCellCurr = m_arrNavicells[listCellsBlockroad[iSeqCell]];
            naviCellCurr.SetBlockRoadCell();
        }

        //@ construct top(GOAL) to leap
        for (int iSeqCellGoal = 0; iSeqCellGoal < listCellsGoal.Count; ++iSeqCellGoal)
        {
            int iSeqCell = listCellsGoal[iSeqCellGoal];
            CNAVICELL naviCellCurr = arrNavicells[iSeqCell];
            naviCellCurr.setLinkcomputeNaviCell_GOAL(0, (int)(CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL), 0.0f);
            listCellsIterator_.Add(iSeqCell);
        }

        //@ Simulate validation or not.
        do	// while( false==bMappingComplete )
        {
            foreach (int iSeqCellParent_ in listCellsIterator_)
            {
                CNAVICELL naviCellParent = arrNavicells[iSeqCellParent_];
                naviCellParent.setLinkAllAdjs(ref m_arrNavicells, ref listCellAdjCollected);
            }

            if (listCellAdjCollected.Count < 1)
            {
                bMappingComplete = true;
                break;
            }

            iLevelFromGoalRefer = ++iLevelFromGoalIterator;

            listCellsIterator_.Clear();
            listCellAdjCollected.Sort();

            foreach (int iSeqCellADJ in listCellAdjCollected)
            {
                listCellsIterator_.Add(iSeqCellADJ);
            }
            listCellAdjCollected.Clear();

        } while (false == bMappingComplete);

        m_iDepthLevel_Navi = iLevelFromGoalRefer;
    } // protected void mappingNavigation_(List<int> listCellsGoal, List<int> listCellsBlock, ref CNAVICELL[] arrNavicells)

    public void setLevelOfDepth_Navimesh(List<int> listCellsGoal, List<int> listCellsBlock, ref CNAVICELL[] arrNavicells)
    {
        m_iLevelDepth_Navi_Standard = simulateMappingNavigation_(listCellsGoal, listCellsBlock, ref m_arrNavicells);
    }

    protected int simulateMappingNavigation_(List<int> listCellsGoal, List<int> listCellsBlock, ref CNAVICELL[] arrNavicells)
    {
        int iLevelFromGoalRefer = -1, iIteratorLevel = 0;
        List<int> listCellsCurrentLevel = new List<int>();

        for (int iSeqCell = 0; iSeqCell < listCellsBlock.Count; ++iSeqCell)
        {
            CNAVICELL naviCellCurr = arrNavicells[listCellsBlock[iSeqCell]];
            naviCellCurr.SetBlockCell();
        }

        //@ Set all Goal, block cell
        for (int iSeqCellGoal = 0; iSeqCellGoal < listCellsGoal.Count; ++iSeqCellGoal)
        {
            int iSeqCell = listCellsGoal[iSeqCellGoal];
            CNAVICELL naviCellCurr = arrNavicells[iSeqCell];
            naviCellCurr.setLinkcomputeSimulate(0, (int)(CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL));
            listCellsCurrentLevel.Add(iSeqCell);
        }
        listCellsCurrentLevel.Sort();

        List<int> listCellsNextLevel = new List<int>();
        bool bMappingComplete = false;

        while (false == bMappingComplete)
        {
            foreach (int iSeqCell in listCellsCurrentLevel)
            {
                CNAVICELL naviCellParent = arrNavicells[iSeqCell];
                naviCellParent.setLinkAllAdjs_simulate(iSeqCell,
                                                        iLevelFromGoalRefer,
                                                        ref m_arrNavicells,
                                                        ref listCellsCurrentLevel,
                                                        ref listCellsNextLevel);
            }

            if (listCellsNextLevel.Count > 0)
            {
                iLevelFromGoalRefer = iIteratorLevel++;
                listCellsNextLevel.Sort();

                listCellsCurrentLevel.Clear();

                foreach (int iSeqCellAdjacentNextlevel in listCellsNextLevel)
                {
                    listCellsCurrentLevel.Add(iSeqCellAdjacentNextlevel);
                }
                listCellsNextLevel.Clear();
            }
            else
            {
                bMappingComplete = true;
                break;
            }
        } // while (false == bMappingComplete)

        return iLevelFromGoalRefer;
    } // protected bool mappingNavigation_Simulate_(List<int> listCellsGoal, List<int> listCellsBlock, ref CNAVICELL[] arrNavicells)

    //@ Construct All navigation cell's cost of GOAL
    // 1. Set clear all.
    // 2. mapping all Goal cells.
    // 3. mapping all Block cells.
    // 4. Calculate all cells cost from GOAL cells.
    public bool mappingNavigation(List<int> listCellsGoal, List<int> listCellsBlock, List<int> listCellsBlockRoad, List<int> listCellsStart, bool bRequestSetStandardLevel)
    {
        if (listCellsGoal.Count < 1)
        {
            return false;
        }

        setClearAllcell_unprocessed(ref m_arrNavicells);

        if (true == bRequestSetStandardLevel)
        {
            //For PROFILING
            //CFORDEBUG.profilerStart();
            //setLevelOfDepth_Navimesh(listCellsGoal, listCellsBlock, ref m_arrNavicells);
            //float fProcessTime = CFORDEBUG.profilerEnd();
            //Debug.Log("Time Profiling(Simulate mapping) = " + fProcessTime);
            //For PROFILING

            //For PROFILING
            //UnityEngine.Profiler.logFile = "aa.txt";
            //UnityEngine.Profiler.enabled = true;
            //UnityEngine.Profiler.BeginSample("aa");
            //setLevelOfDepth_Navimesh(listCellsGoal, listCellsBlock, ref m_arrNavicells);
            //UnityEngine.Profiler.EndSample();
            //setClearAllcell_unprocessed(ref m_arrNavicells);
        }

        ////For PROFILING mappingNavigation
        //CFORDEBUG.profilerStart();

        //@ Mapping Navigation all relationships each cells.
        mappingNavigation_(listCellsGoal, listCellsBlock, listCellsBlockRoad, ref m_arrNavicells);

        //float fProcessTimeMapping = CFORDEBUG.profilerEnd();
        //Debug.Log("Time Profiling(mappingNavigation) = " + fProcessTimeMapping);
        ////For PROFILING

        m_bDidBuildGoal = true;
        return true;
    } // public bool mappingNavigation( List<int> listCellsGoal, List<int> listCellsBlock, List<int> listCellsStart )
    #endregion // navigationmodule
}
