using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum E_TYPE_SPLINE
{
    SPLINE_NULL = -1,
    SPLINE_ROUNDEDNONUNIFORMSPLINE = 0,
    SPLINE_SMOOTHNONUNIFORMSPLINE,
    SPLINE_TIMEDUNIFORMSPLINE,
    SPLINE_END,
}

public class CSplineGenerator
{
    public class ICompareCurvePath : IComparer<CCurvePathUnit>
    {
        //@ Compare for sort : return -1=>Right is greator, return 1=>Left is greator.
        public int Compare(CCurvePathUnit pathUnitLeft, CCurvePathUnit pathUnitRight)
        {
            int iSeqLeft = pathUnitLeft.GetSeqPathunit();
            int iSeqRight = pathUnitRight.GetSeqPathunit();

            if (iSeqLeft == iSeqRight)
            {
                return 0;
            }

            if (iSeqLeft < iSeqRight)
            {
                return -1;
            }

            return 1;
        } // public int Compare(CCurvePathUnit pathUnitLeft, CCurvePathUnit pathUnitRight)
    }

    #region constparameter
    public static readonly E_TYPE_SPLINE DEFLT_TYPE_SPLINECURVE = E_TYPE_SPLINE.SPLINE_ROUNDEDNONUNIFORMSPLINE;
    public static readonly float DFLT_WEIGHT_TIMEDIVIDE = 6.0f;
    public static readonly float multiply_adjust_divisionWeight = 0.5f;
    #endregion // #region constparameter

    //@ Index
    int m_iSeqProcessorSpline;

    //@ PathUnit
    [System.NonSerialized]
    public List<CCurvePathUnit> m_listCurvepathunit = new List<CCurvePathUnit>();

    [System.NonSerialized]
    public List<Vector3> m_listPath_Spline = new List<Vector3>();

    ICompareCurvePath m_ICompCurvePath = new ICompareCurvePath();

    //@ SPLINE 
    Vector3[] m_arrSectionCurve = null;
    RNS m_ProcessCurve_Activate = null;
    E_TYPE_SPLINE m_typeSplineCurve = E_TYPE_SPLINE.SPLINE_NULL;
    bool m_bBuildedCurve = false;

    float m_fDivisionWeight_LineCurve = 4.0f;

    //@ Get/Set
    public int getSeqProcessorSpline()
    {
        return m_iSeqProcessorSpline;
    }

    public void setSeqProcessorSpline(int iSeqSplineGenerator)
    {
        m_iSeqProcessorSpline = iSeqSplineGenerator;
    }

    public E_TYPE_SPLINE GetTypeSplineCurve()
    {
        return m_typeSplineCurve;
    }

    public void SetTypeSplineCurve(E_TYPE_SPLINE typeSplineCurve)
    {
        m_typeSplineCurve = typeSplineCurve;
    }

    public float GetDivisionWeight()
    {
        return m_fDivisionWeight_LineCurve;
    }

    public void SetDivisionWeight(float fDivisionWeight_LineCurve)
    {
        m_fDivisionWeight_LineCurve = fDivisionWeight_LineCurve;
    }

    public bool IsBuildedCurve()
    {
        return m_bBuildedCurve;
    }

    public int GetCntPathSpline()
    {
        return m_listPath_Spline.Count;
    }

    public Vector3 [] getSectionSpline()
    {
        if (null == m_arrSectionCurve)
        {
            return null;
        }
        if(m_arrSectionCurve.Length < 1)
        {
            Debug.Log("getSectionSpline()//if(m_arrSectionCurve.Length < 1)" + UnityEngine.Random.Range(0.0f, 10000.0f));
            return null;
        }

        return m_arrSectionCurve;
    }

    public Vector3 [] getPathSpline()
    {
        return m_listPath_Spline.ToArray();
    } 

    //@ Just 2 more Point of Instant Curve Position, and then It consider ready to build positive. (Is it right 3 more?)
    public bool IsReadytoBuild()
    {
        return (m_listPath_Spline.Count > 1);
    }

    public void SetVisibleAllCurvePath(bool bVisibleRequestCurvePath)
    {
        foreach (CCurvePathUnit curvePathUnit in m_listCurvepathunit)
        {
            if (curvePathUnit)
                curvePathUnit.setVisibleCurvePath(bVisibleRequestCurvePath);
        }
    }

    //@ Set Unselect All CurvePath
    public void SetUnselectAllCurvePath()
    {
        foreach (CCurvePathUnit pathUnit in m_listCurvepathunit)
        {
            if(pathUnit)
                pathUnit.setUnselectFlyPathUnit();
        }
    }

