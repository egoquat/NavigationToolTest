using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//@ Process all Managements. 
public class processCycle
{
    public static processCycle GetInstance
    {
        get
        { return Singleton<processCycle>.GetInstance; }
    }

    #region constparameter
    public static bool APPLICATION_MODE_NAVITOOL = false;
    #endregion // #region constparameter

    //@ Instances
    public CNaviMesh m_meshNavigation_global;
    public CTriCollector m_triCollector;
    public CBaseTowerCollector m_baseTowerCollector;
    public CBaseCoreCollector m_baseCoreCollector;
    public CBaseCoreSubCollector m_baseCoresubCollector;
    public CBaseStartCollector m_baseStartCollector;
    public CBaseBlockCollector m_baseBlockCollector;
    public CSplineManufacturer m_SplineCurve;
    public IntervalUnitWalking m_intervalUnitWalking;
    public CToolModuleNavimesh m_toolmoduleNavimesh;

    public float m_speedmove_unitwalking = UnitWalking.default_speedmove_unitWalking;
    public float m_speedmove_unitflying = UnitFlying.default_speedmove_unitFlying;
    
    private int _IdxKeyNavi = -1;

    public int currentKeyNavi
    {
        get { return _IdxKeyNavi; }
    }

    //@ External Access
    public navimeshResource m_datanavimeshs;
    public CUnitFactory m_unitFactory_;
    public CProcessInput m_processInput;
    public CCurvePathUnit m_curvePathUnit_src;
    public CCurvePathLineDraw m_curvePathLineDraw_src;
    public CDrawText3D m_drawText3D_src;
    public UnitFlying m_unitFlying_src;
    public UnitWalking m_unitWalking_src;
    public CComboBoxCustom m_comboBox_src;
    //@Ex-Access - UI
    public CSelectBoxCustom m_selectBox_src;        //Select box 
    public CComboBoxCustom m_combo_src;             //Combo box // curve path, navigation mesh

    //@ External Access : valuable
    public bool RecomputeAllIV = true;

    protected bool modeTool = false;
    public bool _modeTool
    {
        get { return modeTool; }
        set { modeTool = value; }
    }

    //@ Load StageMap
    protected CMapTemplate m_stageMap = new CMapTemplate();

    protected bool resetGlobal_TrianglesAll(CNaviMesh naviMesh_active)
    {
        //@ Construct All Triangle
        naviMesh_active.ConstructAllTriangles(RecomputeAllIV);

        return true;
    } 

    protected bool resetGlobal_TrianglesAll_LoadBinaryFile(CNaviMesh naviMesh_active)
    {
        //  Load triangle and naviinfo
        if (0 < m_stageMap.MapTriangle.Count)
        {
            naviMesh_active.setAllFunctionalTris(m_stageMap.MapTriangle,
                                                m_stageMap.GroundBlockPoint_List,
                                                m_stageMap.GroundBlockRoadPoint_List,
                                                m_stageMap.GroundStartPoint_List,
                                                m_stageMap.GoalPoint_List);
        } // if (0 < m_stageMap.MapTriangle.Count)

        if (0 == m_stageMap.MapTriangle.Count)
        {
            return resetGlobal_TrianglesAll(naviMesh_active);
        } 

        return true;
    }

    public bool resetGlobal_NavigationCells(CTriCollector triCollector, 
                                            ref CNaviMesh navigationMesh )
    {
        List<CTRI> listTris;
        triCollector.getTris(out listTris);  // TriCollector에서 데이터 리스트를 얻어옴

        navigationMesh.navigation.ConstructNavi(listTris);
        bool bResultMapping = navigationMesh.navigation.mappingNavigation(triCollector.m_listTris_naviGoal,
                                                                triCollector.m_listTris_naviBlock,
                                                                triCollector.m_listTris_naviBlockRoad,
                                                                triCollector.m_listTris_naviStart,
                                                                true );

        if (true == bResultMapping)
        {
            m_toolmoduleNavimesh.drawStartCellToGoal(m_meshNavigation_global.navigation, m_triCollector);
        } 

        triCollector._bReadyToExecuteBuildup_NAVI = false;

        if (triCollector.getCount_NaviFunction_Tris() > 0)
        {
            m_toolmoduleNavimesh.drawAllTri_eachFunctional(m_meshNavigation_global.triCollector, false);
        }

        triCollector.setExtractAdjCells(navigationMesh.navigation);

        return true;
    } // protected bool resetGlobal_NavigationCells()

