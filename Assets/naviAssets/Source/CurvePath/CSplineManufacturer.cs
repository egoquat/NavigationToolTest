using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Manager of Each spline processor
public class CSplineManufacturer
{
    public class curvesection
    {
        public Vector3 _posSectionStart;
        public Vector3 _posSectionDest;
        public Vector3 _dirtoNextP;
        public Vector3 _dirtoOppositeP;
        public float _distSqrSection;
        public float _distSection;

        public bool crossoverCurr(Vector3 posCurr)
        {
            Vector3 v3DirtoCurr = posCurr - _posSectionDest;

            float fDot = Vector3.Dot(v3DirtoCurr, _dirtoOppositeP);

            return (fDot <= 0.0f);
        }

        public void setcurvesection(Vector3 posstart, Vector3 posnext)
        {
            Vector3 vectorTonext = posnext - posstart;

            _distSection = (vectorTonext).magnitude;
            _distSqrSection = _distSection * _distSection;

            Vector3 dirTonext = vectorTonext / _distSection;

            _posSectionDest = new Vector3(posnext.x, posnext.y, posnext.z);
            _posSectionStart = new Vector3(posstart.x, posstart.y, posstart.z);
            _dirtoNextP = dirTonext;
            _dirtoOppositeP = dirTonext * -1.0f;
        }

        public void setcurvesection(curvesection sectionright)
        {
            _posSectionStart = new Vector3(sectionright._posSectionStart.x, sectionright._posSectionStart.y, sectionright._posSectionStart.z);
            _posSectionDest = new Vector3(sectionright._posSectionDest.x, sectionright._posSectionDest.y, sectionright._posSectionDest.z);
            _dirtoNextP = new Vector3(sectionright._dirtoNextP.x, sectionright._dirtoNextP.y, sectionright._dirtoNextP.z);
            _dirtoOppositeP = new Vector3(sectionright._dirtoOppositeP.x, sectionright._dirtoOppositeP.y, sectionright._dirtoOppositeP.z);

            _distSection = sectionright._distSection;
            _distSqrSection = sectionright._distSqrSection;
        }
    }

    #region constparameter
    public static readonly int DFLT_COUNT_COLOR_PATHLAYER = 9;
    public static readonly int DFLT_LIMIT_CNT_LINEGUILD = 999;
    #endregion // #region constparameter

    //@ Spline Curve Processor
    [System.NonSerialized]
    public List<CSplineGenerator> m_listProcessorSpline = new List<CSplineGenerator>();

    int m_iCurveActivate = -1;
    CSplineGenerator _splineGenerator_active = null;

    //@ TestDrawMesh
    List<CCurvePathUnit> m_listCurvePathUnit = new List<CCurvePathUnit>();
    List<CCurvePathLineDraw> m_listLineDraw = new List<CCurvePathLineDraw>();
    CSplineGenerator.ICompareCurvePath m_ICompCurvePath = new CSplineGenerator.ICompareCurvePath();
    Color[] m_arrColorLineDraw = null;

    #region staticparam_guideline_splinecurve

    //@ UnitFlying
    bool bConstructedGuildLine;
    public bool ConstructedBuildLine
    {
        get { return bConstructedGuildLine; }
        set { bConstructedGuildLine = value;}
    }

    //@Get/Set
    public curvesection[][] m_arrPosLineGuides = null;

    public curvesection[] getLineGuide(int iGuideLine)
    {
        if (0 > iGuideLine || m_arrPosLineGuides.Length <= iGuideLine)
        {
            Debug.Log("getLineGuide return null.iGuideLine(" + iGuideLine + ")");

            return null;
        }

        return m_arrPosLineGuides[iGuideLine];
    } 

