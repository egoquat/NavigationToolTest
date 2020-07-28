using UnityEngine;
using System;
using System.Collections;

//@ Triangle is refered by Index Buffer, Vertex Buffer in geometry.
public class CTRI
{
    //@ CELL(==TRIANGLE)'s EDGE 0-1, 1-2, 2-0
    public enum E_TRI_EDGE
    {
        E_TRI_EDGE_NULL = CTRI.NULL_TRI_EDGE,
        E_TRI_EDGE_01 = 0,
        E_TRI_EDGE_12,
        E_TRI_EDGE_20,
        E_TRI_EDGE_CNT,
    }; // public enum E_TRI_EDGE

    #region constparameter
    public const int NULL_TRI = -1;
    public const int NULL_TRI_EDGE = -1;
    public const int CNTPNTTRI = 3;
    #endregion // #region constparameter

    public Vector3 _v3TriNormal = new Vector3();
    public Vector3[] _arrv3PT = new Vector3[CNTPNTTRI];
    public int[] _arriIV = new int[CNTPNTTRI];

    //@ Value for Isosceles, right-angle triangle 
    public float[] _arrEdgeLength = new float[3];                           // °¢ »ï°¢Çü º¯ÀÇ ±æÀÌ
    public E_TRI_EDGE _triedge_Hypotenuse = E_TRI_EDGE.E_TRI_EDGE_NULL;     // ÀÌµîº¯-Á÷°¢»ï°¢ÇüÀÇ ºøº¯
    public bool bIsoscelesRightAngleTriangle = false;
    //public int 

    //@ Set / Get
    public void Set(CTRI triSrc)
    {
        _v3TriNormal = triSrc._v3TriNormal;
        Array.Copy(triSrc._arrv3PT, _arrv3PT, triSrc._arrv3PT.Length);
        Array.Copy(triSrc._arriIV, _arriIV, triSrc._arriIV.Length);

        Cal_EdgeDistance();
    }

    public void Set(Vector3[] arrv3Ver_,int[] ariIV_)
    {
        Array.Copy(arrv3Ver_, _arrv3PT, CNTPNTTRI);
        Array.Copy(ariIV_, _arriIV, CNTPNTTRI);

        CalculateNormal(arrv3Ver_[0], arrv3Ver_[1], arrv3Ver_[2]);

        Cal_EdgeDistance();
    }

    public void Set(Vector3 v0_, Vector3 v1_, Vector3 v2_,
                int iIV0, int iIV1, int iIV2)
    {
        _arrv3PT[0] = v0_;
        _arrv3PT[1] = v1_;
        _arrv3PT[2] = v2_;

        CalculateNormal(_arrv3PT[0], _arrv3PT[1], _arrv3PT[2]);
        _arriIV[0] = iIV0; _arriIV[1] = iIV1; _arriIV[2] = iIV2;

        Cal_EdgeDistance();
    }

    protected void CalculateNormal(Vector3 v0_, Vector3 v1_, Vector3 v2_)
    {
        Vector3 v3Edge01 = v1_ - v0_;
        Vector3 v3Edge02 = v2_ - v0_;
        _v3TriNormal = Vector3.Cross(v3Edge01, v3Edge02);
    }

    public void GetEdgePoint(E_TRI_EDGE eTriEdge, ref Vector3 v3Pnt0, ref Vector3 v3Pnt1)
    {
        switch (eTriEdge)
        {
            case E_TRI_EDGE.E_TRI_EDGE_01:
            {
                v3Pnt0 = _arrv3PT[0];
                v3Pnt1 = _arrv3PT[1];
            }
            break;
            case E_TRI_EDGE.E_TRI_EDGE_12:
            {
                v3Pnt0 = _arrv3PT[1];
                v3Pnt1 = _arrv3PT[2];
            }
            break;
            case E_TRI_EDGE.E_TRI_EDGE_20:
            {
                v3Pnt0 = _arrv3PT[2];
                v3Pnt1 = _arrv3PT[0];
            }
            break;
        } // switch (eTriEdge)
    } // public void GetEdgePoint(E_TRI_EDGE eTriEdge, ref Vector3 v3Pnt0, ref Vector3 v3Pnt1)