    public void LinkTriCollectorToAllBase(  ref CBaseTowerCollector managerBaseTower_ref,
                                            ref CBaseCoreCollector managerBaseCore_ref,
                                            ref CBaseCoreSubCollector managerBaseCoreSub_ref,
                                            ref CBaseStartCollector managerBaseStart_ref,
                                            ref CBaseBlockCollector managerBaseBlock_ref,
                                            CTriCollector triCollector)
    {
        managerBaseTower_ref.setTrisallToBase(triCollector.getTris());
        managerBaseCore_ref.setTrisallToBase(triCollector.getTris());
        managerBaseCoreSub_ref.setTrisallToBase(triCollector.getTris());
        managerBaseStart_ref.setTrisallToBase(triCollector.getTris());
        managerBaseBlock_ref.setTrisallToBase(triCollector.getTris());
    }

    protected bool Load_BaseTower_Binary(ref CBaseTowerCollector managerBaseTower_ref)
    {
        //  Load BaseTower
        if (0 < m_stageMap.BaseTower.Count)
        {
            foreach (CBASE__ block in m_stageMap.BaseTower)
            {
                managerBaseTower_ref.m_listBase__.Add(block);
            }

            managerBaseTower_ref._SetDrawDebug_Base__All();

            return true;
        } 
        else
        {
            return false;
        }
    }

    protected bool Load_BaseCore_Binary(ref CBaseCoreCollector managerBaseCore_ref)
    {        
        foreach ( BaseInfo core in m_stageMap.MainCoreList)
        {
            CBASE__ baseCurr = new CBASE__();

            baseCurr._v3PositionCenter = core.CenterPos;
            baseCurr.setIdxType(core.Type);
            baseCurr._listIdxTris = core.CellIndex;
            baseCurr._listv3Pnts = core.CoreTriPnt;
            baseCurr._listv3PntsSrc = core.CoreTriPntSrc;

            if (baseCurr._listv3PntsSrc.Count > 2)
            {
                managerBaseCore_ref.m_listBase__.Add(baseCurr);
            }
        }

        managerBaseCore_ref._SetDrawDebug_Base__All();

        return false;
    }

    protected bool Load_BaseCoreSub_Binary(ref CBaseCoreSubCollector managerBaseCoreSub_ref)
    {
        foreach (BaseInfo core in m_stageMap.SubCoreList)
        {
            CBASE__ baseCurr = new CBASE__();

            baseCurr._v3PositionCenter = core.CenterPos;
            baseCurr.setIdxType(core.Type);
            baseCurr._listIdxTris = core.CellIndex;
            baseCurr._listv3Pnts = core.CoreTriPnt;
            baseCurr._listv3PntsSrc = core.CoreTriPntSrc;

            if (baseCurr._listv3PntsSrc.Count > 2)
            {
                managerBaseCoreSub_ref.m_listBase__.Add(baseCurr);
            }
        }

        managerBaseCoreSub_ref._SetDrawDebug_Base__All();

        return false;
    }

    protected bool Load_BaseStart_Binary(ref CBaseStartCollector managerBaseStart_ref)
    {
        foreach (BaseInfo core in m_stageMap.StartbaseList)
        {
            CBASE__ baseCurr = new CBASE__();

            baseCurr._v3PositionCenter = core.CenterPos;
            baseCurr.setIdxType(core.Type);
            baseCurr._listIdxTris = core.CellIndex;
            baseCurr._listv3Pnts = core.CoreTriPnt;
            baseCurr._listv3PntsSrc = core.CoreTriPntSrc;

            if (baseCurr._listv3PntsSrc.Count > 2)
            {
                managerBaseStart_ref.m_listBase__.Add(baseCurr);
            }
        }

        managerBaseStart_ref._SetDrawDebug_Base__All();

        return false;
    }