    //@ Process
    public void InitSplieCurve()
    {
        if (true == processCycle.GetInstance._modeTool)
        {
            m_arrColorLineDraw = new Color[DFLT_COUNT_COLOR_PATHLAYER];

            m_arrColorLineDraw[0] = Color.Lerp(Color.red, Color.black, 0.3f);
            m_arrColorLineDraw[1] = Color.Lerp(Color.magenta, Color.black, 0.3f);
            m_arrColorLineDraw[2] = Color.Lerp(Color.yellow, Color.black, 0.3f);
            m_arrColorLineDraw[3] = Color.Lerp(Color.green, Color.black, 0.3f);
            m_arrColorLineDraw[4] = Color.Lerp(Color.blue, Color.black, 0.2f);
            m_arrColorLineDraw[5] = Color.Lerp(Color.blue, Color.black, 0.7f);
            m_arrColorLineDraw[6] = Color.Lerp(Color.red, Color.blue, 0.5f);
            m_arrColorLineDraw[7] = Color.Lerp(Color.cyan, Color.black, 0.3f);
        }
    }

    //@ New or insert line guild points.
    public bool newAssignLineGuideActive(int iSeqCurve)
    {
        if (iSeqCurve < 0 || DFLT_LIMIT_CNT_LINEGUILD <= iSeqCurve)
        {
            Debug.Log("(iSeqCurve < 0 || DFLT_LIMIT_CNT_LINEGUILD <= iSeqCurve)"
                        + UnityEngine.Random.Range(0.0f, 1000.0f));
            return false;
        }

        Vector3 [] _arrv3PosLineSection = getLineCurvePnt_Activate();

        bool bSeqLineGuide = newAssignLineguide__(iSeqCurve, _arrv3PosLineSection);
        return bSeqLineGuide;
    } //  void newConstructLineGuide(int iSeqCurve, Vector3[] _arrv3PosLineSection)

    //@ Process // After build up Spline Curve.
    bool newAssignLineguide__(int iSeqCurve, Vector3[] _arrv3PosLineSection)
    {
        if (null == _arrv3PosLineSection)
        {
            return false;
        }

        //@ list need at least 2 more points, because of set direction from to.
        if (_arrv3PosLineSection.Length > 1)
        {
            curvesection[] arrLineSection = new curvesection[_arrv3PosLineSection.Length-1];
            curvesection posSection;
            Vector3 v3Curr = Vector3.zero;
            Vector3 v3Next = Vector3.zero;
            int iLastLineGuide = _arrv3PosLineSection.Length - 1;

            for (int iSeqSP = 0; iSeqSP < iLastLineGuide; ++iSeqSP)
            {
                v3Curr = _arrv3PosLineSection[iSeqSP];
                v3Next = _arrv3PosLineSection[iSeqSP + 1];

                posSection = new curvesection();
                posSection.setcurvesection(v3Curr, v3Next);

                arrLineSection[iSeqSP] = posSection;
            }

            ConstructedBuildLine = true;

            if (null == m_arrPosLineGuides)
            {
                m_arrPosLineGuides = new curvesection[iSeqCurve + 1][];
            }

            if (iSeqCurve >= m_arrPosLineGuides.Length)
            {
                Array.Resize(ref m_arrPosLineGuides, iSeqCurve + 1);
            }

            m_arrPosLineGuides[iSeqCurve] = arrLineSection;

            return true;
        } // if (_arrv3PosLineSection.Length > 1)
        else
        {
            Debug.Log("newAssignLineguide__ need at least 2 points.//");
            ConstructedBuildLine = false;
        } // if (_arrv3PosLineSection.Length > 1)

        return false;
    } //  bool newAssignLineguide__(int iSeqCurve, Vector3[] _arrv3PosLineSection )

    private void ClearCurve(int iSeqCurve)
    {
        if (m_arrPosLineGuides.Length <= iSeqCurve)
        {
            return;
        }

        if (null != m_arrPosLineGuides)
        {
            curvesection[] arrPosLine = m_arrPosLineGuides[iSeqCurve];

            if (null != arrPosLine && 0 < arrPosLine.Length)
            {
                Array.Clear(arrPosLine, 0, arrPosLine.Length);
            }
        }
    } // private void ClearCurve(int iSeqCurve)

