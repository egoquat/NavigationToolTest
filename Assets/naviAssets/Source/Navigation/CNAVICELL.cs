using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ NAVICell has cost the way to GOAL and DEPTH LEVEL, and actual position infomation the way to shortest from GOAL.
public class CNAVICELL
{
    //@ STATUS CELL NAVIGATION for Object movement
    public enum E_CELL_STATUS
    {
        CELL_STATUS_NULL = -1,
        CELL_STATUS_ROAD,
        CELL_STATUS_GOAL,
        CELL_STATUS_BLOCK,
        CELL_STATUS_BLOCKROAD,
        CELL_STATUS_CNT,
    };

    public class CADJ_TRI
    {
        public CTRI tri = null;
        public int[] _arrTrisAdjacent = new int[3];
    }

    //@ CELL(==TRIANGLE)'s EDGE 0-1, 1-2, 2-0   //엣지
    enum E_CELL_SIDEEDGE
    {
        E_EDGE_NULL = -1,
        E_EDGE_01 = 0,
        E_EDGE_12,
        E_EDGE_20,
        E_EDGE_CNT,
    };

    //@ CELL(==TRIANGLE)'s POINT 0, 1, 2
    enum E_CELL_VERTEX
    {
        E_VERTEX_0 = 0,
        E_VERTEX_1,
        E_VERTEX_2,
        E_VERTEX_CNT,
    };

    //@ STATUS BLOCKTYPE 
    enum E_CELL_BLOCKROADTYPE
    {
        BLOCKTYPE_NULL = -1,
        BLOCKROADTYPE_LV1 = 100,
        BLOCKROADTYPE_LV2 = 200,
        BLOCKROADTYPE_LV4 = 400,
        BLOCKROADTYPE_LV8 = 800,
        BLOCKROADTYPE_LVE = 10000,
        BLOCKTYPE_CNT,
    };

    #region constparameter
    static readonly int LEVELNAVI_SIMILAR = 10;
    static readonly int MAX_LEVELFROMGOAL = 1 << 14; // 16384
    static readonly int NULL_LEVELFROMGOAL = MAX_LEVELFROMGOAL;

    static readonly int DFLT_LV_BLOCKROADTYPE = (int)E_CELL_BLOCKROADTYPE.BLOCKROADTYPE_LV8;
    static readonly int DFLT_LV_BLOCKTYPE = (int)E_CELL_BLOCKROADTYPE.BLOCKROADTYPE_LVE;

    public static readonly int NULL_CELL = -1;              // All of cell's no-allocated.
    #endregion // #region constparameter

    public Vector3[] m_arrv3PT = new Vector3[(int)E_CELL_VERTEX.E_VERTEX_CNT];           // 셀의 각 정점 위치점

    //@ Internal Values
	Vector3			        m_posCenterCell;                                                    // 셀의 센터 위치 값
	Vector3[]		        m_arrv3MddlEdge = new Vector3[(int)E_CELL_SIDEEDGE.E_EDGE_CNT];     // 셀의 각 EDGE 중심 값 (실제 이동 경로 정정으로 사용)
    Vector3[]               m_arrv3Perpendicular = new Vector3[(int)E_CELL_SIDEEDGE.E_EDGE_CNT]; // 셀 센터 위치로부터 수직으로 잇는 엣지 수직 점.
	float[]			        m_arrfDistEdgtoCntr = new float[(int)E_CELL_SIDEEDGE.E_EDGE_CNT];   // 각 EDGE중심부터 센터까지 거리

	//@ Relation Values
	int				        m_iSeqCell;                                                         // 접근 고유 인덱스
    CTRI                    m_tri_linker;						                                // For adjacent
    E_CELL_STATUS           m_eStatusCell;                                                      // 셀의 종류 (블럭,골,시작)
    int                     m_iLevelWeight_BlockRoad = DFLT_LV_BLOCKROADTYPE;                   // BLOCKROAD CELL의 가중치 LEVEL

    E_CELL_SIDEEDGE         m_eEdge_bestway;                                                  // 골셀로 가는 엣지 번호(엣지:01, 12, 20)
	int				        m_iLevelToGoal;                                                   // 골셀까지 가는데 드는 레벨
	float			        m_fcosttoGoal_ex;					                                // 골셀까지 가는데 드는 거리 
	float			        m_fcosttoGoal_cen;					                                // 골점 부터 중심까지 거리
	float[]                 m_arrfcosttoGoal_edge = new float[(int)E_CELL_SIDEEDGE.E_EDGE_CNT];	// 골점 부터 각 거리 EDGE별.
    int                     m_iLevelBlockroadToGoal = 0;                                        // 골점으로 가는 단계 셀중 BlockRoad갯수