    protected bool Load_BaseBlock_Binary(ref CBaseBlockCollector managerBaseBlock_ref)
    {
        foreach (BaseInfo core in m_stageMap.BlockBaseList)
        {
            CBASE__ baseCurr = new CBASE__();

            baseCurr._v3PositionCenter = core.CenterPos;
            baseCurr.setIdxType(core.Type);
            baseCurr._listIdxTris = core.CellIndex;
            baseCurr._listv3Pnts = core.CoreTriPnt;
            baseCurr._listv3PntsSrc = core.CoreTriPntSrc;

            if (baseCurr._listv3PntsSrc.Count > 2)
            {
                managerBaseBlock_ref.m_listBase__.Add(baseCurr);
            }
        }

        managerBaseBlock_ref._SetDrawDebug_Base__All();

        return false;
    }

    protected bool Load_CurvePath_Binary( ref CSplineManufacturer splinecurveManufacturer)
    {
        splinecurveManufacturer.LoadCurvePath_CMapTemplate(m_stageMap.FlyUnitPath_List);

        return true;
    }

    private void LoadNaviFromTemplate()
    {
        foreach (int block in m_stageMap.GroundBlockPoint_List)
            m_triCollector.m_listTris_naviBlock.Add(block);

        foreach (int blockroad in m_stageMap.GroundBlockRoadPoint_List)
            m_triCollector.m_listTris_naviBlockRoad.Add(blockroad);

        foreach (int goal in m_stageMap.GoalPoint_List)
            m_triCollector.m_listTris_naviGoal.Add(goal);

        foreach (int start in m_stageMap.GroundStartPoint_List)
            m_triCollector.m_listTris_naviStart.Add(start);
    } // void _LoadNaviFromTemplate()

    public bool resetGlobal_constantObjects(CTriCollector triCollector, bool bConstantlyCreateObj)
    {
        if (triCollector.m_listTris_naviStart.Count > 0)
        {
            foreach (int iTriStart in triCollector.m_listTris_naviStart)
            {
                CTRI triStartCurr = triCollector.getTri(iTriStart);
                if (null != triStartCurr)
                {
                    m_intervalUnitWalking.newIntervalConstantly(iTriStart,
                                                                m_processInput._respawnunitwalking,
                                                                m_processInput._timeinterval_unitwalking,
                                                                triStartCurr.GetCenterTri());
                }
            }
        }
        else
        {
            m_intervalUnitWalking.workprocessInterval = false;
        }

        m_intervalUnitWalking.workprocessInterval = bConstantlyCreateObj;

        return true;
    }