     void ClearCurveGuidelineAll()
    {
        if (null != m_arrPosLineGuides)
        {
            Array.Clear(m_arrPosLineGuides, 0, m_arrPosLineGuides.Length);
        }

        m_arrPosLineGuides = null;

        ConstructedBuildLine = false;
    } // public void ClearCurveGuidelineAll()

    #endregion // staticparam_guideline_splinecurve

    public void LoadCurvePath_CMapTemplate(List<CSplineGenerator> flyPath)
    {
        int iSeqpath = 0;
        foreach( CSplineGenerator path in flyPath)
        {
            int type = (int)path.GetTypeSplineCurve();
            bool bRequestFromTool = processCycle.GetInstance._modeTool;
            float divideWeight = path.GetDivisionWeight();
            int pathSize = path.m_listPath_Spline.Count;
            

            if (path.m_listPath_Spline.Count < 1)
                continue;

            if (divideWeight < 0)
            {
                divideWeight = CSplineGenerator.DFLT_WEIGHT_TIMEDIVIDE;
            }

            setNewProcessorSpline();
            
            for (int i = 0; i < pathSize; ++i)
            {
                Vector3 pnt = new Vector3(path.m_listPath_Spline[i].x, path.m_listPath_Spline[i].y, path.m_listPath_Spline[i].z);

                //@ Add PathUnit 
                if (true == bRequestFromTool)
                {
                    CCurvePathUnit flypathUnit = InsertNewCurvePathUnit_BLANK(pnt);

                    if (null != flypathUnit)
                    {
                        ActivateOrUnactivateCurvePoint(flypathUnit);
                    }
                    else
                    {
                        Debug.Log("(null==flypathUnit)//");
                    }
                }
                else
                {
                    ActivateCurvePosition(pnt);
                }
            }

            BuildupCurve(iSeqpath, (E_TYPE_SPLINE)type, divideWeight);

            iSeqpath++;
        } // foreach( CSplineGenerator path in flyPath)

        setActivitySpline(0, false);
           
    } // public void LoadCurvePath_CMapTemplate()


    //@ Insert / Delete
    //@ Set New Layer Processor Spline
    public int setNewProcessorSpline()
    {
        Set_Inactivate_Current(_splineGenerator_active);

        CSplineGenerator processorSplineNew = new CSplineGenerator();

        m_listProcessorSpline.Add(processorSplineNew);
        int iSequenceSpline= m_listProcessorSpline.Count - 1;
        processorSplineNew.InitializeSplineGenerator(iSequenceSpline);

        m_iCurveActivate = iSequenceSpline;
        _splineGenerator_active = processorSplineNew;

        return m_iCurveActivate;
    }


    //@ New CurvePoint
    public bool ActivateOrUnactivateCurvePoint(CCurvePathUnit flypathPoint)
    {
        if (null == _splineGenerator_active)
        {
            Debug.Log("Any spline curve does not selected." + "//" + UnityEngine.Random.Range(0.0f, 1000.0f));
            return false;
        }

        _splineGenerator_active.RegisterOrUnregisterCurvePath_spline(flypathPoint);

        return true;
    } 

    //@ New CurvePoint
    public bool ActivateCurvePosition(Vector3 flypathPosition)
    {
        if (null == _splineGenerator_active)
        {
            return false;
        }

        return _splineGenerator_active.SetNewCurvePoint_spline(flypathPosition);
    } 