	public int[]            m_arriAdjCells = new int[(int)E_CELL_SIDEEDGE.E_EDGE_CNT];          // 인접 EDGE별 참조하는 셀인덱스
    
    //@ For Collision
    Vector3                 m_dirCenterToBest;                                                  // Triangle 중점부터 골 에지 중점을 향하는 
    Vector3[]               m_arrDirEntryToBest = new Vector3[3];                               // 진입 에지 중점부터 골 에지 중점을 향하는 

    //@ Portal (Sight-visibility movement/가시성 고려 최단 거리 이동)
    bool                    m_bUseWaytoPortal;                                                  // 포탈로 이동 할지 여부
    int                     m_iSeqPortal;                                                       // 참조 하는 다음 포탈 지점 셀 인덱스 // WORKING
    Vector3                 m_v3TargetToPortal;                                                 // 참조 하는 다음 포탈 지점 위치           

    //@ static
    static public bool IsNullCell(int cellstatus) 
    {
        return (cellstatus >= (int)E_CELL_STATUS.CELL_STATUS_CNT) ||
                (cellstatus <= (int)E_CELL_STATUS.CELL_STATUS_NULL);
    }

    static public int countSideEdge
    {
        get { return (int)CNAVICELL.E_CELL_SIDEEDGE.E_EDGE_CNT; }
    }

    public int seqCell { get { return m_iSeqCell; } }

    public bool IsNullCell()        { return m_eStatusCell == E_CELL_STATUS.CELL_STATUS_NULL; }
    public bool IsBlockCell()       { return m_eStatusCell == E_CELL_STATUS.CELL_STATUS_BLOCK; }
    public bool IsBlockRoadCell()   { return m_eStatusCell == E_CELL_STATUS.CELL_STATUS_BLOCKROAD; }
    public bool IsGoalCell()        { return m_eStatusCell == E_CELL_STATUS.CELL_STATUS_GOAL; }
    public bool IsRoadCell()        { return m_eStatusCell == E_CELL_STATUS.CELL_STATUS_ROAD; }
    public void SetGoalCell()       { m_eStatusCell = E_CELL_STATUS.CELL_STATUS_GOAL; }
    public void SetBlockCell()      { m_eStatusCell = E_CELL_STATUS.CELL_STATUS_BLOCK;}

    public void SetBlockRoadCell()
    {
        m_eStatusCell = E_CELL_STATUS.CELL_STATUS_BLOCKROAD;
        m_iLevelWeight_BlockRoad = DFLT_LV_BLOCKROADTYPE;
    }

    public bool useWaytoPortal { get { return m_bUseWaytoPortal; } }

    public void SetRoadCell() { m_eStatusCell = E_CELL_STATUS.CELL_STATUS_ROAD; }
    public Vector3 dirCenterToGoal  { get { return m_dirCenterToBest; } }
    public CTRI trilinkcell { get { return m_tri_linker; } }
    public Vector3 poscenterofcell { get { return m_posCenterCell; } }
    public int leveltogoal { get { return m_iLevelToGoal; } }

    //@ Set Portal
    public void SetPortalPosition(int iSeqTriPortal, Vector3 v3PosTargetPortal)
    {
        m_bUseWaytoPortal = true;
        //m_iSeqPortal = iSeqTriPortal; //WORKING
        m_v3TargetToPortal = v3PosTargetPortal;
    }

    public void SetPortalClear()
    {
        m_bUseWaytoPortal = false;
    }

    //@ Calculate cost from GOAL. Function arguments gives from Parent Cell.
    public void setLinkcomputeNaviCell_GOAL(int iLevelFromGoal,
                                            int iSeqCellParent,
                                            float fCostFromGoal)
    {
        m_fcosttoGoal_cen = 0.0f;
        m_fcosttoGoal_ex = 0.0f;
        m_iLevelToGoal = 0;
        SetGoalCell();
    }

    bool getPntsOfEdge(E_CELL_SIDEEDGE edgecell, ref Vector3 pntEdge0, ref Vector3 pntEdge1)
    {
        if (E_CELL_SIDEEDGE.E_EDGE_NULL == edgecell)
        {
            return false;
        }

        int i0 = (int)edgecell;
        int i1 = (i0 + 1) % 3;

        pntEdge0 = m_arrv3PT[i0];
        pntEdge1 = m_arrv3PT[i1];

        return true;
    }

