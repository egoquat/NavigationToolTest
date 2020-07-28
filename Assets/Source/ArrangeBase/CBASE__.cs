using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CBASE__
{
    //@ Internal properties
    protected int _IdxBase = -1;
    protected int _IdxType = -1;

    public List<int> _listIdxTris = new List<int>();
    public List<Vector3> _listv3Pnts = new List<Vector3>();         // pure triangle point

    public List<Vector3> _listv3PntsSrc = new List<Vector3>();      // summary point

    public Vector3 _v3PositionCenter;

    public void ReleaseBase__()
    {
        _listIdxTris.Clear();
        _listv3Pnts.Clear();
        _listv3PntsSrc.Clear();
    }

    //@ Operator
    public bool IsSimilar(CBASE__ baseRight)
    {
        foreach (int iTriLeft in _listIdxTris)
        {
            foreach (int iTriRight in baseRight._listIdxTris)
            {
                if (iTriLeft == iTriRight)
                {
                    return true;
                }
            }
        } // foreach (int iTriLeft in baseLeft._listIdxTris)

        return false;
    } // public static bool IsSimilar(CBASE__ baseRight)


    //@ Get/Set
    public int getIdxBase()
    {
        return _IdxBase;
    }

    public void setIdxBase(int iIdxBase)
    {
        _IdxBase = iIdxBase;
    }

    public int getIdxType()
    {
        return _IdxType;
    }

    public void setIdxType(int iIdxType)
    {
        _IdxType = iIdxType;
    }

    public Vector3 getCenterPosition()
    {
        return _v3PositionCenter;
    }

    public Vector3[] getArrayPoint()
    {
        return _listv3Pnts.ToArray();
    }

    public Vector3[] getArrayPoint_src()
    {
        return _listv3PntsSrc.ToArray();
    }

    //@ Ray Instersect
    public bool FindinTri_onAB(int iIdxTri_Request)
    {
        if (_listIdxTris.Count < 1)
        {
            return false;
        }

        return (-1 < _listIdxTris.BinarySearch(iIdxTri_Request));
    }

    //@ Construct New Triangle
    protected void SetNewTri(int iSeqTri, CTRI tri_in)
    {
        for (int iSeqPnt = 0; iSeqPnt < tri_in._arrv3PT.Length; ++iSeqPnt)
        {
            Vector3 v3PntTri = tri_in._arrv3PT[iSeqPnt];
            _listv3Pnts.Add(v3PntTri);
        }

        _listIdxTris.Add(iSeqTri);
        _listIdxTris.Sort();
    }

    //@ Construct Triangle
    public bool InsertNewTri(int iSeqTri, CTRI tri_in, bool bConfirm)
    {
        bool bAlreadyExisted = false;
        if (_listIdxTris.Count > 0)
        {
            bAlreadyExisted = (-1 < _listIdxTris.BinarySearch(iSeqTri));
        }

        if (true == bAlreadyExisted)
        {
            return false;
        }

        for (int iPnt = 0; iPnt < tri_in._arrv3PT.Length; ++iPnt)
        {
            _listv3PntsSrc.Add(tri_in._arrv3PT[iPnt]);
        }

        if (_listv3Pnts.Count < 1)
        {
            SetNewTri(iSeqTri, tri_in);
        }
        else
        {
            List<Vector3> listInsertPnt = new List<Vector3>();
            bool bAlreadyPutin = false;

            for (int iPntTri = 0; iPntTri < 3; ++iPntTri)
            {
                Vector3 v3PntTri = tri_in._arrv3PT[iPntTri];

                bAlreadyPutin = false;

                for (int iSeqPnt = 0; iSeqPnt < _listv3Pnts.Count; ++iSeqPnt)
                {
                    Vector3 v3Pnt_ = _listv3Pnts[iSeqPnt];

                    if (CMATH.similarVector3_f2(v3PntTri, v3Pnt_))
                    {
                        bAlreadyPutin = true;
                        break;
                    }
                }

                if (false == bAlreadyPutin)
                {
                    listInsertPnt.Add(v3PntTri);
                }
            } // for(int iPntTri = 0; iPntTri < 3; ++iPntTri)

            if (listInsertPnt.Count > 0)
            {
                if (listInsertPnt.Count == tri_in._arrv3PT.Length)
                {
                    Debug.Log("(Wrong value.//listInsertPnt.Count > 0)//(listInsertPnt.Count == tri_in._arrv3PT.Length)//");
                }

                for (int iSeq = 0; iSeq < listInsertPnt.Count; ++iSeq)
                {
                    Vector3 v3Pnt = listInsertPnt[iSeq];
                    _listv3Pnts.Add(v3Pnt);
                }

                listInsertPnt.Clear();
            } // if(listInsertPnt.Count>0)

            _listIdxTris.Add(iSeqTri);
            _listIdxTris.Sort();
        } // if(_listv3Pnts.Count<1)

        return true;

    } // public void InsertNewTri(int iSeqTri, CTRI tri_in)

    //@ Construct Base__
    public bool CalculateBase__(int iSeqBase__)
    {
        int iCntPnts = _listv3Pnts.Count;

        if (iCntPnts < 1)
        {
            return false;
        }

        _IdxBase = iSeqBase__;

        Vector3 v3Cal = new Vector3(0.0f, 0.0f, 0.0f);
        for (int iPnt = 0; iPnt < iCntPnts; ++iPnt)
        {
            Vector3 v3Pnt_ = _listv3Pnts[iPnt];
            v3Cal = v3Cal + v3Pnt_;
        }
        _v3PositionCenter = v3Cal / iCntPnts;
        //_listv3Pnts.Clear();

        return true;
    } // public bool CalculateBase__()

    ~CBASE__()
    {
    }
} // CBASE__