    //@ Build New Curve
    public bool BuildupCurveCurrent(E_TYPE_SPLINE eTypeCurve, float fDivisionWeight)
    {
        _Clear_LineDraw_Pnts();
        bool bResultBuild = _splineGenerator_active.BuildupCurve_spline(eTypeCurve, fDivisionWeight);
        if (true == bResultBuild)
        {
            Vector3[] arrv3LineCurve = _splineGenerator_active.getSectionSpline();
            newAssignLineguide__(m_iCurveActivate, arrv3LineCurve);
        }

        return bResultBuild;
    }

    //@ Build New Curve
    public bool BuildupCurve(int iSeqSpline, E_TYPE_SPLINE eTypeCurve, float fDivisionWeight)
    {
        bool bResultBuild = m_listProcessorSpline[iSeqSpline].BuildupCurve_spline(eTypeCurve, fDivisionWeight);
        if (true == bResultBuild)
        {
            Vector3[] arrv3LineCurve = m_listProcessorSpline[iSeqSpline].getSectionSpline();
            newAssignLineguide__(iSeqSpline, arrv3LineCurve);
        }

        return bResultBuild;
    }

    //@ Build New Curve
    public bool BuildupCurveAll(E_TYPE_SPLINE eTypeCurve, float fDivisionWeight)
    {
        bool bBuildupall = true;
        int iSeqSpline = 0;
        foreach (CSplineGenerator splinegenerator in m_listProcessorSpline)
        {
            splinegenerator.BuildupCurve_spline(eTypeCurve, fDivisionWeight);
            Vector3[] arrv3LineCurve = splinegenerator.getSectionSpline();
            newAssignLineguide__(iSeqSpline, arrv3LineCurve);
            iSeqSpline++;
        }

        return bBuildupall;
    } 

    //@ Delete Curve
    public bool DeleteCurve(int iIdxCurve, bool bDrawLineMode)
    {
        if(m_listProcessorSpline.Count <= iIdxCurve || iIdxCurve < 0 )
        {
            return false;
        }

        ReleaseCurve_(_splineGenerator_active);
        m_listProcessorSpline.RemoveAt(iIdxCurve);

        setActivitySpline(0, bDrawLineMode);

        return true;
    }

    //@ Get/Set
    public int getCountSpline()
    {
        return m_listProcessorSpline.Count;
    }

    //@ Set Activity
    public int setActivitySpline(int iSeqSpline, bool bDrawLine)
    {
        if (iSeqSpline < 0 || m_listProcessorSpline.Count < 1)
        {
            return -1;
        }

        if (iSeqSpline >= m_listProcessorSpline.Count)
        {
            Debug.Log("Out of range Spline Select.  m_listProcessorSpline.Count="
                + m_listProcessorSpline.Count + "//"
                + "iSeqSpline=" + iSeqSpline + "////" + UnityEngine.Random.Range(0.0f, 10000.0f));

            return -1;
        } 

        Set_Inactivate_Current(_splineGenerator_active);

        m_iCurveActivate = iSeqSpline;

        if (true == bDrawLine)
        {
            _splineGenerator_active = m_listProcessorSpline[m_iCurveActivate];

            Set_activate_Current(_splineGenerator_active);
        }

        return m_iCurveActivate;
    } // public int setActivitySpline(int iLayerCurve)

    //@ Set Activate All curve
    public int setActivitySplines_ALL(bool bDrawLine)
    {
        if (m_listProcessorSpline.Count < 1)
        {
            return -1;
        }

        if (true == bDrawLine)
        {
            foreach (CSplineGenerator splineCurveCurr in m_listProcessorSpline)
            {
                Set_activate_Current(splineCurveCurr);
            }
        }

        return m_iCurveActivate;
    } 

    public void setUnActivitySplines_ALL()
    {
        if (m_listProcessorSpline.Count < 1)
        {
            return;
        }

        foreach (CSplineGenerator splineCurveCurr in m_listProcessorSpline)
        {
            Set_Inactivate_Current(splineCurveCurr);
        }

        Set_activate_Current(_splineGenerator_active);
    }