    public bool resetGlobal(int iIdxKeyNavi, bool bInitFromGeometry)
    {
        if (iIdxKeyNavi < 0)
        {
            return false;
        }

        _IdxKeyNavi = iIdxKeyNavi;

        bool bResultProcess = false;

        //@ Clear all managers
        bResultProcess = clearGLOBAL(true);
        if (false == bResultProcess)
        {
            Debug.Log("Error. clearGLOBAL()!/////");
        }

        //@ Instantiate NaviMesh
        m_meshNavigation_global = m_datanavimeshs.getNavigationMesh(iIdxKeyNavi);
        m_meshNavigation_global.InitializeNaviMesh();

        m_triCollector = m_meshNavigation_global.triCollector;
        m_SplineCurve = m_meshNavigation_global.splinecurve;

        if (true == bInitFromGeometry)
        {
            //@ Construct All Triangle
            bResultProcess = resetGlobal_TrianglesAll(m_meshNavigation_global);
            if (false == bResultProcess)
            {
                Debug.Log("Error!.//");
            }

            //@ Set constant objects to start position
            bResultProcess = resetGlobal_constantObjects(m_triCollector, false);
            if (false == bResultProcess)
            {
                Debug.Log("Error!.//");
            }

            //@ Intiailzie basemanets
            LinkTriCollectorToAllBase(  ref m_baseTowerCollector, 
                                        ref m_baseCoreCollector, 
                                        ref m_baseCoresubCollector, 
                                        ref m_baseStartCollector, 
                                        ref m_baseBlockCollector,
                                        m_triCollector );

            //@ Re-load navi cells from template pre-loads.
            LoadNaviFromTemplate();

            //@ Load bases.
            Load_BaseTower_Binary(ref m_baseTowerCollector);
            Load_BaseCore_Binary(ref m_baseCoreCollector);
            Load_BaseCoreSub_Binary(ref m_baseCoresubCollector);
            Load_BaseStart_Binary(ref m_baseStartCollector);
            Load_BaseBlock_Binary(ref m_baseBlockCollector);

            //@ Load Curve Path through script
            bResultProcess = Load_CurvePath_Binary(ref m_SplineCurve);
            if (false == bResultProcess)
            {
                Debug.Log("loadingProcess_Binary Load_CurvePath_Binary_error. //");
            }

            //@ Construct Navigation 
            bResultProcess = resetGlobal_NavigationCells(m_triCollector, ref m_meshNavigation_global);
            if (false == bResultProcess)
            {
                Debug.Log("Error!.//");
            }

            //@Initialze UnitWalking
            UnitWalking.SetStatusCellToCell();

        } // if (true == bInitFromGeometry)
        else
        {
            //@ Load using script.
            loadingProcess_Binary(iIdxKeyNavi);

        } // if (false == bInitFromGeometry)

        //@ Input Process Initialize
        m_processInput.resetInput(this);
        m_intervalUnitWalking.InitIntervalUnitWalking(m_triCollector);

        return true;
    } // public void resetGlobal

    //@ Loading All Navi Process From script data.
    protected bool loadingProcess_Binary(int iIdxKeyNavi)
    {
        bool bResultProcess = false;

        m_stageMap.m_Filename = m_datanavimeshs.getNaviMeshName(iIdxKeyNavi);
        bool IsLoadMap = m_stageMap.LoadStage();
        
        {
            //@ Construct All Triangle
            bResultProcess = resetGlobal_TrianglesAll_LoadBinaryFile(m_meshNavigation_global);
            if (false == bResultProcess)
            {
                Debug.Log("Error. false == bResultProcess.//");
            }

            //@ Set constant objects to start position
            bResultProcess = resetGlobal_constantObjects(m_triCollector, false);
            if (false == bResultProcess)
            {
                Debug.Log("Notice. false==resetGlobal_constantObjects().//");
            }

            //@ Intiailzie basemanets
            {
                LinkTriCollectorToAllBase(  ref m_baseTowerCollector,
                                            ref m_baseCoreCollector,
                                            ref m_baseCoresubCollector,
                                            ref m_baseStartCollector,
                                            ref m_baseBlockCollector,
                                            m_triCollector);
            }

            if (true == IsLoadMap)
            {
                //@ BaseTower,BaseCore-Sub,BaseCore
                Load_BaseTower_Binary(ref m_baseTowerCollector);
                Load_BaseCore_Binary(ref m_baseCoreCollector);
                Load_BaseCoreSub_Binary(ref m_baseCoresubCollector);
                Load_BaseStart_Binary(ref m_baseStartCollector);
                Load_BaseBlock_Binary(ref m_baseBlockCollector);

                //@ Load Curve Path through script
                bResultProcess = Load_CurvePath_Binary(ref m_SplineCurve);
                if (false == bResultProcess)
                {
                    Debug.Log("loadingProcess_Binary Load_CurvePath_Binary_error. //");
                }
            } 

            

            //@ Construct Navigation 
            bResultProcess = resetGlobal_NavigationCells(m_triCollector, ref m_meshNavigation_global);
            if (false == bResultProcess)
            {
                Debug.Log("Notice. false==resetGlobal_NavigationCells().//");
            }

            //@Initialze UnitWalking
            UnitWalking.SetStatusCellToCell();
        }

        return true;
    } // protected void loadingProcess_Binary