    public bool getPntsOfEdgeBestway(ref Vector3 pntEdge0, ref Vector3 pntEdge1)
    {
        return getPntsOfEdge(m_eEdge_bestway, ref pntEdge0, ref pntEdge1);
    }


    public void setLinkAllAdjs(ref CNAVICELL[] arrnavicellall, ref List<int> listCellAdjCollected)
    {
        bool bComputeOK = false;
        int iLevelFromGoalParent = m_iLevelToGoal;
        int iLevelBlockroad = m_iLevelBlockroadToGoal;

        CNAVICELL.E_CELL_SIDEEDGE eEdgeAdj;

        for (int iEdge = 0; iEdge < 3; ++iEdge)
        {
            eEdgeAdj = (CNAVICELL.E_CELL_SIDEEDGE)iEdge;
            int iSeqCellAdj = m_arriAdjCells[iEdge];
            if (CNAVICELL.NULL_CELL == iSeqCellAdj) { continue; }
            if (eEdgeAdj == m_eEdge_bestway) { continue; }

            CNAVICELL naviCellAdj = arrnavicellall[iSeqCellAdj];

            bComputeOK = naviCellAdj.setLinkcomputeCell(iLevelFromGoalParent + 1,
                                                            m_iSeqCell,
                                                            m_arrfcosttoGoal_edge[iEdge],
                                                            iLevelBlockroad);

            if (true == bComputeOK)
            {
                listCellAdjCollected.Add(iSeqCellAdj);
            }
        } // for( int iEdge=0; iEdge<3; ++iEdge)
    }

    public void setLinkAllAdjs_simulate(int iSeqCell,
                                        int iLevelFromGoal,
                                        ref CNAVICELL[] arrnavicellall, 
                                        ref List<int> listCellsCurrentLevel, 
                                        ref List<int> listCellsNextLevel )
    {
        if (true == IsBlockCell())
        {
            return;
        }

        CNAVICELL.E_CELL_SIDEEDGE eEdgeAdj;
        bool bOverlappedWith = false, bComputeOK = false;

        for (int iEdge = 0; iEdge < 3; ++iEdge)
        {
            eEdgeAdj = (CNAVICELL.E_CELL_SIDEEDGE)iEdge;
            int iSeqCellAdj = m_arriAdjCells[iEdge];

            if (CNAVICELL.NULL_CELL == iSeqCellAdj)
            {
                continue;
            }

            CNAVICELL naviCellAdj = arrnavicellall[iSeqCellAdj];
            if (naviCellAdj.IsBlockCell() || naviCellAdj.IsGoalCell())
            {
                continue;
            }

            if (eEdgeAdj == m_eEdge_bestway)
            {
                continue;
            }

            if (CNAVICELL.NULL_LEVELFROMGOAL == naviCellAdj.m_iLevelToGoal)
            {
                bOverlappedWith = (-1 < listCellsCurrentLevel.BinarySearch(iSeqCellAdj));
                if (false == bOverlappedWith)
                {
                    bComputeOK = naviCellAdj.setLinkcomputeSimulate(iLevelFromGoal,
                                                                    iSeqCell);

                    if (true == bComputeOK)
                    {
                        listCellsNextLevel.Add(iSeqCellAdj);
                    }
                }
            } // if (CNAVICELL.NULL_LEVELFROMGOAL == naviCellAdj.m_iLevelToGoal)

        } // for (int iEdge = 0; iEdge < 3; ++iEdge)
    } // public void setLinkAllAdjs_simulate