    //@ Set select All CurvePath
    public void SetSelectAllCurvePath()
    {
        for(int iSeqUnit = 0; iSeqUnit < m_listCurvepathunit.Count; ++iSeqUnit)
        {
            CCurvePathUnit pathUnit = m_listCurvepathunit[iSeqUnit];
            pathUnit.setSelectFlyPathUnit(iSeqUnit);
        }
    }

    //@ Delete Curve Current, not destroy instance.
    public bool DeleteCurvePath(CCurvePathUnit curvepathUnitDel)
    {
        if(m_listCurvepathunit.Count < 1)
        {
            return false;
        }

        if (null == curvepathUnitDel)
        {
            return false;
        }

        int iSeqPathDel = -1;
        for (int iSeqPath = 0; iSeqPath < m_listCurvepathunit.Count; ++iSeqPath)
        {
            CCurvePathUnit flypathunitCurr = m_listCurvepathunit[iSeqPath];
            if (curvepathUnitDel == flypathunitCurr)
            {
                iSeqPathDel = iSeqPath;
                break;
            }
        }

        if (-1 != iSeqPathDel)
        {
            m_listCurvepathunit.RemoveAt(iSeqPathDel);
            m_listPath_Spline.RemoveAt(iSeqPathDel);
        }

        return true;
    } // public bool DeleteCurvePath(CCurvePathUnit curvepathUnitDel)

    //@ Draw Path
    protected void ClearCurvePathInstant()
    {
        foreach (CCurvePathUnit curvepathunit in m_listCurvepathunit)
        {
            if (curvepathunit)
                GameObject.Destroy(curvepathunit.gameObject);
        }
        m_listCurvepathunit.Clear();

        m_listPath_Spline.Clear();
    }

    //@ Process // Activate or Unactivate
    public void RegisterOrUnregisterCurvePath_spline(CCurvePathUnit flypathPoint)
    {
        if (null == flypathPoint)
        {
            return;
        }

        if (m_listCurvepathunit.Count > 0)
        {
            int iIdxFind = findCurvePoint(flypathPoint);

            if (-1 < iIdxFind)
            {
                flypathPoint.setUnselectFlyPathUnit();
                DeleteCurvePath(flypathPoint);
                return;
            }
            else
            {
                if (true == flypathPoint.getSelect())
                {
                    flypathPoint.setUnselectFlyPathUnit();
                }
                else
                {
                    int iSeqpath = m_listCurvepathunit.Count;
                    flypathPoint.setSelectFlyPathUnit(iSeqpath);
                }
            } 
        } 
        else
        {
            int iSeqpath = m_listCurvepathunit.Count;
            flypathPoint.setSelectFlyPathUnit(iSeqpath);
        } 

        m_listCurvepathunit.Add(flypathPoint);
        m_listCurvepathunit.Sort(m_ICompCurvePath);

        SetNewCurvePoint_spline(flypathPoint.getPosUnit());

        return;
    } // public bool SetNewCurvePoint_spline(CCurvePathUnit flypathPoint)

    //@ Process // SetNew
    public bool SetNewCurvePoint_spline(Vector3 v3PntCurveInstant)
    {
        m_listPath_Spline.Add(v3PntCurveInstant);

        return true;
    } 

    protected int findCurvePoint(CCurvePathUnit flypathPoint)
    {
        return m_listCurvepathunit.BinarySearch(flypathPoint, m_ICompCurvePath);
    }

    //@ Process // Initialize()
    public void InitializeSplineGenerator( int iSeqSplineGenerator )
    {
        if(null!=m_arrSectionCurve)
        {
            Array.Clear(m_arrSectionCurve, 0, m_arrSectionCurve.Length);
        }

        m_bBuildedCurve = false;

        setSeqProcessorSpline(iSeqSplineGenerator);
    }

