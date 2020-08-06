using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//@ Container collect and construct triangles for navigation.
public class CTriCollector
{
    #region constparameter
    readonly Color color_overllaped_triangle = Color.Lerp(Color.yellow, Color.red, 0.3f);
    #endregion

    public bool _bReadyToExecuteBuildup_NAVI = false;

    //@ BLOCK CELL List, GOAL CELL List, START CELL List, NAVI BaseTower List, TRI ALL LIST
    public List<int> m_listTris_naviBlock = new List<int>();
    public List<int> m_listTris_naviBlockRoad = new List<int>();
    public List<int> m_listTris_naviGoal = new List<int>();
    public List<int> m_listTris_naviStart = new List<int>();

    //List<CRenderLine> m_listRenderLine;
    public List<CTRI> m_listTris = new List<CTRI>();

    protected CDrawRenderMesh m_drawRenderMesh_forcheckoverlap = new CDrawRenderMesh();

    protected CNAVICELL.CADJ_TRI[] m_arrAdjTris;

    public CNAVICELL.CADJ_TRI[] arrayAllTriAdjacent
    {
        get { return m_arrAdjTris; }
    }

    public void InitTriCollector()
    {
        if (true == processCycle.GetInstance._modeTool)
        {
            m_drawRenderMesh_forcheckoverlap.InitDrawRenderMesh();
        }
    }

    public void DestructAllTris()
    {
        Clear_functionalTris();

        m_listTris.Clear();

        _bReadyToExecuteBuildup_NAVI = false;
    }

    public List<CTRI> getTris()
    {
        return m_listTris;
    }

    public void getTris(out List<CTRI> listTris_out)
    {
        listTris_out = m_listTris;
    }

	public bool getTriIdxAll(	ref int[] arTriIdx_ )
	{
		int iCntAllTris=m_listTris.Count;
		if(iCntAllTris<1) 
		{
			return false;
		}

        Array.Resize( ref arTriIdx_, iCntAllTris );

		for(int iTriIdx=0;iTriIdx<iCntAllTris;++iTriIdx)
		{
			arTriIdx_[iTriIdx]=iTriIdx;
		}

		return true;
	}

	public int getCountTris(){ return m_listTris.Count; }

	public bool getTri(int iIdxTriangle, ref CTRI tri_out)
    {
	    if( iIdxTriangle > m_listTris.Count)	
	    {
		    Debug.Log("Accessed Index is out of range in Triangle Container");
		    return false;
	    }

	    tri_out = m_listTris[iIdxTriangle];

	    return true;
    }
	
	public Vector3 getTriCenter(int iIdxTriangle)
	{
		Vector3 [] arrV3Pnts = m_listTris[iIdxTriangle]._arrv3PT;
		
		Vector3 result;
		result = (arrV3Pnts[0] + arrV3Pnts[1] + arrV3Pnts[2]) / 3.0f;
		
		return result;
	}

	public CTRI getTri(int uiSeqTri)
    {
        if (uiSeqTri >= m_listTris.Count)
        {
            Debug.Log("getTri(int uiSeqTri)//uiSeqTri >= m_listTris.Count//m_listTris.Count=" 
                + m_listTris.Count + "//uiSeqTri=" + uiSeqTri + "////");
            return null;
        }
        
        return m_listTris[uiSeqTri]; 
    } 

    public void Clear_functionalTris()
    {
        m_listTris_naviBlock.Clear();
        m_listTris_naviBlockRoad.Clear();
        m_listTris_naviGoal.Clear();
        m_listTris_naviStart.Clear();
    }

    public int getCount_NaviFunction_Tris()
    {
        return (m_listTris_naviGoal.Count 
               + m_listTris_naviBlock.Count 
               + m_listTris_naviBlockRoad.Count
               + m_listTris_naviStart.Count);
    }

    public bool isExistTri(int iIdxTri, List<int> listTrinavicells)
    {
        bool isExist = false == listTrinavicells.IsNullOrEmpty()
            && listTrinavicells.Exists(t => { return t == iIdxTri; });
        return isExist;
    }