    //@ Calculate cost from GOAL. Function arguments gives from Parent Cell.    //현재 셀의 골 비용을 계산 부모 셀의 인자값으로 계산한다
    bool setLinkcomputeCell(int iLevelFromGoal,
                            int iSeqCellParent,
                            float fCostFromGoal,
                            int iCntAncestorIsBlockroad )
    {
        //@ already computed more shorter condition.    //
        if ((m_iLevelToGoal + LEVELNAVI_SIMILAR) < iLevelFromGoal)
        {
            return false;
        }

        //@ Already compute way to go already. // 이미 목표점으로 가는 지점이 요청 셀과 동일하게 구성됬는지 여부
        if (E_CELL_SIDEEDGE.E_EDGE_NULL != m_eEdge_bestway
            && iLevelFromGoal == m_iLevelToGoal
            && m_arriAdjCells[(int)m_eEdge_bestway] == iSeqCellParent)
        {
            return false;
        }

        //@ compute blockroad weight level.
        switch (m_eStatusCell)
        {
            case E_CELL_STATUS.CELL_STATUS_BLOCK:
                {
                    iLevelFromGoal = DFLT_LV_BLOCKTYPE;

                    m_iLevelBlockroadToGoal = iCntAncestorIsBlockroad + 1;

                    if (m_iLevelToGoal != NULL_LEVELFROMGOAL)
                    {
                        return false;
                    }
                }
                break;
            case E_CELL_STATUS.CELL_STATUS_BLOCKROAD:
                {
                    if (0 == iCntAncestorIsBlockroad)
                    {
                        iLevelFromGoal = iLevelFromGoal + m_iLevelWeight_BlockRoad;
                    }

                    //if (m_iLevelToGoal != NULL_LEVELFROMGOAL)
                    //{
                    //    return false;
                    //}

                    m_iLevelBlockroadToGoal = iCntAncestorIsBlockroad + 1;
                } // case E_CELL_STATUS.CELL_STATUS_BLOCKROAD:
                break;
            default:
                {
                    m_iLevelBlockroadToGoal = iCntAncestorIsBlockroad;
                } // default:
                break;
        } // switch (m_eStatusCell)

        //@ select 1 edge as shortest way to goal in 3 of edge.
        //@ if different value is bigger than LEVELNAVI_SIMILAR, give to current level as absolute level value.
        int iEdgeBestWay = (int)E_CELL_SIDEEDGE.E_EDGE_NULL;
        for (int iE = 0; iE < 3; ++iE)
        {
            if (iSeqCellParent == m_arriAdjCells[iE])
            {
                if (m_iLevelToGoal > iLevelFromGoal + LEVELNAVI_SIMILAR)
                {
                    iEdgeBestWay = iE;
                    break;
                }
                else
                {
                    if (m_fcosttoGoal_ex > fCostFromGoal)   
                    {
                        iEdgeBestWay = iE;
                        break;
                    }
                }
            }
        } // for(int iE=0; iE<3; ++iE)

        bool bShorterCostOfPreviousAlready = ((int)E_CELL_SIDEEDGE.E_EDGE_NULL == iEdgeBestWay);
        if (true == bShorterCostOfPreviousAlready)
        {
            return false;
        }

        //@ set each entry edge to goal edge dir.
        Vector3 v3PosEdgeGoal = m_arrv3MddlEdge[iEdgeBestWay];
        for (int iE = 0; iE < 3; ++iE)
        {
            if (iEdgeBestWay == iE)
            {
                continue;
            }

            //@ 추후 최적화 고려
            m_arrDirEntryToBest[iE] = (v3PosEdgeGoal - m_arrv3MddlEdge[iE]).normalized;
        } // for (int iE = 0; iE < 3; ++iE)

        //@ set center to goal dege dir.
        m_dirCenterToBest = (v3PosEdgeGoal - m_posCenterCell).normalized;

        m_fcosttoGoal_ex = fCostFromGoal;
        m_eEdge_bestway = (E_CELL_SIDEEDGE)iEdgeBestWay;

        m_fcosttoGoal_cen = m_fcosttoGoal_ex + m_arrfDistEdgtoCntr[(int)m_eEdge_bestway];
        m_iLevelToGoal = iLevelFromGoal;

        for (int iE = 0; iE < 3; ++iE)
        {
            if (iEdgeBestWay != iE)
            {
                m_arrfcosttoGoal_edge[iE] = m_fcosttoGoal_cen + m_arrfDistEdgtoCntr[iE];
            }
        }

        return true;
    } // public bool setLinkcomputeCell


    public bool setLinkcomputeSimulate(int iLevelFromGoal,
                                int iSeqCellParent)
    {
        if (E_CELL_STATUS.CELL_STATUS_BLOCK == m_eStatusCell)
        {
            return false;
        }

        if (0 == iLevelFromGoal)	// This Cell is obviously Goal.
        {
            m_iLevelToGoal = 0;
            SetGoalCell();

            return true;
        }

        m_eStatusCell = E_CELL_STATUS.CELL_STATUS_ROAD;



        m_iLevelToGoal = iLevelFromGoal;

        return true;
    } // public bool setLinkcompute_