    //@ Process // BuildupCurve_spline()
    public bool BuildupCurve_spline(E_TYPE_SPLINE eTypeCurve, float fDivisionWeight)
    {
        if (m_listCurvepathunit.Count > 0)
        {
            m_listPath_Spline.Clear();

            foreach (CCurvePathUnit flypathUnit in m_listCurvepathunit)
            {
                if (flypathUnit)
                    m_listPath_Spline.Add(flypathUnit.getPosUnit());
            }
        }

        if (m_listPath_Spline.Count < 1)
        {
            Debug.Log("Cant Buildup CurveList. no Instant Custom Point!!//");
            return false;
        }

        float fDivisionWeight__ = -1.0f;
        if(fDivisionWeight < 0)
        {
            fDivisionWeight__ = m_fDivisionWeight_LineCurve;
        }
        else
        {
            fDivisionWeight__ = m_fDivisionWeight_LineCurve = fDivisionWeight;
        } // if(fDivisionWeight < 0)


        if (E_TYPE_SPLINE.SPLINE_NULL == eTypeCurve)
        {
            eTypeCurve = m_typeSplineCurve;

            if(E_TYPE_SPLINE.SPLINE_NULL == eTypeCurve)
            {
                eTypeCurve = DEFLT_TYPE_SPLINECURVE;
            }
        } // if (E_TYPE_SPLINE.SPLINE_NULL == eTypeCurve)

        m_typeSplineCurve = eTypeCurve;

        bool bBuildCurve = false;
        switch(m_typeSplineCurve)
        {
            case E_TYPE_SPLINE.SPLINE_ROUNDEDNONUNIFORMSPLINE:
            {
                RNS __ProcessCurve_rns = new RNS();
                foreach (Vector3 v3PntCurve in m_listPath_Spline)
                {
                    __ProcessCurve_rns.AddNode(v3PntCurve);
                }

                bBuildCurve = __ProcessCurve_rns.BuildSpline_rns();
                if(false==bBuildCurve)
                {
                    Debug.Log("FALSE !! __ProcessCurve_rns.BuildSpline_rns()");
                    return false;
                }

                m_ProcessCurve_Activate = __ProcessCurve_rns;
            }
            break;
            case E_TYPE_SPLINE.SPLINE_SMOOTHNONUNIFORMSPLINE:
            {
                SNS __ProcessCurve_sns = new SNS();

                foreach (Vector3 v3PntCurve in m_listPath_Spline)
                {
                    __ProcessCurve_sns.AddNode(v3PntCurve);
                }

                bBuildCurve = __ProcessCurve_sns.BuildSpline_sns();
                if(false==bBuildCurve)
                {
                    Debug.Log("FALSE !! __ProcessCurve_sns.BuildSpline_sns()");
                    return false;
                }

                m_ProcessCurve_Activate = __ProcessCurve_sns;
            }
            break;
            case E_TYPE_SPLINE.SPLINE_TIMEDUNIFORMSPLINE:
            {
                TNS __ProcessCurve_tns = new TNS();

                foreach (Vector3 v3PntCurve in m_listPath_Spline)
                {
                    __ProcessCurve_tns.AddNode(v3PntCurve);
                }

                bBuildCurve = __ProcessCurve_tns.BuildSpline_tns();

                if(false==bBuildCurve)
                {
                    Debug.Log("FALSE !! __ProcessCurve_tns.BuildSpline_tns()");
                    return false;
                }

                m_ProcessCurve_Activate = __ProcessCurve_tns;
            }
            break;
            default:
            {
                Debug.Log("m_typeSplineCurve is not valid type.");

                m_ProcessCurve_Activate = null;
                return false;
            }
        } // switch(m_typeSplineCurve)

        int iCntTime_Division = (int)(m_ProcessCurve_Activate.m_fmaxDistance * fDivisionWeight__ * multiply_adjust_divisionWeight);

        m_arrSectionCurve = new Vector3[iCntTime_Division];

        //@ First, Last Position are we just knew. 
        m_arrSectionCurve[0] = m_listPath_Spline[0];
        m_arrSectionCurve[iCntTime_Division - 1] = m_listPath_Spline[m_listPath_Spline.Count - 1];

        for (int iSeq = 1; iSeq <= iCntTime_Division-1; ++iSeq)
        {
            float fT = (float)iSeq / (float)iCntTime_Division;

            Vector3 v3PosNew_Curve = new Vector3();
            v3PosNew_Curve = m_ProcessCurve_Activate.GetPositionSpline(fT);
            m_arrSectionCurve[iSeq] = v3PosNew_Curve;
        }

        m_bBuildedCurve = true;

        return true;
    } // public bool BuildupCurve_spline(E_TYPE_SPLINE eTypeCurve)

    //@ Process // ClearCurveLine_spline
    public bool ClearCurveLine_spline()
    {
        if (false == m_bBuildedCurve && m_listCurvepathunit.Count < 1)
        {
            return false;
        } 

        m_bBuildedCurve = false;

        if (null != m_ProcessCurve_Activate)
        {
            m_ProcessCurve_Activate.ReleaseSpline();
        }

        return true;
    }


    //@ Process // Release_ProcessorSpline
    public bool Release_ProcessorSpline()
    {
        m_bBuildedCurve = false;

        if(null!=m_ProcessCurve_Activate)
        {
            m_ProcessCurve_Activate.ReleaseSpline();
            m_ProcessCurve_Activate = null;
        }

        ClearCurvePathInstant();

        if (null!=m_arrSectionCurve)
        {
            Array.Clear(m_arrSectionCurve, 0, m_arrSectionCurve.Length);
            m_arrSectionCurve = null;
        }

        return true;
    } // public bool Release_ProcessorSpline()


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
} // public class CSplineGenerator : MonoBehaviour