    public bool deletetriByLinear(int iIdxTri, ref List<int> listTrinavicells)
    {
        int iDeleteAt = -1;
        for(int iterTri = 0; iterTri < listTrinavicells.Count; ++iterTri)
        {
            int iCurrentTri = listTrinavicells[iterTri];
            if (iIdxTri == iCurrentTri)
            {
                iDeleteAt = iterTri;
            }
        }

        if (-1 != iDeleteAt)
        {
            listTrinavicells.RemoveAt(iDeleteAt);
            return true;
        }

        return false;
    }

    protected bool extractNavicellToAdj(
                                List<CTRI> listTris,
                                CNAVICELL[] arrNavicells,
                                ref CNAVICELL.CADJ_TRI[] arrAdjs)
    {
        int iCntNaviCell = arrNavicells.Length;

        if (iCntNaviCell < 1)
        {
            Debug.Log("if(iCntNaviCell < 1)");
            return false;
        }

        arrAdjs = new CNAVICELL.CADJ_TRI[iCntNaviCell];

        for (int iSeqTri = 0; iSeqTri < iCntNaviCell; ++iSeqTri)
        {
            CNAVICELL naviCellCurr = arrNavicells[iSeqTri];
            arrAdjs[iSeqTri] = naviCellCurr.getAdj();
            arrAdjs[iSeqTri].tri = listTris[iSeqTri];
        }
        return true;
    }

    public int getTriHypotenuse(CNAVICELL.CADJ_TRI[] arrAdjTri, List<CTRI> listTris, int iSeqTri)
    {
        if (arrAdjTri.Length < 1 || iSeqTri < 0)
        {
            return -1;
        }

        CNAVICELL.CADJ_TRI tri_Adj = arrAdjTri[iSeqTri];
        CTRI tri = listTris[iSeqTri];

        int iEdge_Hyptenuse = tri.GetHypotenuseEdge();
        if (CTRI.NULL_TRI_EDGE == iEdge_Hyptenuse)
        {
            return -1;
        }

        int iTri_Hypotenuse = tri_Adj._arrTrisAdjacent[iEdge_Hyptenuse];
        if (iTri_Hypotenuse == CNAVICELL.NULL_CELL)
        {
            return -1;
        }

        return iTri_Hypotenuse;
    }

    public bool setExtractAdjCells(CNavigation navi)
    {
        m_arrAdjTris = null;

        return extractNavicellToAdj(m_listTris, navi.getNaviCellsAll(), ref m_arrAdjTris);
    }

    public int searchTriHypotenuse(int iIdxTri)
    {
        if (iIdxTri >= m_listTris.Count || iIdxTri < 0)
        {
            return -1;
        }

        return getTriHypotenuse(m_arrAdjTris, m_listTris, iIdxTri);
    }

    //@ Collect all isoscelles right trignales
    public bool collectAll_IsoscellesRightTriangles(ref List<int> listIRTris)
    {
        for(int iSeqTri=0; iSeqTri < m_listTris.Count; ++iSeqTri)
        {
            CTRI triCurr = m_listTris[iSeqTri];
            if(true==triCurr.IsoscelesRightAngleTriangle(0.05f))
            {
                listIRTris.Add(iSeqTri);
            }
        } 

        return (listIRTris.Count > 0);
    }