    //@ Cell clear and set flag unprocessed before all mapping navigation event call.
	public void setClear_unprocessed()
    {
        for(int i=0; i<3; ++i)
        {
	        m_arrfcosttoGoal_edge[i]=-1.0f;
        }

        m_iLevelToGoal = NULL_LEVELFROMGOAL;
        m_eEdge_bestway = E_CELL_SIDEEDGE.E_EDGE_NULL;

        m_fcosttoGoal_ex = CMATH.FLOAT_MAX;
        m_fcosttoGoal_cen = CMATH.FLOAT_MAX;

        m_eStatusCell = E_CELL_STATUS.CELL_STATUS_ROAD;

        m_bUseWaytoPortal = false;

        m_iLevelBlockroadToGoal = 0;
    }

    //@ get/return
    public Vector3 getPos_cellCenter()
    {
        return m_posCenterCell;
    }

    public Vector3 getDirEntryToGoal(int iSeqCell)
    {
        E_CELL_SIDEEDGE cellSideEdge = getEdge_AdjCell(iSeqCell);

        if (cellSideEdge != E_CELL_SIDEEDGE.E_EDGE_NULL)
        {
            return m_arrDirEntryToBest[(int)cellSideEdge];
        }

        Debug.Log("Error. null_dir. NaviCell(" + m_iSeqCell + "), requestSeq(" + iSeqCell + ")/cellSideEdge(" + cellSideEdge + ")//" + UnityEngine.Random.Range(0.0f, 1000.0f));
        return Vector3.zero;
    }

    E_CELL_SIDEEDGE getEdge_AdjCell( int iSeqCell )
    {
        for (int iE = 0; iE < 3; ++iE)
        {
            if (iSeqCell == m_arriAdjCells[iE])
            {
                return (E_CELL_SIDEEDGE)iE;
            }
        }

        return E_CELL_SIDEEDGE.E_EDGE_NULL;
    }

    public bool getPos_BestWay_onEdge(ref Vector3 v3PosWay_Edge)
    {
        if (E_CELL_SIDEEDGE.E_EDGE_NULL == m_eEdge_bestway)
        {
            Debug.Log("ERROR!!! E_EDGE_NULL==m_eEdge_bestway. CellSeq=" + m_iSeqCell);
            return false;
        }

        v3PosWay_Edge = m_arrv3MddlEdge[(int)m_eEdge_bestway];
        return true;
    } // public bool getPos_BestWay_onEdge(ref Vector3 v3PosWay_Edge)

    //@ get/return: Shortest way to GOAL position, one of middle of Edges.
	public bool getPos_nextWay(ref Vector3 v3PosWaytoGoal)
    {
        if (true == IsGoalCell())
        {
            return true;
        }

        if(E_CELL_SIDEEDGE.E_EDGE_NULL==m_eEdge_bestway)
        {
            Debug.Log("ERROR!!! E_EDGE_NULL==m_eEdge_bestway. CellSeq=" + m_iSeqCell);
            return false;
        }

        if (true == m_bUseWaytoPortal)
        {
            v3PosWaytoGoal = m_v3TargetToPortal;
        }
        else
        {
            v3PosWaytoGoal = m_arrv3Perpendicular[(int)m_eEdge_bestway];
        }
        return true;
    } // public bool getPos_nextWay(ref Vector3 v3PosWaytoGoal)

	public int getIdxCell_nextWay()
    {
        if (E_CELL_SIDEEDGE.E_EDGE_NULL == m_eEdge_bestway)
        {
            return NULL_CELL;
        }

        return m_arriAdjCells[(int)m_eEdge_bestway];
    }

    //@ Link Edge to adjacent cell.
	public void SetAdjacentCell(	int eEdgeType, int iSeqCellAdjacent )
	{
		m_arriAdjCells[eEdgeType] = iSeqCellAdjacent;
	}

    //@ Get AdjacentTri
    public CADJ_TRI getAdj()
    {
        CADJ_TRI adjTri = new CADJ_TRI();
        Array.Copy(m_arriAdjCells, adjTri._arrTrisAdjacent, m_arriAdjCells.Length);

        return adjTri;
    }

    //@ Not use yet.
	public bool IsPosCrossBestEdge( Vector3 v3Pos )
    {
        Vector3	v3DirMidtoCen = m_arrv3MddlEdge[(int)m_eEdge_bestway]-m_posCenterCell;
        Vector3 v3DirMidtoPos = m_arrv3MddlEdge[(int)m_eEdge_bestway]-v3Pos;

        v3DirMidtoCen.Normalize();
        v3DirMidtoPos.Normalize();

        if(Vector3.Dot( v3DirMidtoCen, v3DirMidtoPos)<0)
        {
	        return true;
        }

        return false;
    }