    public bool IntersectRay(Vector3 posOriginRay, Vector3 posdirRay, ref Vector3 posIntersected)
    {
        return CMATH.IntersectRayTri_GetPos(posOriginRay, posdirRay, _arrv3PT, ref posIntersected);
    }

    public bool IntersectRay_dirdown(Vector3 v3PosOrigin, ref Vector3 v3PosIntersected)
    {
        return CMATH.IntersectRayTri_GetPos(v3PosOrigin,
                                        Vector3.down,
                                        _arrv3PT,
                                        ref v3PosIntersected);
    }

    //@ Is Isosceles-RightAngle Triangle
    public bool IsisoscelesRightAngle()
    {
        return bIsoscelesRightAngleTriangle;
    }


    //@ Get sample anyoneEdge except Hypotenuse
    public float GetLengthEdge_ExactHypotenuse_Sample()
    {
        return _arrEdgeLength[(((int)_triedge_Hypotenuse) + 1) % 3];
    }

    //@ Get hypotenuse(ºøº¯)
    public int GetHypotenuseEdge()
    {
        //return (int)_triedge_Hypotenuse;
        int iEdgeMoreLen = 0;

        if (_arrEdgeLength[0] < _arrEdgeLength[1])
        {
            iEdgeMoreLen = 1;
        }

        if (_arrEdgeLength[iEdgeMoreLen] < _arrEdgeLength[2])
        {
            iEdgeMoreLen = 2;
        }

        return iEdgeMoreLen;
    } // public int GetHypotenuseEdge()

    //@ Calculate for Visibile Process Navigation
    protected void Cal_EdgeDistance()
    {
        _arrEdgeLength[0] = Vector3.Distance(_arrv3PT[1], _arrv3PT[0]);
        _arrEdgeLength[1] = Vector3.Distance(_arrv3PT[2], _arrv3PT[1]);
        _arrEdgeLength[2] = Vector3.Distance(_arrv3PT[0], _arrv3PT[2]);
    }

    //@ Right-Angled Triangle or not.
    public bool isRightAngledTriangle_est(float fEstRightAngle)
    {
        Vector3 v3DirEdge01 = _arrv3PT[1] - _arrv3PT[0];
        Vector3 v3DirEdge02 = _arrv3PT[2] - _arrv3PT[0];

        bool bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);

        if(false==bRightAngle)
        {
            v3DirEdge01 = _arrv3PT[0] - _arrv3PT[1];
            v3DirEdge02 = _arrv3PT[2] - _arrv3PT[1];

            bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);