    //@ Function has little risk about triangle match. 
    protected bool constructAllTris_IVrecompute(int[] arrTri_in,
                                                Vector3[] arrVertex_in,
                                                ref List<CTRI> listTris_out)
    {
        Vector3[] arrv3TriVer = new Vector3[CTRI.CNTPNTTRI];
        int[] ariTriIdx = new int[CTRI.CNTPNTTRI];
        int iCntVB = arrVertex_in.Length;
        int iCntIV = arrTri_in.Length;

        //CHECKOVERLAPPED
        int iCntSameVer__ = 0;

        if (iCntIV < 1 || iCntVB < 1)
        {
            return false;
        }

        int[] arrIBConvert = new int[iCntIV];

        int iIVConvert = 0, iIVCurr = 0;
        bool bfindEQ = false;

        for (int iterIV = 0; iterIV < iCntIV; ++iterIV)
        {
            arrIBConvert[iterIV] = arrTri_in[iterIV];
        }

        for (int iterIV = 0; iterIV < iCntVB; ++iterIV)
        {
            Vector3 v3VBCurr = arrVertex_in[iterIV];
            iIVCurr = iterIV; bfindEQ = false;
            for (int iIVin = 0; iIVin < iIVCurr; ++iIVin)
            {
                Vector3 v3VBCurr_ = arrVertex_in[iIVin];

                //bfindEQ = CMATH.similarVector3_FIn(v3VBCurr, v3VBCurr_, 0.4f);
                bfindEQ = (v3VBCurr == v3VBCurr_); 

                if (bfindEQ)
                {
                    //FORTEST
                    iCntSameVer__++;

                    iIVConvert = iIVin;
                    break;
                }
            } // for(int iIVin=0; iIVin<iterIV; ++IV_)

            if (true == bfindEQ)
            {
                for (int iSeqIV = 0; iSeqIV < iCntIV; ++iSeqIV)
                {
                    int iIBCurr = (int)arrTri_in[iSeqIV];
                    if (iIBCurr == iterIV)
                    {
                        arrIBConvert[iSeqIV] = iIVConvert;
                    }
                }
            }

        } // for(int iterIV=0; iterIV<arrVertex_in.size(); ++iterIV)

        //@ Overlap vertex 
        if (0 < iCntSameVer__)
        {
            Debug.Log("Notice! (overlapped:" + iCntSameVer__ + " vertices.) Count of exactly same position vertices//constructAllTris_IVrecompute()//arrTri_in.Length=" + arrTri_in.Length);
        }

        //@Insert only validation succeeded triangle.
        CTRI triCurrent;
        int iSeqTriCurr = 0, iSeqTri_ = 0;
        Vector3 v3MinAABB = new Vector3(CMATH.FLOAT_MAX, CMATH.FLOAT_MAX, CMATH.FLOAT_MAX),
                v3MaxAABB = new Vector3(-CMATH.FLOAT_MAX, -CMATH.FLOAT_MAX, -CMATH.FLOAT_MAX);

        Vector3 v3VerTri;

        //@ overlap triangles
        int iCntExactlySameTris = 0, iCntNotValidatePoly = 0;
        List<CTRI> listTriOverlap = new List<CTRI>();

        for (int iSeqIdx = 0, iTri = 0; iSeqIdx < iCntIV; iSeqIdx += 3, ++iTri)
        {
            ariTriIdx[0] = arrIBConvert[iSeqIdx];
            ariTriIdx[1] = arrIBConvert[iSeqIdx + 1];
            ariTriIdx[2] = arrIBConvert[iSeqIdx + 2];

            for (int itV = 0; itV < 3; ++itV)
            {
                v3VerTri = new Vector3(arrVertex_in[ariTriIdx[itV]].x, arrVertex_in[ariTriIdx[itV]].y, arrVertex_in[ariTriIdx[itV]].z);

                if (v3VerTri.x > v3MaxAABB.x) { v3MaxAABB.x = v3VerTri.x; }
                if (v3VerTri.y > v3MaxAABB.y) { v3MaxAABB.y = v3VerTri.y; }
                if (v3VerTri.z > v3MaxAABB.z) { v3MaxAABB.z = v3VerTri.z; }

                if (v3VerTri.x < v3MinAABB.x) { v3MinAABB.x = v3VerTri.x; }
                if (v3VerTri.y < v3MinAABB.y) { v3MinAABB.y = v3VerTri.y; }
                if (v3VerTri.z < v3MinAABB.z) { v3MinAABB.z = v3VerTri.z; }

                arrv3TriVer[itV] = v3VerTri;
            }

            triCurrent = new CTRI();
            triCurrent.Set(arrv3TriVer, ariTriIdx);

            if (true == triCurrent.isValidateASPoly())
            {
                bool bBeExistExactlysame = false;

                for (int iSeqIdx__ = 0; iSeqIdx__ < iSeqTriCurr; ++iSeqIdx__)
                {
                    if (iSeqIdx__ != iSeqIdx)
                    {
                        CTRI tri__ = listTris_out[iSeqIdx__];
                        if (tri__.isEqual(triCurrent))
                        {
                            //FORTEST
                            ++iCntExactlySameTris;

                            listTriOverlap.Add(triCurrent);
                            //FORTEST

                            bBeExistExactlysame = true;
                            break;
                        }
                    }
                } // for (int iSeqIdx__ = 0; iSeqIdx__ < iSeqTriCurr; ++iSeqIdx__)

                if (false == bBeExistExactlysame)
                {
                    iSeqTriCurr = iSeqTri_++;

                    listTris_out.Add(triCurrent);
                }
                else
                {
                    Debug.Log("!ERROR. Overlapped Triangle. Exist Exactly same TRI");
                }
            } // if(true==triCurrent.isValidateASPoly())
            else
            { // //CHECKOVERLAPPED
                ++iCntNotValidatePoly;
            } // if(true==triCurrent.isValidateASPoly())

        } // for( uint iSeqIdx=0, iTri=0; iSeqIdx<iCntIV; iSeqIdx+=3,++iTri )

        //CHECKOVERLAPPED
        //@ maya에서 같은 위치에 Overlap된 polygon 작업이 있는 지 여부 검사.
        if(listTriOverlap.Count > 0)
        {
            int iSeq = 0;
            foreach (CTRI triOverlap in listTriOverlap)
            {
                m_drawRenderMesh_forcheckoverlap.DrawNewRendermesh(CMATH.INT_MAX_DIV248 + (iSeq++), triOverlap._arrv3PT, color_overllaped_triangle, CMATH.FEPSILON_F2, -1.0f);
            }

            string strDebugOut = "!ERROR triangles overlapped. //count=" + listTriOverlap.Count + "\n";
            strDebugOut += "!constructAllTris_IVrecompute:CountTris=" + listTris_out.Count + "//" +
                        "ExactlySameTris=" + iCntExactlySameTris + "//" + "NoValidateTris=" + iCntNotValidatePoly + "//" + UnityEngine.Random.Range(10.0f, 10000.0f);

            Debug.Log(strDebugOut);
        } // if(listTriOverlap.Count > 0)
        //CHECKOVERLAPPED

        return true;
    }	//protected bool CTriContainer::constructAllTris_IVrecompute(

