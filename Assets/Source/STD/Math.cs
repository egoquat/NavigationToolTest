using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CMATH
{
    //@ INFINITE VALUE
    public static readonly int INT_MAX = int.MaxValue;
    public static readonly int INT_MIN = int.MinValue;
    public static readonly float FLOAT_MAX = float.MaxValue;
    public static readonly float FLOAT_MIN = float.MinValue;

    //@ USER INDEX
    public static readonly int INT_MAX_DIV2 = INT_MAX / 2;
    public static readonly int INT_MAX_DIV4 = INT_MAX / 4;
    public static readonly int INT_MAX_DIV8 = INT_MAX / 8;
    public static readonly int INT_MAX_DIV16 = INT_MAX / 16;
    public static readonly int INT_MAX_DIV32 = INT_MAX / 32;
    public static readonly int INT_MAX_DIV2_4 = INT_MAX_DIV2 + INT_MAX_DIV4;
    public static readonly int INT_MAX_DIV2416 = INT_MAX_DIV2_4 + INT_MAX_DIV16;
    public static readonly int INT_MAX_DIV2_8 = INT_MAX_DIV2 + INT_MAX_DIV8;
    public static readonly int INT_MAX_DIV2816 = INT_MAX_DIV2_8 + INT_MAX_DIV16;
    public static readonly int INT_MAX_DIV4_8 = INT_MAX_DIV4 + INT_MAX_DIV8;
    public static readonly int INT_MAX_DIV248 = INT_MAX_DIV2_8 + INT_MAX_DIV4;
    public static readonly int INT_MAX_DIV3216 = INT_MAX_DIV32 + INT_MAX_DIV16;
    public static readonly int INT_MAX_DIV32168 = INT_MAX_DIV32 + INT_MAX_DIV16 + INT_MAX_DIV8;

    //@ EPSILON
    public static readonly float FEPSILON_F6 = 0.000001f;
    public static readonly float FEPSILON_F5 = 0.00001f;
    public static readonly float FEPSILON_F4 = 0.0001f;
    public static readonly float FEPSILON_F3 = 0.001f;
    public static readonly float FEPSILON_F2 = 0.01f;
    public static readonly float FEPSILON_F1 = 0.1f;

    public static bool similarEst_float(float fNum, float fCompare, float fEst)
    {
        return (Mathf.Abs(fNum - fCompare) <= fEst);
    }

    //@ Compare
    public static bool similarFloat_Epsilon_F3(float f01, float f02)
    {
        return (FEPSILON_F3 > Mathf.Abs(f02 - f01));
    }

    public static bool similarFloat_Epsilon_F2(float f01, float f02)
    {
        return (FEPSILON_F2 > Mathf.Abs(f02 - f01));
    }

    public static bool similarFloat_Epsilon_FIn(float f01, float f02, float fEpsilon)
    {
        return (fEpsilon > Mathf.Abs(f02 - f01));
    }

    public static bool similarVector3_f2(Vector3 v301, Vector3 v302)
    {
        bool bSimilar = true;

        bSimilar &= similarFloat_Epsilon_F2(v301.x, v302.x);
        bSimilar &= similarFloat_Epsilon_F2(v301.y, v302.y);
        bSimilar &= similarFloat_Epsilon_F2(v301.z, v302.z);

        return bSimilar;
    }

    public static bool similarVector3_FIn(Vector3 v301, Vector3 v302, float fEpsilon)
    {
        bool bSimilar = true;

        bSimilar &= similarFloat_Epsilon_FIn(v301.x, v302.x, fEpsilon);
        bSimilar &= similarFloat_Epsilon_FIn(v301.y, v302.y, fEpsilon);
        bSimilar &= similarFloat_Epsilon_FIn(v301.z, v302.z, fEpsilon);

        return bSimilar;
    }

    public static Vector3 V3Multiply(Vector3 v3Left, Vector3 v3Right)
    {
        return new Vector3(v3Left.x * v3Right.x, v3Left.y * v3Right.y, v3Left.z * v3Right.z);
    }

    public static Vector3 direction_normalize(Vector3 v3Pos_From, Vector3 v3Pos_To)
    {
        return (v3Pos_To - v3Pos_From).normalized;
    }

    public static Vector3 MultiplyAngleToDir(Vector3 dirSrc, Vector3 AngleDeg)
    {
        Quaternion quatAngle = Quaternion.Euler(AngleDeg);
        return quatAngle * dirSrc;
    }

    public static bool overBasePosition2D(Vector2 v2Posbase, Vector2 v2PosbaseDest, Vector2 v2Posother)
    {
        Vector2 v2DirBase, v2DirOther;

        v2DirBase = v2Posbase - v2PosbaseDest;
        v2DirOther = v2Posother - v2PosbaseDest;

        if (v2DirOther == Vector2.zero)
        {
            Debug.Log("Critical ERROR!! (v2DirOther==Vector3.zero)//crossOverDest//");
        }

        if (0 > Vector2.Dot(v2DirBase, v2DirOther))
        {
            return true;
        }

        return false;
    }

    ////TEST
    //public static int iComputeOnRight = 0;
    //public static int iComputeOnOpposite = 0;

    public static bool findposIntersected(  Vector3 posLine0, 
                                            Vector3 posLine1, 
                                            Vector3 posPlane0, 
                                            Vector3 posPlane1, 
                                            Vector3 posPlane2, 
                                            bool boundarycheck,
                                            ref Vector3 posIntersected)
    {
        Vector3 vectorPlane0 = (posPlane0 - posPlane2);
        Vector3 vectorPlane1 = (posPlane1 - posPlane2);

        Vector3 dirplanenormal = Vector3.Cross(vectorPlane0, vectorPlane1).normalized;
        
        float fdotN0 = Vector3.Dot((posLine1-posLine0), dirplanenormal);
        float fdotN1 = Vector3.Dot((posLine0-posLine1), dirplanenormal);

        //경계면과 완전한 평행인경우.
        if(0.0f == fdotN0 && 0.0f == fdotN1)
        {
            return false;
        }

        bool bonright = (fdotN0 > 0.0f && fdotN1 < 0.0f);
        bool bonleft = (fdotN0 < 0.0f && fdotN1 > 0.0f);
        bool bcross = !(false == bonright && false == bonleft);
        if(false == bcross)
        {
            Debug.Log("--findposIntersected not crossover." + "//" + UnityEngine.Random.Range(0.0f, 10.0f));

            return false;
        }
        
        Vector3 dirline, posLineStart;
        float fposDotN = 0.0f;
        float fdistLine = 0.0f;

        if(true == bonright)
        {
            fdistLine = (posLine1-posLine0).magnitude;
            dirline = (posLine1-posLine0)/fdistLine;
            fposDotN = Vector3.Dot((posLine0), dirplanenormal);
            posLineStart = posLine0;

            //TEST
            //iComputeOnRight++;
        }
        else
        {
            fdistLine = (posLine0-posLine1).magnitude;
            dirline = (posLine0-posLine1)/fdistLine;
            fposDotN = Vector3.Dot((posLine1), dirplanenormal);
            posLineStart = posLine1;

            //TEST
            //iComputeOnOpposite++;
        }

        float fdirDotN = Vector3.Dot(dirline, dirplanenormal);
        float f_D = -Vector3.Dot(posPlane0,dirplanenormal);

        float fdistintersected = -(fposDotN + f_D) / fdirDotN;

        if (true == boundarycheck)
        {
            if (0 == fdistintersected)
            {
                return false;
            }

            if (fdistLine < fdistintersected)
            {
                return false;
            }
        }

        posIntersected = posLineStart + (dirline * fdistintersected);
        return true;
    }

    public static bool crossOverDest(Vector3 posStaticStart, Vector3 posStaticEnd, Vector3 posDynamic)
    {
        Vector3 v3DirBase, v3DirOther;

        v3DirBase = posStaticStart - posStaticEnd;
        v3DirOther = posDynamic - posStaticEnd;

        if (v3DirOther == Vector3.zero)
        {
            Debug.Log("Critical ERROR!! (v3DirOther==Vector3.zero)//crossOverDest//" + UnityEngine.Random.Range(0.0f, 100.0f));
        }

        if (0 > Vector3.Dot(v3DirBase, v3DirOther))
        {
            return true;
        }

        return false;
    } 

    //@ Line Cross only Horizontal Space // Height not consideration. 
    public static bool lineCross2D(  Vector3 v3Pos01_start, 
                                                    Vector3 v3Pos01_end, 
                                                    Vector3 v3Pos02_start, 
                                                    Vector3 v3Pos02_end )
    {
        Vector2 v2Pos0_start, v2Pos0_end, v2Pos1_start, v2Pos1_end;

        v2Pos0_start = new Vector2(v3Pos01_start.x, v3Pos01_start.z);
        v2Pos0_end = new Vector2(v3Pos01_end.x, v3Pos01_end.z);
        v2Pos1_start = new Vector2(v3Pos02_start.x, v3Pos02_start.z);
        v2Pos1_end = new Vector2(v3Pos02_end.x, v3Pos02_end.z);

        float fReturn = 0.0f;
        return lineCross2D(v2Pos0_start, v2Pos0_end, v2Pos1_start, v2Pos1_end, ref fReturn);
    }

    public static bool lineCross2D_AxisY(Vector3 v3Pos0_start,
                                        Vector3 v3Pos0_end,
                                        Vector3 v3Pos1_start,
                                        Vector3 v3Pos1_end,
                                        ref Vector3 v3PosIntersected)
    {
        Vector2 v2Pos0_start = new Vector2(v3Pos0_start.x, v3Pos0_start.z);
        Vector2 v2Pos0_end = new Vector2(v3Pos0_end.x, v3Pos0_end.z);
        Vector2 v2Pos1_start = new Vector2(v3Pos1_start.x, v3Pos1_start.z);
        Vector2 v2Pos1_end = new Vector2(v3Pos1_end.x, v3Pos1_end.z);

        float fIntersectedRatio = 0.0f;
        bool bResultofIntersected = lineCross2D(v2Pos0_start, v2Pos0_end, v2Pos1_start, v2Pos1_end, ref fIntersectedRatio);
        if (false == bResultofIntersected)
        {
            return false;
        }

        v3PosIntersected = v3Pos0_start + ((v3Pos0_end - v3Pos0_start).normalized * fIntersectedRatio);
        return true;
    }

    public static bool lineCross2D(Vector2 v2Pos0_start,
                                        Vector2 v2Pos0_end,
                                        Vector2 v2Pos1_start,
                                        Vector2 v2Pos1_end,
                                        ref float fIntersected )
    {
        // Denominator for ua and ub are the same, so store this calculation
        float d =
            (v2Pos1_end.y - v2Pos1_start.y) * (v2Pos0_end.x - v2Pos0_start.x)
             -
            (v2Pos1_end.x - v2Pos1_start.x) * (v2Pos0_end.y - v2Pos0_start.y);

        //n_a and n_b are calculated as seperate values for readability
        float n_a =
            (v2Pos1_end.x - v2Pos1_start.x) * (v2Pos0_start.y - v2Pos1_start.y)
             -
            (v2Pos1_end.y - v2Pos1_start.y) * (v2Pos0_start.x - v2Pos1_start.x);

        float n_b =
            (v2Pos0_end.x - v2Pos0_start.x) * (v2Pos0_start.y - v2Pos1_start.y)
             -
            (v2Pos0_end.y - v2Pos0_start.y) * (v2Pos0_start.x - v2Pos1_start.x);

        // Make sure there is not a division by zero - this also indicates that
        // the lines are parallel.  (the parallel check accounts for this).
        if (d == 0)
            return false;

        // Calculate the intermediate fractional point that the lines potentially intersect.
        float ua = n_a / d;
        float ub = n_b / d;

        // The fractional point will be between 0 and 1 inclusive if the lines
        // intersect.  If the fractional calculation is larger than 1 or smaller
        // than 0 the lines would need to be longer to intersect.
        if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
        {
            fIntersected = ua;
            return true;
        }

        return false;
    } // public static bool lineCross2D

    public static bool lineCross2DPos(Vector2 v2Pos0_start,
                                                    Vector2 v2Pos0_end,
                                                    Vector2 v2Pos1_start,
                                                    Vector2 v2Pos1_end,
                                                    ref Vector2 v2PosIntersected)
    {
        float fIntersected = 0.0f;
        if(lineCross2D(v2Pos0_start, v2Pos0_end, v2Pos1_start, v2Pos1_end, ref fIntersected))
        {
            v2PosIntersected.x = v2Pos0_start.x + (fIntersected * (v2Pos0_end.x - v2Pos0_start.x));
            v2PosIntersected.y = v2Pos0_start.y + (fIntersected * (v2Pos0_end.y - v2Pos0_start.y));
            return true;
        }

        return false;
    } // public static bool lineCross2DPos

    //@ Simple Adjust Size // 함수 재검증 필요, 소소한 결과 차이.
    public static void rescaleVertices(ref Vector3[] arrv3VB_, float fScaleAdj)
    {
        Vector3 v3Summary = new Vector3(0.0f, 0.0f ,0.0f);
        foreach (Vector3 v3VBPnt in arrv3VB_)
        {
            v3Summary += v3VBPnt;
        }

        Vector3 v3Centerof = v3Summary / arrv3VB_.Length;

        rescaleVertices(ref arrv3VB_, v3Centerof, fScaleAdj);
    }

    //@ Simple Adjust Size
    public static void rescaleVertices(ref Vector3[] arrv3VB_, Vector3 v3Center, float fScaleAdj)
    {
        int iSeq = 0;
        foreach (Vector3 v3VBPNT in arrv3VB_)
        {
            arrv3VB_[iSeq++] = ((v3VBPNT - v3Center) * fScaleAdj) + v3Center;
        }
    } 

    //@ Convert Vector3 to Vector2 based on some environment
    public static Vector2 ConvertToV2_Y(Vector3 v3Src)
    {
        Vector2 v2Convert = new Vector2();

        v2Convert.x = v3Src.x;
        v2Convert.y = v3Src.z;
        return v2Convert;
    }

    //@ Triangle
    public static bool IntersectRayTriSimple(Vector3 v3RayOrigin, Vector3 v3RayDir, Vector3 v3T_0, Vector3 v3T_1, Vector3 v3T_2)
    {
        if (Mathf.Abs(1.0f - v3RayDir.magnitude) < FEPSILON_F5)
        {
            Debug.Log("Error. IntersectRayTri//if (Mathf.Abs(1.0f - v3RayDir.magnitude) < FEPSILON_F5)//" + UnityEngine.Random.Range(10.0f, 10000.0f));
        }

        // e1 = v1 - v0, e2 = v2 - v0
        Vector3 v3Edge1 = v3T_1 - v3T_0;
        Vector3 v3Edge2 = v3T_2 - v3T_0;

        if (v3Edge1 == v3Edge2)
        {

            return false;
        }

        if (v3Edge1.magnitude == 0.0f || v3Edge2.magnitude == 0.0f)
        {

            return false;
        }

        Vector3 v3Temp;
        v3Temp = Vector3.Cross(v3RayDir, v3Edge2);
        // det 구하기
        float fDet = Vector3.Dot(v3Edge1, v3Temp);

        float fInverseOfDet;
        Vector3 wvQ;

        if (FEPSILON_F5 > Mathf.Abs(fDet))
        {
            return false;
        }
        fInverseOfDet = 1.0f / fDet;
        // s = o - v0
        Vector3 wvS = v3RayOrigin - v3T_0;
        // U 값 구하기
        float fU = Vector3.Dot(wvS, v3Temp) * fInverseOfDet;
        if (fU < 0.0f || fU > 1.0f)
        {
            return false;
        }

        // q = s x e1
        wvQ = Vector3.Cross(wvS, v3Edge1);
        // V 값 구하기
        float fV = Vector3.Dot(v3RayDir, wvQ) * fInverseOfDet;
        if (fV < 0.0f || (fU + fV) > 1.0f)
        {
            return false;
        }

        return true;
    }

    // 교차 검사 (외부/교차/포함) 교차지점 유
    public static bool IntersectRayTri_GetDist( Vector3 v3RayOrigin, Vector3 v3RayDir, Vector3 v3T_0, Vector3 v3T_1, Vector3 v3T_2, ref float fT )
    {        
	    // e1 = v1 - v0, e2 = v2 - v0
	    Vector3 v3Edge1 = v3T_1 - v3T_0;
	    Vector3 v3Edge2 = v3T_2 - v3T_0;

	    if( v3Edge1 == v3Edge2)
	    {

		    return false;
	    }

        if (v3Edge1.sqrMagnitude == 0.0f || v3Edge2.sqrMagnitude == 0.0f)
	    {
		    return false;
	    }

	    // p = d x e2, q = s x e1 ( s = o - v0) : q 는 뒤에서 구함
        Vector3 v3Temp = Vector3.Cross(v3RayDir, v3Edge2);
	    // det 구하기
        float fDet = Vector3.Dot(v3Edge1, v3Temp);

        if ( FEPSILON_F5 > Mathf.Abs(fDet))
	    {
		    return false;
	    }
	    float fInverseOfDet = 1.0f / fDet;
	    // s = o - v0
	    Vector3 wvS = v3RayOrigin - v3T_0;
	    // U 값 구하기
	    float fU = Vector3.Dot( wvS, v3Temp) * fInverseOfDet;
	    if ( fU < 0.0f || fU > 1.0f)
	    {
		    return false;
	    }
	    // q = s x e1
	    //CrossProduct_( wvS, v3Edge1, wvQ );
        Vector3 wvQ = Vector3.Cross(wvS, v3Edge1);
	    // V 값 구하기
	    float fV = Vector3.Dot( v3RayDir, wvQ) * fInverseOfDet;
	    if ( fV < 0.0f || ( fU + fV) > 1.0f)
	    {
		    return false;
	    }

	    // Ray 의 시작점에서부터의 거리
	    fT = Vector3.Dot( v3Edge2, wvQ) * fInverseOfDet;

	    return true;
    } // public static bool IntersectRayTri_GetDist

    // 교차 검사 (외부/교차/포함) 교차지점 유
    public static bool IntersectRayTri_GetPos(Vector3 v3RayOrigin, Vector3 v3RayDir, Vector3 [] arrv3Pos, ref Vector3 v3Intersected)
    {
        float fDistIntersected = 0.0f;
        if (false == IntersectRayTri_GetDist(v3RayOrigin, v3RayDir, arrv3Pos[0], arrv3Pos[1], arrv3Pos[2], ref fDistIntersected))
        {
            return false;
        }

        v3Intersected = v3RayOrigin + (v3RayDir * fDistIntersected);
        return true;
    } 

    public static Vector3 Bezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float mu)
    {
		Vector3 p;
		float mum = 1 - mu;
		float mum0 = mu * mu * mu;
		float mum1 = mum * mu * mu;
		float mum2 = mum * mum * mu;
		float mum3 = mum * mum * mum;
		p.x = mum3*p1.x + 3*mum2*p2.x + 3*mum1*p3.x + mum0*p4.x;
		p.y = mum3*p1.y + 3*mum2*p2.y + 3*mum1*p3.y + mum0*p4.y;
		p.z = mum3*p1.z + 3*mum2*p2.z + 3*mum1*p3.z + mum0*p4.z;
		return p;
	}
} // public class CMATH
