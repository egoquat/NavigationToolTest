using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Game Object: Navigation Mesh
public class CNaviMesh : MonoBehaviour
{
    #region geometry_navigationmesh
    private Mesh m_meshGeometry;
    [System.NonSerialized]
    public Vector3[] m_arrv3VB;
    [System.NonSerialized]
    public int[] m_arrIV;
    #endregion // geometry_navigationmesh

    protected CTriCollector _triCollector;
    protected CNavigation _navigation;
    protected CSplineManufacturer _splinecurve;

    public CTriCollector triCollector
    {
        get { return _triCollector; }
    }

    public CNavigation navigation
    {
        get { return _navigation; }
    }
    
    public CSplineManufacturer splinecurve
    {
        get { return _splinecurve; }
    }

    public bool rayIntersect_ScreenPoint(Vector3 v3Posray_2D, out RaycastHit raycastHit)
    {
        Ray rayPick = Camera.main.ScreenPointToRay(v3Posray_2D);
        return GetComponent<Collider>().Raycast(rayPick, out raycastHit, CMATH.FLOAT_MAX);
    }

    public void setAllFunctionalTris(List<CTRI> listTriAll,
                                        List<int> listTris_naviBlock,
                                        List<int> listTris_naviBlockRoad,
                                        List<int> listTris_naviStart,
                                        List<int> listTris_naviGoal)
    {
        _triCollector.m_listTris = new List<CTRI>(listTriAll);
        _triCollector.m_listTris_naviBlock = new List<int>(listTris_naviBlock);
        _triCollector.m_listTris_naviBlockRoad = new List<int>(listTris_naviBlockRoad);
        _triCollector.m_listTris_naviStart = new List<int>(listTris_naviStart);
        _triCollector.m_listTris_naviGoal = new List<int>(listTris_naviGoal);
    }

    //@ Process
    public void InitializeNaviMesh()
    {
        _navigation = new CNavigation();
        _triCollector = new CTriCollector();
        _splinecurve = new CSplineManufacturer();

        _navigation.InitializeNavi();
        _triCollector.InitTriCollector();
        _splinecurve.InitSplieCurve();

        m_meshGeometry = GetComponent<MeshFilter>().mesh;

        m_arrv3VB = m_meshGeometry.vertices;
        m_arrIV = m_meshGeometry.triangles;
    }

    public void ConstructAllTriangles(bool bRecomputeAllIV)
    {
        _triCollector.constructAllTris(m_arrIV,
                                        m_arrv3VB,
                                        transform,
                                        bRecomputeAllIV);
    }

    public void DestructNaviMesh()
    {
        _navigation.DestructNavi();
        _splinecurve.destructSplinecurveAll();
        _triCollector.DestructAllTris();

        _triCollector = null;
        _navigation = null;
        _splinecurve = null;
    }

    void Awake()
    {
        
    }
} // public class CNaviMesh : MonoBehaviour