    //@ Construct all Triangle refer to Index Buffers, Vertex Buffers.
    protected bool constructAllTris_IV(   int[] arrTri_in,
                                        Vector3[] arrVertex_in,
                                        ref List<CTRI> listTris_out )
    {
        Vector3[] arrv3TriVer = new Vector3[CTRI.CNTPNTTRI];

        int[] ariTriIdx = new int[CTRI.CNTPNTTRI];
        int iCntVB = arrVertex_in.Length;
        int iCntIV = arrTri_in.Length;
        if (iCntIV < 1 || iCntVB < 1)
        {
            Debug.Log("(iCntIV < 1 || iCntVB < 1)");
            return false;
        }

        //int iCntTries = (iCntIV / CTRI.CNTPNTTRI) + 1;

        CTRI triCurrent;
        Vector3 v3MinAABB = new Vector3(CMATH.FLOAT_MAX, CMATH.FLOAT_MAX, CMATH.FLOAT_MAX),
                v3MaxAABB = new Vector3(-CMATH.FLOAT_MAX, -CMATH.FLOAT_MAX, -CMATH.FLOAT_MAX);

        Vector3 v3VerTri;

        for (int iSeqIdx = 0, iTri = 0; iSeqIdx < iCntIV; iSeqIdx += 3, ++iTri)
        {
            ariTriIdx[0] = arrTri_in[iSeqIdx];
            ariTriIdx[1] = arrTri_in[iSeqIdx + 1];
            ariTriIdx[2] = arrTri_in[iSeqIdx + 2];

            for (int itV = 0; itV < 3; ++itV)
            {
                v3VerTri = new Vector3(arrVertex_in[ariTriIdx[itV]].x, arrVertex_in[ariTriIdx[itV]].y, arrVertex_in[ariTriIdx[itV]].z);

                if (v3VerTri.x > v3MaxAABB.x) { v3MaxAABB.x = v3VerTri.x; }
                if (v3VerTri.y > v3MaxAABB.y) { v3MaxAABB.y = v3VerTri.y; }
                if (v3VerTri.z > v3MaxAABB.z) { v3MaxAABB.z = v3VerTri.z; }

                if (v3VerTri.x < v3MinAABB.x) { v3MinAABB.x = v3VerTri.x; }
                if (v3VerTri.y < v3MinAABB.y) { v3MinAABB.y = v3VerTri.y; }
                if (v3VerTri.z < v3MinAABB.z) { v3MinAABB.z = v3VerTri.z; }

                arrv3TriVer[itV] = v3VerTri;
            }
            triCurrent = new CTRI();
            triCurrent.Set(arrv3TriVer, ariTriIdx);

            listTris_out.Add(triCurrent);
        }

        return true;
    } //protected bool constructAllTris_IV(
	