    //@ Initialize managements.
	public void Initialize (processResource resources) 
    {
        if (null == resources)
        {
            Debug.Log("ERROR(critical) all gameobjectProcessResource is null.");
            return;
        }

        _modeTool = true;
        
        //@ re-link resource all.
        m_datanavimeshs = resources._navimeshResource;
        m_unitFactory_ = resources._unitfactory;
        m_processInput = resources._processInput;
        m_curvePathUnit_src = resources._curvePathUnit_src;
        m_curvePathLineDraw_src = resources._curvePathLineDraw_src;
        m_drawText3D_src = resources._drawText3D_src;
        m_unitFlying_src = resources._unitFlying_src;
        m_unitWalking_src = resources._unitWalking_src;
        m_selectBox_src = resources._selectBox_src;
        m_combo_src = resources._combo_src;

        _IdxKeyNavi = m_datanavimeshs.StartNaviMesh;

        m_speedmove_unitwalking = m_processInput.velocity_unitwalking;
        m_speedmove_unitflying = m_processInput.velocity_unitflying;

        //@ Construct
        m_baseTowerCollector = new CBaseTowerCollector();
        m_baseCoreCollector = new CBaseCoreCollector();
        m_baseCoresubCollector = new CBaseCoreSubCollector();
        m_baseStartCollector = new CBaseStartCollector();
        m_baseBlockCollector = new CBaseBlockCollector();

        m_toolmoduleNavimesh = new CToolModuleNavimesh();
        m_intervalUnitWalking = new IntervalUnitWalking();

        //@ Initialize
        m_processInput.InitProcessInput(this);
        m_toolmoduleNavimesh.InitNaviMeshTool();
        m_baseTowerCollector.InitBaseCollector();
        m_baseCoreCollector.InitBaseCollector();
        m_baseCoresubCollector.InitBaseCollector();
        m_baseStartCollector.InitBaseCollector();
        m_baseBlockCollector.InitBaseBlockCollector(m_processInput._blockbasetypes.Length);
       
        //@ Initialize all managers
        bool bResult = false;

        bResult = resetGlobal(_IdxKeyNavi, m_processInput._loadfrom_geometry);
        if (false == bResult)
        {
            Debug.Log("ERROR. resetGlobal().//");
        }

        //@ Application execute mode, which is naviscene or gamescene
        processCycle.APPLICATION_MODE_NAVITOOL = true;

        GameContext gamecontext = GameContext.GetInstance;
        gamecontext.ShowDebugInfo = false;
    } // void Initialize

    protected bool clearGLOBAL(bool isSkipLoadedData = false)
    {
        try
        {
            //@ Release & Clear Triangle, DrawMesh, Object Dyn
            UnitPool unitpool_ = UnitPool.GetInstance;
            unitpool_.TruncateAll();

            m_intervalUnitWalking.DestructIntervalsAll();
            m_processInput.DestructInput();

            if (false == isSkipLoadedData)
            {
                m_stageMap.Release();
            }

            m_baseTowerCollector.DestructBaseAll();
            m_baseCoreCollector.DestructBaseAll();
            m_baseCoresubCollector.DestructBaseAll();
            m_baseStartCollector.DestructBaseAll();
            m_baseBlockCollector.DestructBaseAll();

            m_toolmoduleNavimesh.ReleaseNaviMeshTool();

            if (null != m_meshNavigation_global)
            {
                m_meshNavigation_global.DestructNaviMesh();
                m_meshNavigation_global = null;
            }

            GC.Collect();

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    } // protected void clearGLOBAL()

    public void Release()
    {
        clearGLOBAL();
    }

    //Constructor, Destructor
	processCycle()
    {

    }
} // public class processCycle : MonoBehaviour