    //@ Get Activity Index
    public int getActivityProcessorSpline()
    {
        return m_iCurveActivate;
    }

    //@Get/Set
    //@ Builded Line Curve Points
    public Vector3[] getLineCurvePnt_Activate()
    {
        return _splineGenerator_active.getSectionSpline();
    }

    //@ Builded Curve Points
    public int GetCountPntCurve()
    {
        if(null==_splineGenerator_active)
        {
            return 0;
        }

        return _splineGenerator_active.GetCntPathSpline();
    }

    //@ Draw Line
    public void Set_DrawLineCurve(bool bDrawLineCurve)
    {
        if(true==bDrawLineCurve)
        {
            _Draw_LineCurvePnts( _splineGenerator_active );
        }
        else
        {
            _UnDraw_LineCurvePnts();
        }
    } 

    public void _Draw_LineCurvePnts(CSplineGenerator processSpline_)
    {
        if (null == processSpline_ || false == processSpline_.IsBuildedCurve() || null == m_arrColorLineDraw)
        {
            return;
        }

        Vector3[] arrPntCurveLine = processSpline_.getSectionSpline();
        Vector3 v3PntCurve;
        Quaternion qtRot = new Quaternion();
        qtRot.eulerAngles = Vector3.forward;

        Color colorLayerCurveLine = m_arrColorLineDraw[processSpline_.getSeqProcessorSpline()%9];

        for (int iSeqPnt = 0; iSeqPnt < arrPntCurveLine.Length; ++iSeqPnt)
        {
            v3PntCurve = arrPntCurveLine[iSeqPnt];
            CCurvePathLineDraw flylineDraw = (CCurvePathLineDraw)GameObject.Instantiate(processCycle.GetInstance.m_curvePathLineDraw_src, v3PntCurve, qtRot);
            flylineDraw.SetDiffuseColor(colorLayerCurveLine);

            m_listLineDraw.Add(flylineDraw);
        }
    } 

    public void _UnDraw_LineCurvePnts()
    {
        foreach (CCurvePathLineDraw pathLineDraw in m_listLineDraw)
        {
            if (pathLineDraw)
                GameObject.Destroy(pathLineDraw.gameObject);
        }

        m_listLineDraw.Clear();
    }

     void Set_activate_Current( CSplineGenerator processorSpline_activate )
    {
        if (null == processorSpline_activate)
        {
            return;
        }

        foreach (CSplineGenerator processorSpline in m_listProcessorSpline)
        {
            processorSpline.SetVisibleAllCurvePath(false);
        } 

        SetUnselectAll_CurvePathUnits();

        processorSpline_activate.SetVisibleAllCurvePath(true);
        processorSpline_activate.SetSelectAllCurvePath();

        //processorSpline_activate.BuildupCurve_spline(E_TYPE_SPLINE.SPLINE_NULL, -1);
        _Draw_LineCurvePnts( processorSpline_activate );
    }

    //@ Unselect LineCurvePnts
     void Set_Inactivate_Current(CSplineGenerator processorSpline_activate)
    {
        if (null == processorSpline_activate)
        {
            return;
        }
        
        SetUnselectAll_CurvePathUnits();

        _Clear_LineDraw_Pnts();
        processorSpline_activate.SetUnselectAllCurvePath();
        processorSpline_activate.SetVisibleAllCurvePath(false);
    }

    public CCurvePathUnit InsertNewCurvePathUnit_BLANK(Vector3 v3PointCurvePathUnit )
    {
        CCurvePathUnit flypathUnit = (CCurvePathUnit)GameObject.Instantiate(processCycle.GetInstance.m_curvePathUnit_src, v3PointCurvePathUnit, Quaternion.identity);
        flypathUnit.InitializeFlyPathUnit();

        m_listCurvePathUnit.Add(flypathUnit);
        m_listCurvePathUnit.Sort(m_ICompCurvePath);

        return flypathUnit;
    }