            if(false==bRightAngle)
            {
                v3DirEdge01 = _arrv3PT[1] - _arrv3PT[2];
                v3DirEdge02 = _arrv3PT[0] - _arrv3PT[2];

                bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);
            }
        }

        return bRightAngle;
    } // protected bool isRightAngledTriangle_est()

    //@ Isosceles Triangle or not.
    public bool isIsoscelesTriangle_est(float fEstIsosceles)
    {
        float fEpsilonEst = Mathf.Abs(fEstIsosceles);
        for (int iEdge = 0; iEdge < 3; ++iEdge )
        {
            float leftLen = _arrEdgeLength[iEdge];
            for(int iEdge_=0; iEdge_ < 3; ++iEdge_)
            {
                if( iEdge==iEdge_ )
                {
                    continue;
                }

                float rightLen = _arrEdgeLength[iEdge_];

                if (CMATH.similarEst_float(leftLen, rightLen, fEpsilonEst))
                {
                    return true;
                }
            } // for(int iEdge_=0; iEdge_ < 3; ++iEdge_)
        } // for (int iEdge = 0; iEdge < 3; ++iEdge )

        return false;
    } // public bool isIsoscelesTriangle_est(float fEstIsosceles)

    //@ Is this Isosceles-RightAngle Triangle? 
    public bool IsoscelesRightAngleTriangle(float fRate)
    {
        float fEstIsosceles = ((_arrEdgeLength[0] + _arrEdgeLength[1] + _arrEdgeLength[2]) / 3) * (fRate*2);
        float fEstRightAngle = 180.0f * fRate;

        Vector3 v3DirEdge01 = _arrv3PT[1] - _arrv3PT[0];
        Vector3 v3DirEdge02 = _arrv3PT[2] - _arrv3PT[0];

        bool bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);
        if (true == bRightAngle)
        {
            if(CMATH.similarEst_float(_arrEdgeLength[0], _arrEdgeLength[2], fEstIsosceles))
            {
                bIsoscelesRightAngleTriangle = true;
                _triedge_Hypotenuse = E_TRI_EDGE.E_TRI_EDGE_12;
                return true;
            }
        }
        else
        {
            v3DirEdge01 = _arrv3PT[0] - _arrv3PT[1];
            v3DirEdge02 = _arrv3PT[2] - _arrv3PT[1];

            bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);

            if (true == bRightAngle)
            {
                if(CMATH.similarEst_float(_arrEdgeLength[0], _arrEdgeLength[1], fEstIsosceles))
                {
                    bIsoscelesRightAngleTriangle = true;
                    _triedge_Hypotenuse = E_TRI_EDGE.E_TRI_EDGE_20;
                    return true;
                }
            }
            else
            {
                v3DirEdge01 = _arrv3PT[1] - _arrv3PT[2];
                v3DirEdge02 = _arrv3PT[0] - _arrv3PT[2];

                bRightAngle = CMATH.similarEst_float(Vector3.Angle(v3DirEdge01, v3DirEdge02), 90.0f, fEstRightAngle);

                if (true == bRightAngle)
                {
                    if (CMATH.similarEst_float(_arrEdgeLength[1], _arrEdgeLength[2], fEstIsosceles))
                    {
                        bIsoscelesRightAngleTriangle = true;
                        _triedge_Hypotenuse = E_TRI_EDGE.E_TRI_EDGE_01;
                        return true;
                    }
                }
            } // if (true == bRightAngle)
        } // if (true == bRightAngle)

        return false;
    } // public bool IsoscelesRightAngleTriangle(float fRate)
    //@ Isosceles-RightAngle Triangle

    // Rarely It's same, despite of different point of triangle obviously.
    public bool isValidateASPoly()
    {
        for (int iPnt = 0; iPnt < CNTPNTTRI; ++iPnt)
        {
            int iVCurr = _arriIV[iPnt];
            for (int iPnt__ = 0; iPnt__ < CNTPNTTRI; ++iPnt__)
            {
                if (iPnt == iPnt__)
                {
                    continue;
                }

                int iVCurr__ = _arriIV[iPnt__];

                if (iVCurr == iVCurr__)
                {
                    return false;
                }
            }
        } // for(int iPnt=0; iPnt<CNTPNTTRI; ++iPnt)

        return true;
    }

    public bool inclinedPlane()
    {
        float fDifferHeight = 0.0f;

        for (int iSeqPnt = 1; iSeqPnt < _arrv3PT.Length; ++iSeqPnt)
        {
            fDifferHeight = fDifferHeight + Mathf.Abs(_arrv3PT[iSeqPnt].y - _arrv3PT[iSeqPnt-1].y);
        }

        return (fDifferHeight > CMATH.FEPSILON_F3);
    } // public bool inclinedPlane()

    public bool isEqual(CTRI rValue)
    {
        bool bEqual = true;
        for (int iPnt = 0; iPnt < CNTPNTTRI; ++iPnt)
        {
            if (_arriIV[iPnt] != rValue._arriIV[iPnt])
            {
                bEqual = false;
                break;
            }
        }

        if (true == bEqual)
        {
            return true;
        }

        bEqual = true;
        for (int iPnt = 0; iPnt < CNTPNTTRI; ++iPnt)
        {
            int iPnt_ = (iPnt + 1) % 3;
            if (_arriIV[iPnt] != rValue._arriIV[iPnt_])
            {
                bEqual = false;
                break;
            }
        }

        if (true == bEqual)
        {
            return true;
        }

        bEqual = true;
        for (int iPnt = 0; iPnt < CNTPNTTRI; ++iPnt)
        {
            int iPnt_ = (iPnt + 2) % 3;
            if (_arriIV[iPnt] != rValue._arriIV[iPnt_])
            {
                bEqual = false;
                break;
            }
        }

        return bEqual;
    }

    public void GetCenterTri(ref Vector3 v3Center)
    {
        v3Center = (_arrv3PT[0] + _arrv3PT[1] + _arrv3PT[2]) / 3;
    }

    public Vector3 GetCenterTri()
    {
        return (_arrv3PT[0] + _arrv3PT[1] + _arrv3PT[2]) / 3;
    }

    public CTRI()
    {

    }
} // CTRI