    //@ Did adjacented.
	public bool DidSetAllAdjacent()
    {
        for( int i=0; i<3; ++i )
        {
            if (NULL_CELL == m_arriAdjCells[i])
	        {
		        return false;
	        }
        }

        return true;
    }


    //@ return:which edge is shared for build adjacent cell. Numbering shared Count other cell.
    static public int getTypeEdge(int iSharedNum)
    {
        E_CELL_SIDEEDGE eSideEdge = E_CELL_SIDEEDGE.E_EDGE_NULL;
        switch (iSharedNum)
        {
            case 1:
                {
                    eSideEdge = E_CELL_SIDEEDGE.E_EDGE_01;
                }
                break;
            case 2:
                {
                    eSideEdge = E_CELL_SIDEEDGE.E_EDGE_20;
                }
                break;
            case 3:
                {
                    eSideEdge = E_CELL_SIDEEDGE.E_EDGE_12;
                }
                break;
            default:
                {
                    Debug.Log("NOSHARED IV NAVICELL.ERROR");
                }
                break;
        }

        return (int)eSideEdge;
    }

    //@ Post-Initialize
    //@ Insert Cells Basic Information and calculate internal cell distance from cell center to each cell edge middle. 
    //  It use for which cell is shortest way to GOAL.
    public void SetComponentCell(int iSeqCell_, CTRI Tri_)
    {
        m_iSeqCell = iSeqCell_;
        m_tri_linker = Tri_;

        Array.Copy(Tri_._arrv3PT, m_arrv3PT, Tri_._arrv3PT.Length);
        computeCellData();
    } // protected void SetComponentCell(Vector3[] arv3TriPT, Vector3 v3NormalCell)

    //@ Calculate distance of each Cell's Edge from Cell's Center.
    protected void computeCellData()
    {
        m_posCenterCell = (m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_0] + m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_1] + m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_2]) / 3;

        m_arrv3MddlEdge[(int)E_CELL_SIDEEDGE.E_EDGE_01] = (m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_0] + m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_1]) / 2;
        m_arrv3MddlEdge[(int)E_CELL_SIDEEDGE.E_EDGE_12] = (m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_1] + m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_2]) / 2;
        m_arrv3MddlEdge[(int)E_CELL_SIDEEDGE.E_EDGE_20] = (m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_2] + m_arrv3PT[(int)E_CELL_VERTEX.E_VERTEX_0]) / 2;

        float fdistEdgtoCntr_sum = 0.0f;
        for (int i = 0; i < 3; ++i)
        {
            m_arrfDistEdgtoCntr[i] = Mathf.Abs((m_arrv3MddlEdge[i] - m_posCenterCell).magnitude);
            fdistEdgtoCntr_sum = fdistEdgtoCntr_sum + m_arrfDistEdgtoCntr[i];

            Vector3 vectorPnttoCenter = m_posCenterCell - m_arrv3PT[i];
            Vector3 vectorEdge = m_arrv3PT[(i + 1) % 3] - m_arrv3PT[i];
            Vector3 dirvectorEdge = vectorEdge.normalized;

            //Projection
            m_arrv3Perpendicular[i] = m_arrv3PT[i] + Vector3.Project(vectorPnttoCenter, dirvectorEdge);
        }
    } 


    // 함수:InitializeNaviCell()
	//@ Process 각 네비셀들의 초기화 작업
    //@ Set all properties unprocessed and set NULL.
	public void InitializeNaviCell()
    {
        m_iSeqCell = NULL_CELL;

        for(int i=0; i<3; ++i)
        {
            m_arriAdjCells[i] = NULL_CELL;
	        m_arrfcosttoGoal_edge[i]=-1.0f;
        }

        m_iLevelToGoal = NULL_LEVELFROMGOAL;
        m_eEdge_bestway = E_CELL_SIDEEDGE.E_EDGE_NULL;

        m_fcosttoGoal_ex = CMATH.FLOAT_MAX;
        m_fcosttoGoal_cen = CMATH.FLOAT_MAX;

        m_tri_linker = null;
        m_eStatusCell = E_CELL_STATUS.CELL_STATUS_ROAD;

        m_bUseWaytoPortal = false;
    }
} // public class CNAVICELL