    public void DeleteCurvePathUnitInclude(CCurvePathUnit flypathunitDel)
    {
        if (null == flypathunitDel) return;

        foreach (CSplineGenerator processorSplineCurve in m_listProcessorSpline)
        {
            if (null != processorSplineCurve)
            {
                processorSplineCurve.DeleteCurvePath(flypathunitDel);
            }
        }

        DeleteCurvePathUnit(flypathunitDel);
    } 

    void DeleteCurvePathUnit(CCurvePathUnit flypathunitDel)
    {
        if (null == flypathunitDel) return;

        m_listCurvePathUnit.Remove(flypathunitDel);
    
        flypathunitDel.setUnselectFlyPathUnit();
        flypathunitDel.Release_curvepathUnit();

        GameObject.Destroy(flypathunitDel.gameObject);

        return;
    }

    public void DeleteCurvePathUnitAll_Unselect()
    {
        List<CCurvePathUnit> listCurvePathCollectedForDel = new List<CCurvePathUnit>();
        foreach (CCurvePathUnit curvepathUnit in m_listCurvePathUnit)
        {
            if (false == curvepathUnit.getSelect())
            {
                listCurvePathCollectedForDel.Add(curvepathUnit);
            }
        }

        if (listCurvePathCollectedForDel.Count > 0)
        {
            foreach (CCurvePathUnit curvepathUnit in listCurvePathCollectedForDel)
            {
                DeleteCurvePathUnitInclude(curvepathUnit);
            }

            listCurvePathCollectedForDel.Clear();
        } 

    } 

    public void Set_activate(bool bActivateCurrent)
    {
        if(true==bActivateCurrent)
        {
            Set_activate_Current(_splineGenerator_active);
        }
        else
        {
            Set_Inactivate_Current(_splineGenerator_active);
        }
    } 

    //@ Clear draw Line
    public void _Clear_LineDraw_Pnts()
    {
        foreach (CCurvePathLineDraw pathLineDraw in m_listLineDraw)
        {
            if(pathLineDraw)
                GameObject.Destroy(pathLineDraw.gameObject);
        }

        m_listLineDraw.Clear();
    } 

    public void SetUnselectAll_CurvePathUnits()
    {
        foreach (CCurvePathUnit flypathUnit in m_listCurvePathUnit)
        {
            if (flypathUnit)
                flypathUnit.setUnselectFlyPathUnit();
        }
    }

    public void _Clear_CurvePathUnit_Pnts()
    {
        foreach (CCurvePathUnit flypathUnit in m_listCurvePathUnit)
        {
            if (flypathUnit)
            {
                flypathUnit.setUnselectFlyPathUnit();
                flypathUnit.Release_curvepathUnit();
                UnityEngine.GameObject.Destroy(flypathUnit.gameObject);
            }
        } 

        m_listCurvePathUnit.Clear();
    }

     void ReleaseCurve_(CSplineGenerator processorSplinecurve_)
    {
        if (null == processorSplinecurve_)
        {
            return;
        }

        Set_Inactivate_Current(processorSplinecurve_);

        foreach (CCurvePathUnit curvePath in processorSplinecurve_.m_listCurvepathunit)
        {
            DeleteCurvePathUnit(curvePath);
        }

        if (null != processorSplinecurve_)
        {
            processorSplinecurve_.ClearCurveLine_spline();
            processorSplinecurve_.Release_ProcessorSpline();
        }

        processorSplinecurve_ = null;
    } // public void ReleaseCurve_()

    public void destructSplinecurveAll()
    {
        ClearCurveGuidelineAll();

        foreach (CSplineGenerator splineCurr in m_listProcessorSpline)
        {
            ReleaseCurve_(splineCurr);
        }
        m_listProcessorSpline.Clear();
        _Clear_CurvePathUnit_Pnts();
    } 

} // public class CSplineManufacturer : MonoBehaviour