    //@ Build up all triangles which use navigation.
    // -arrTri_in:Index Buffer Array
    // -arrVertex_in:Vertex Buffer Array
    // -tmUser_in:Transform
    // -bRecomputeAllIV:Refer to IV directly or indirectly.
    //                   In case of indirectly, all index buffers re-linked based on real position.
    public bool constructAllTris(	int[]	    arrTri_in,
							        Vector3[]	arrVertex_in, 
                                    Transform   tmUser_in,
                                    bool        bRecomputeAllIV)
    {
	    bool bResult=false;

        bool bNeedtoTransform = false;

        float fMagTranslate, fMagScale, fMagRotate;
        fMagTranslate = tmUser_in.localPosition.magnitude;
        fMagScale = tmUser_in.localScale.magnitude;
        fMagRotate = tmUser_in.eulerAngles.magnitude;

        if (fMagTranslate > CMATH.FEPSILON_F5 ||
            fMagScale != 1.0f ||
            fMagRotate > CMATH.FEPSILON_F5)
        {
            bNeedtoTransform = true;
        }

        Vector3[] arrVertex__ = new Vector3[arrVertex_in.Length];
        Array.Copy(arrVertex_in, arrVertex__, arrVertex_in.Length);

        Vector3 v3VBCurr;
        if(true == bNeedtoTransform)
        {
            for (int iVBSeq = 0; iVBSeq < arrVertex_in.Length; ++iVBSeq)
            {
                v3VBCurr = arrVertex__[iVBSeq];
                v3VBCurr = tmUser_in.TransformPoint(v3VBCurr);

                arrVertex__[iVBSeq] = v3VBCurr;
            }
        }
        
        if (true==bRecomputeAllIV)
	    {
            bResult = constructAllTris_IVrecompute(arrTri_in,
                                                    arrVertex__,
									                ref m_listTris );
	    }
	    else
 	    {
            bResult = constructAllTris_IV(   arrTri_in,
                                             arrVertex__,
                                             ref m_listTris );
	    }

	    return bResult;
    }

    //@ Intersect ray to tries iteration.
    public CBASE__ GetIntersectedRay(Vector3 lineOrigin, Vector3 lineDir)
    {
        if (true == m_listTris.IsNullOrEmpty())
        {
            return null;
        }

        List<CTRI> triesForIteration = m_listTris.Where(t => { return null != t.GetBaseLinked; }).ToListOrDefault();
        if (true == triesForIteration.IsNullOrEmpty())
        {
            return null;
        }
        
        CTRI intersectedTri = triesForIteration.FirstOrDefault(t=> 
        {
            return CMATH.IntersectRayTriSimple(lineOrigin, lineDir, t._arrv3PT[0], t._arrv3PT[1], t._arrv3PT[2]); 
        });

        if (null == intersectedTri)
        {
            return null;
        }

        CBASE__ intersectedBase = intersectedTri.GetBaseLinked;
        return intersectedBase;
    }

} // public class CTriCollector
