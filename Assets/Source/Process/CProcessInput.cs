using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

//@ All user keyboard, mouse, joystick event.
public class CProcessInput : MonoBehaviour
{
    //@ InputMode // Navigation/Base
    enum E_INPUT_MODE
    {
        E_INPUT_MODE_NULL = -1,
        E_INPUT_MODE_NAVIGATION = 0,
        E_INPUT_MODE_CURVELINE,
        E_INPUT_MODE_END,
    }

    //@ PROPERTY Input
    class propertyinput_gui
    {
        public float _fMarginfromLeft_Row01 = 5.0f;

        //@ Panel Position
        public Rect _rect_GroupPanelBatch = new Rect(410, 600, 400, 650);                // Panel batch function

        //@ Select navigation-mesh 
        public Rect _rect_Text_NameNavimesh = new Rect(20.0f, 10.0f, 300.0f, 24.0f);
        public Rect _rect_BtnSelect_Navimesh = new Rect(330.0f, 10.0f, DFLT_BTN_SIZE.x, DFLT_BTN_SIZE.y);
        public Rect _rect_BtnSave_Navimesh = new Rect(450.0f, 10.0f, DFLT_BTN_SIZE.x, DFLT_BTN_SIZE.y);

        //@ SelectBox NavigationMesh
        public Rect _rect_SelectBox_Navimesh = new Rect(20.0f, 40.0f, 280.0f, 24.0f);

        //@ Label
        public Vector2 _v2Position_Label_ToolMode = new Vector2(0.0f, 30.0f);    
        public Vector2 _v2Position_Label_CurveSpline = new Vector2(0.0f, 90.0f); 
        public Vector2 _v2Position_Label_Base = new Vector2(0.0f, 270.0f);       

        //@ MODE
        public Vector2 _v2Position_BtnModeChange = new Vector2(0.0f, 50.0f);                // Mode Changes
        public Vector2 _v2Position_BtnReload_ALL = new Vector2(130.0f, 50.0f);                // Load ALL
        public Vector2 _v2Position_BtnClearAll = new Vector2(260.0f, 50.0f);                // Clear All

        //@ BATCH FUNCTION
        //@ SelectBox curve   
        public int _isel_TypeCurve = 0;                                                    // SELECT Type Build Curve Type.
        public Rect _rectPos_Sel_TypeCurve = new Rect();                                   // Which Curve

        //@ combo curve path
        public Rect _rect_Combo_CurvePathLayer = new Rect(0.0f, 110.0f, 125.0f, 24.0f);     // Which Curve
        public Rect _rect_SelectBox_TypeCurve = new Rect(130.0f, 110.0f, 125.0f, 24.0f);    // Curve Type (RNS, TNS, SNS)
        public Vector2 _v2Position_BtnBuildCurve = new Vector2(260.0f, 110.0f);             // Build Curve for FlyUnit
        public Vector2 _v2Position_BtnDeleteSelectCurve = new Vector2(260.0f, 140.0f);            // Delete Current Select Curve 

        public Vector2 _v2Position_BtnClearCurveLine = new Vector2(260.0f, 170.0f);         // Clear FlyUnit DrawLine

        public Vector2 _v3Position_BtnNewCurvePath = new Vector2(0.0f, 200.0f);             // New Curve path 
        public Vector2 _v2Position_BtnShowAllCurve = new Vector2(130.0f, 200.0f);               // Show All Curve
        public Vector2 _v2Position_BtnDeleteCurvePath = new Vector2(260.0f, 200.0f);            // Delete all curve paths except selected ones.

        public Vector2 _v2Position_BtnNewTestFlyObj = new Vector2(0.0f, 230.0f);            // New Test FlyUnit
        public Vector2 _v2Position_BtnSwitchStateFlyunit = new Vector2(130.0f, 230.0f);     // Switch State FlyUnit
        public Vector2 _v2Position_BtnClearTestFlyObj = new Vector2(260.0f, 230.0f);        // Clear all FlyUnit

        //@ Basement
        public Vector2 _v2Position_BtnCollectPerfectSquares = new Vector2(130.0f, 320.0f);  // Collect Perfect Square in NaviMesh for Basement
        public Vector2 _v2Position_BtnClearBase = new Vector2(260.0f, 320.0f);              // Clear Base

        public Rect _rect_Combo_CorebaseType = new Rect(0.0f, 350.0f, 125.0f, 24.0f);       // Core base type
        public Rect _rect_Combo_StartbaseType = new Rect(130.0f, 350.0f, 125.0f, 24.0f);    // Start base type
        public Rect _rect_Combo_BlockbaseType = new Rect(260.0f, 350.0f, 125.0f, 24.0f);    // Block base type


        #region panelNavigation
        public Rect _rect_Toggle_NaviPanel = new Rect(260.0f + DFLT_LEFTMARGIN_TOGGLEBOX, 350.0f, 125.0f - DFLT_LEFTMARGIN_TOGGLEBOX, 24.0f);// NaviPanel

        //@ Panel Position - test
        public Rect _rect_GroupPanelTest = new Rect(410, 800, 400, 500);                 // Panel batch function

        //@ Label
        public Vector2 _v2Position_Label_Navigation = new Vector2(0.0f, 30.0f);            // Label Navigation
        public Vector2 _v2Position_Label_Information = new Vector2(0.0f, 140.0f);

        //@ Navigation
        public Rect _rect_Toggle_BlockTypeNAVICELL = new Rect(0.0f + DFLT_LEFTMARGIN_TOGGLEBOX, 50.0f, 125.0f - DFLT_LEFTMARGIN_TOGGLEBOX, 24.0f);// Block Type
        public Vector2 _v2Position_BtnDrawLevelCellsAll = new Vector2(130.0f, 50.0f);
        public Vector2 _v2Position_BtnClearNavi = new Vector2(260.0f, 50.0f);
        public Vector2 _v2Position_LabelInformation01 = new Vector2(0.0f + DFLT_LEFTMARGIN_TOGGLEBOX, 160);

        //@ Walk Object
        public Vector2 _v2Position_BtnObjStateSwitch = new Vector2(0.0f, 80.0f);           // Object Status switching (IDLE/WALKING NAVI)
        public Vector2 _v2Position_BtnObjStartInst = new Vector2(130.0f, 80.0f);           // Object Instant on/off on StartCell in Navigation
        public Vector2 _v2Position_BtnObjClearAll = new Vector2(260.0f, 80.0f);            // Clear Obj
        public Rect _rect_Combo_UnitwalkModelType = new Rect(0.0f + DFLT_LEFTMARGIN_TOGGLEBOX, 110.0f, 125.0f, 24.0f);  // Unitwalking modeltype
        #endregion // panelNavigation

    } // public class propertyinput_gui

    #region constparameter
    //@param : external
    public static readonly int NULLMODELTYPE_UNITWALKING = -1;

    //@param ui : external
    public static readonly int DFLT_SELECTED_NAVIMESH = 0;

    //@param : internal
    static readonly E_INPUT_MODE DFLT_INPUT_MODE = E_INPUT_MODE.E_INPUT_MODE_NAVIGATION;
    static readonly Vector2 DFLT_V2ROT_CAM = new Vector2(0.0f, 0.0f);
    static readonly float DFLT_SPDROTATE_CAM = 600.0f;
    static readonly float DFLT_SPDMOVE_CAM = 20.0f;
    static readonly float DFLT_SPDMOVEWHEEL_CAM = 1000.0f;

    static readonly bool DFLT_DO_ACCERLMOVE_CAM = false;
    static readonly bool DFLT_READYTOASSIGN_OBJ = false;

    //@param navimesh
    static readonly bool DFLT_READYTO_RENDERNAVIMESH_DEBUG = false;
    static readonly bool DFLT_READYTOASSIGN_NAVI = false;
    static readonly bool DFLT_ONGUI = true;

    //@param ui : internal
    static readonly float DFLT_LEFTMARGIN_TOGGLEBOX = 10.0f;
    static readonly Vector2 DFLT_BTN_SIZE = new Vector2(110.0f, 24.0f);
    static readonly int DFLT_SELECTED_CURVE = 0;
    static readonly float DFLT_DISTANCEOF_CURVEPATHUNIT_FROM_CAM = 20.0f;

    static readonly Vector2 DFLT_V2SIZE_BUTTON = new Vector2(125.0f, 24.0f);
    static readonly Vector2 DFLT_V2SIZE_LABEL_INFO = new Vector2(380.0f, 24.0f);
    static readonly string[] DFLT_ARRSTR_TYPECURVE = { "rounded_ns", "smooth_ns", "timed_ns" };


    static readonly Color DFLT_COLOR_DRAWALLTRIANGLES_DEBUG = Color.green;
    #endregion // #region constparameter

    //@ PROPERTY Input
    bool _bExpendToSelectNavigation = false;
    E_INPUT_MODE _eInput_Mode = DFLT_INPUT_MODE;
    Vector2 _v2Rot_CAM = DFLT_V2ROT_CAM;
    Vector2 _v2MousePoint_previous = DFLT_V2ROT_CAM;
    readonly float _fSpdRot_CAM = DFLT_SPDROTATE_CAM;
    readonly float _fSpdMove_CAM = DFLT_SPDMOVE_CAM;
    readonly float _fSpdMoveWheel_CAM = DFLT_SPDMOVEWHEEL_CAM;
    readonly float _fSpdMoveAccel_CAM = 2.0f;
    bool _bAccelMove_CAM = DFLT_DO_ACCERLMOVE_CAM;
    bool _bOnGUI = DFLT_ONGUI;
    bool _bReadyToAssign_OBJ = DFLT_READYTOASSIGN_OBJ;
    RaycastHit _rayhitNaviMesh_OBJ = new RaycastHit();
    bool _bReadyToRenderLineDebug_NAVIMESH = DFLT_READYTO_RENDERNAVIMESH_DEBUG;
    bool _bReadyToRenderMeshDebug_NAVIMESH = DFLT_READYTO_RENDERNAVIMESH_DEBUG;

    bool _bReadyToAssign_NAVI = DFLT_READYTOASSIGN_NAVI;
    bool _bReadyToAssign_NAVIADJACENT = DFLT_READYTOASSIGN_NAVI;
    bool _bBlockTypeRoad = true;
    bool _bReadyToAssign_BASE = false;

    bool _bDoDrawCurveLine = true;
    bool _bShowAllCurveLine = false;
    bool _bReadyToAssign_FlyPath = false;
    bool _bReadyToActivate_FlyPath = false;
    bool _bOutputDetectLeaks = false;

    //@
    bool _bRequestRepresentPanelNavi = false;
    bool _bRepresentPanelNavi = false;
    bool _bRequestRepresentStartToGoal = false;
    bool _bRepresentStartToGoal = false;

    Camera m_camera_SceneMain;

    //@ UI:property
    propertyinput_gui m_propertyInputGUI = new propertyinput_gui();

    //@ UI:Component
    CComboBoxCustom m_combo_LayerCurve;               // Curve path select combo box
    CComboBoxCustom m_combo_modeltype_unitwalking;    // unitwalking modeltype select combo box
    CComboBoxCustom m_combo_CoreBaseType;             // Core type select combo box
    CComboBoxCustom m_combo_StartBaseType;            // Start type select combo box
    CComboBoxCustom m_combo_BlockBaseType;            // Block base select combo box
    CComboBoxCustom m_combo_TypeCurve;                // Curve path select box
    CSelectBoxCustom m_selectbox_Navimesh;            // Navigation mesh select box

    //@ UI:Property
    int[] m_arrIDUnitModel = null;
    CDrawRenderMesh m_drawRenderMesh = new CDrawRenderMesh();

    //@ navi module
    navimeshResource m_datanavimeshs = null;
    CNaviMesh m_meshNavi_input;
    CToolModuleNavimesh m_toolmoduleNavimesh;
    CSplineManufacturer m_SplineCurve;
    CTriCollector m_triCollector;

    //@ navi module: Save StageMap 
    CMapTemplate m_stageMap = new CMapTemplate();
    CBaseTowerCollector m_baseTowerCollector;
    CBaseCoreCollector m_baseCoreCollector;
    CBaseCoreSubCollector m_baseCoresubCollector;
    CBaseStartCollector m_baseStartCollector;
    CBaseBlockCollector m_baseBlockCollector;

    IntervalUnitWalking m_intervalUnitwalking;


    //@Ex-link - value
    public bool _loadfrom_geometry = false;
    public int _respawnunitwalking = 3;
    public float _timeinterval_unitwalking = 4.0f;
    public bool _randomUnitPresent = true;
    public float _gameSpeed = 1.0f;
    public float gamespeed_unit = 0.2f;
    public float velocity_unitwalking = 1.0f;
    public float velocity_unitflying = 3.0f;

    //@Ex-link - base
    public string[] _corebasetypes = { "coretype0", "coretype1", "coretype2", "coretype3", "coretype4" };
    public string[] _startbasetypes = { "startype0", "startype1", "startype2", "startype3", "startype4" };
    public string[] _blockbasetypes = { "blocktype0", "blocktype1", "blocktype2", "blocktype3", "blocktype4" };


    public void setRenderOnGUI(bool renderOnGUI)
    {
        _bOnGUI = renderOnGUI;
    }

    //@ Process
    public void InitProcessInput(processCycle processcycle)
    {
        GameObject [] arrUnitModel = processcycle.m_unitFactory_.m_UnitModel;
        m_arrIDUnitModel = new int[arrUnitModel.Length + 1];
        m_arrIDUnitModel[0] = NULLMODELTYPE_UNITWALKING;
        for (int iModel = 0; iModel < arrUnitModel.Length; ++iModel)
        {
            int.TryParse(arrUnitModel[iModel].name, out m_arrIDUnitModel[iModel+1]);
        }

        CComboBoxCustom combobox_SRC = processcycle.m_combo_src;
        CSelectBoxCustom selectbox_SRC = processcycle.m_selectBox_src;

        m_combo_CoreBaseType = (CComboBoxCustom)Instantiate(combobox_SRC);
        m_combo_StartBaseType = (CComboBoxCustom)Instantiate(combobox_SRC);
        m_combo_BlockBaseType = (CComboBoxCustom)Instantiate(combobox_SRC);
        m_combo_LayerCurve = (CComboBoxCustom)Instantiate(combobox_SRC);
        m_combo_modeltype_unitwalking = (CComboBoxCustom)Instantiate(combobox_SRC);
        m_combo_TypeCurve = (CComboBoxCustom)Instantiate(combobox_SRC);

        m_selectbox_Navimesh = (CSelectBoxCustom)Instantiate(selectbox_SRC);

        SetSelectbox_NavigationMesh(processcycle.m_datanavimeshs.getArrayNaviMeshNeme(), processcycle.currentKeyNavi, true);
        SetComboBox_TypeCurve(DFLT_ARRSTR_TYPECURVE, DFLT_SELECTED_CURVE, true);

        GameContext.GetInstance.GameSpeed = _gameSpeed;
    }

    public void resetInput( processCycle processcycle)
    {
        m_meshNavi_input = processcycle.m_meshNavigation_global;
        m_datanavimeshs = processcycle.m_datanavimeshs;

        //@ 
        m_triCollector = m_meshNavi_input.triCollector;
        m_SplineCurve = m_meshNavi_input.splinecurve;
        m_toolmoduleNavimesh = processcycle.m_toolmoduleNavimesh;

        //@
        m_baseTowerCollector = processcycle.m_baseTowerCollector;
        m_baseCoreCollector = processcycle.m_baseCoreCollector;
        m_baseCoresubCollector = processcycle.m_baseCoresubCollector;
        m_baseStartCollector = processcycle.m_baseStartCollector;
        m_baseBlockCollector = processcycle.m_baseBlockCollector;

        //@ 
        m_intervalUnitwalking = processcycle.m_intervalUnitWalking;
        m_intervalUnitwalking.randomUnitWalkingVisual = _randomUnitPresent;

        //Combo, Selectbox
        SetComboBox_CoreType(_corebasetypes, 0, true);
        SetComboBox_StartType(_startbasetypes, 0, true);
        SetComboBox_BlockType(_blockbasetypes, 0, true);
        SetComboBox_Curve(ref m_SplineCurve, DFLT_SELECTED_CURVE, true);
        SetComboBox_ModeltypeUnitwalking(m_arrIDUnitModel, 0, true);
    }

    //@ Get intersect ray Point Intersected from current mouse point. 
    private bool rayIntersect_MousePoint(out Vector3 v3PosIntersected, int iLayerMask)
    {
        RaycastHit raycastHit;
        Vector3 v3Posray_2D = Input.mousePosition;
        Ray rayPick = Camera.main.ScreenPointToRay(v3Posray_2D);
        bool bIntersected = Physics.Raycast(rayPick, out raycastHit, CMATH.FLOAT_MAX);
        v3PosIntersected = raycastHit.point;
        return bIntersected;
    }

    //@ Get intersect ray info from current mouse point. 
    // -return:RaycastHit(Intersect Position, Intersect Distance...)
    private bool rayIntersect_MousePoint(out RaycastHit raycastHit, int iLayerMask)
    {
        Vector3 v3Posray_2D = Input.mousePosition;
        Ray rayPick = Camera.main.ScreenPointToRay(v3Posray_2D);
        return Physics.Raycast(rayPick, out raycastHit, CMATH.FLOAT_MAX);
    }

    //@ Get intersect ray from current mouse point. 
    // -return:ray
    // -return:RaycastHit(Intersect Position, Intersect Distance...)
    private bool rayIntersect_MousePoint(out Ray rayPick_out, out RaycastHit raycastHit, int iLayerMask)
    {
        Vector3 v3Posray_2D = Input.mousePosition;
        rayPick_out = Camera.main.ScreenPointToRay(v3Posray_2D);
        return Physics.Raycast(rayPick_out, out raycastHit, CMATH.FLOAT_MAX);
    }

    //@ Get intersect ray from current mouse point. 
    // -return:RaycastHit(Intersect Position, Intersect Distance...)
    private bool rayIntersect_ScreenCenter(CNaviMesh navimesh_, out RaycastHit raycastHit)
    {
        Vector3 v3Posray_2D = new Vector3(Screen.height, Screen.width, 0.0f);
        Ray rayPick = Camera.main.ScreenPointToRay(v3Posray_2D);
        return navimesh_.GetComponent<Collider>().Raycast(rayPick, out raycastHit, CMATH.FLOAT_MAX);
    }

    private Vector3 getPosScreenToWorld_FarFrom(Vector2 v2PosOnScreen, float fDistanceFar)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(v2PosOnScreen.x, v2PosOnScreen.y, fDistanceFar));
    }

    private Ray getRayMousePnt()
    {
        Vector3 v3Posray_2D = Input.mousePosition;
        return Camera.main.ScreenPointToRay(v3Posray_2D);
    }

    //@ Set Select box curve path
    public void SetSelectbox_NavigationMesh(string[] arrstrSelectItems, int iSelect, bool bVisible)
    {
        //@ Navigation mesh list
        m_selectbox_Navimesh.InitSelectBox(m_propertyInputGUI._rect_SelectBox_Navimesh);
        m_selectbox_Navimesh.SetClear();
        m_selectbox_Navimesh.visible = bVisible;

        if (arrstrSelectItems.Length < 1)
        {
            return;
        }

        int iSequenceNaviMapStage = 0;

        foreach (string strItem in arrstrSelectItems)
        {
            m_selectbox_Navimesh.InsertNewItem( (iSequenceNaviMapStage++) + ". " + strItem);
        }

        if (iSelect >= 0 && iSelect < m_selectbox_Navimesh.getCount())
        {
            m_selectbox_Navimesh.SetSelectItemForced(iSelect);
        }
    }

    //@ Set ComboBox CoreBaseType
    public void SetComboBox_TypeCurve(string[] arrItemsTypeCurve, int iSelect, bool bVisible)
    {
        m_combo_TypeCurve.Initialize(m_propertyInputGUI._rect_SelectBox_TypeCurve.xMin,
                                      m_propertyInputGUI._rect_SelectBox_TypeCurve.yMin,
                                      m_propertyInputGUI._rect_SelectBox_TypeCurve.width,
                                      m_propertyInputGUI._rect_SelectBox_TypeCurve.height, "CurveType");

        m_combo_TypeCurve.SetClear();
        m_combo_TypeCurve.visible = bVisible;

        if (arrItemsTypeCurve.Length < 1)
        {
            return;
        }

        foreach (string typecurve in arrItemsTypeCurve)
        {
            m_combo_TypeCurve.InsertNewItem(typecurve);
        }

        if (iSelect >= 0 && iSelect < m_combo_TypeCurve.getCount())
        {
            m_combo_TypeCurve.SetSelectItem(iSelect);
        }
    } 

    //@ Set ComboBox CoreBaseType
    public void SetComboBox_CoreType(string[] arrCoreTypes, int iSelectCoreType, bool bVisible)
    {
        m_combo_CoreBaseType.Initialize(m_propertyInputGUI._rect_Combo_CorebaseType.xMin,
                                      m_propertyInputGUI._rect_Combo_CorebaseType.yMin,
                                      m_propertyInputGUI._rect_Combo_CorebaseType.width,
                                      m_propertyInputGUI._rect_Combo_CorebaseType.height, "CoreBaseType");

        m_combo_CoreBaseType.SetClear();
        m_combo_CoreBaseType.visible = bVisible;

        if (arrCoreTypes.Length < 1)
        {
            return;
        }

        foreach (string coreType in arrCoreTypes)
        {
            m_combo_CoreBaseType.InsertNewItem(coreType);
        }

        if (iSelectCoreType >= 0 && iSelectCoreType < m_combo_CoreBaseType.getCount())
        {
            m_combo_CoreBaseType.SetSelectItem(iSelectCoreType);
        }
    } 

    //@ Set ComboBox StartType
    public void SetComboBox_StartType(string[] arrStartTypes, int iSelectStartType, bool bVisible)
    {
        m_combo_StartBaseType.Initialize(m_propertyInputGUI._rect_Combo_StartbaseType.xMin,
                                      m_propertyInputGUI._rect_Combo_StartbaseType.yMin,
                                      m_propertyInputGUI._rect_Combo_StartbaseType.width,
                                      m_propertyInputGUI._rect_Combo_StartbaseType.height, "StartBaseType");

        m_combo_StartBaseType.SetClear();
        m_combo_StartBaseType.visible = bVisible;

        if (arrStartTypes.Length < 1)
        {
            return;
        }

        foreach (string startType in arrStartTypes)
        {
            m_combo_StartBaseType.InsertNewItem(startType);
        }

        if (iSelectStartType >= 0 && iSelectStartType < m_combo_StartBaseType.getCount())
        {
            m_combo_StartBaseType.SetSelectItem(iSelectStartType);
        }
    }


    //@ Set ComboBox BlockType
    public void SetComboBox_BlockType(string[] arrBlockTypes, int iSelectBlockType, bool bVisible)
    {
        m_combo_BlockBaseType.Initialize(m_propertyInputGUI._rect_Combo_BlockbaseType.xMin,
                                      m_propertyInputGUI._rect_Combo_BlockbaseType.yMin,
                                      m_propertyInputGUI._rect_Combo_BlockbaseType.width,
                                      m_propertyInputGUI._rect_Combo_BlockbaseType.height, "StartBaseType");

        m_combo_BlockBaseType.SetClear();
        m_combo_BlockBaseType.visible = bVisible;

        if (arrBlockTypes.Length < 1)
        {
            return;
        }

        foreach (string strType in arrBlockTypes)
        {
            m_combo_BlockBaseType.InsertNewItem(strType);
        }

        if (iSelectBlockType >= 0 && iSelectBlockType < m_combo_BlockBaseType.getCount())
        {
            m_combo_BlockBaseType.SetSelectItem(iSelectBlockType);
        }
    } 

    //@ Set ComboBox Curve
    public void SetComboBox_Curve(ref CSplineManufacturer managerSplineCurve_, int iSelectCurve, bool bVisible)
    {
        m_combo_LayerCurve.Initialize(m_propertyInputGUI._rect_Combo_CurvePathLayer.xMin,
                                      m_propertyInputGUI._rect_Combo_CurvePathLayer.yMin,
                                      m_propertyInputGUI._rect_Combo_CurvePathLayer.width,
                                      m_propertyInputGUI._rect_Combo_CurvePathLayer.height, "> New Curve");

        m_combo_LayerCurve.SetClear();
        m_combo_LayerCurve.visible = bVisible;

        if (managerSplineCurve_.m_listProcessorSpline.Count < 1)
        {
            return;
        }

        for (int iSeqSpline = 0; iSeqSpline < managerSplineCurve_.m_listProcessorSpline.Count; ++iSeqSpline)
        {
            m_combo_LayerCurve.InsertNewItem("> CurvePath" + iSeqSpline);
        }

        managerSplineCurve_.setActivitySpline(0, true);
        managerSplineCurve_.BuildupCurveCurrent(E_TYPE_SPLINE.SPLINE_NULL, -1);
        managerSplineCurve_.Set_DrawLineCurve(true);

        if (iSelectCurve >= 0 && iSelectCurve < m_combo_LayerCurve.getCount())
        {
            m_combo_LayerCurve.SetSelectItem(iSelectCurve);
        }
    } 

    //@ Set ComboBox Curve
    public void SetComboBox_ModeltypeUnitwalking(int[] arrModeltype, int iSelectCurve, bool bVisible)
    {
        m_combo_modeltype_unitwalking.Initialize(m_propertyInputGUI._rect_Combo_UnitwalkModelType.xMin,
                                      m_propertyInputGUI._rect_Combo_UnitwalkModelType.yMin,
                                      m_propertyInputGUI._rect_Combo_UnitwalkModelType.width,
                                      m_propertyInputGUI._rect_Combo_UnitwalkModelType.height, "ModelUnitWalking");

        m_combo_modeltype_unitwalking.SetClear();
        m_combo_modeltype_unitwalking.visible = bVisible;

        if (arrModeltype.Length < 1)
        {
            return;
        }

        foreach (int modelID in arrModeltype)
        {
            m_combo_modeltype_unitwalking.InsertNewItem(">unit(" + modelID + ")");
        }

        if (iSelectCurve >= 0 && iSelectCurve < m_combo_modeltype_unitwalking.getCount())
        {
            m_combo_modeltype_unitwalking.SetSelectItem(iSelectCurve);
        }
    } 

    //@ Input Process Camera 
    //  Rotation camera, camera move forward, back...
    void Update_inputControl_CameraMain()
    {
        float fTimeDelta = Time.deltaTime;

        if (Input.GetMouseButton(1))
        {
            float fSpeedRotCurr = _fSpdRot_CAM * fTimeDelta;

            _v2Rot_CAM.x += Input.GetAxis("Mouse X") * fSpeedRotCurr;
            _v2Rot_CAM.y -= Input.GetAxis("Mouse Y") * fSpeedRotCurr;

            m_camera_SceneMain.transform.rotation = Quaternion.Euler(_v2Rot_CAM.y, _v2Rot_CAM.x, 0);
        }

        if (Input.GetMouseButton(2))
        {
            m_camera_SceneMain.transform.Translate(-(Input.GetAxis("Mouse X")), -(Input.GetAxis("Mouse Y")), 0);
        } // if (Input.GetMouseButton(0))

        //@ Front, Back, Left, Right
        float fInputHori = Input.GetAxis("Horizontal");
        float fInputVert = Input.GetAxis("Vertical");
        float fWeightMoveSpeed = 1.0f;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _bAccelMove_CAM = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _bAccelMove_CAM = false;
        }

        if (true == _bAccelMove_CAM)
        {
            fWeightMoveSpeed *= _fSpdMoveAccel_CAM;
        }

        if (fInputHori + fInputVert != 0.0f)
        {
            Vector3 v3PosCam = m_camera_SceneMain.transform.position;
            Vector3 v3DirCam = m_camera_SceneMain.transform.forward;
            Vector3 v3RightCam = m_camera_SceneMain.transform.right;
            float fSpeedCamCurr = _fSpdMove_CAM * fTimeDelta * fWeightMoveSpeed;

            v3PosCam += (v3RightCam * (fInputHori * fSpeedCamCurr));
            v3PosCam += (v3DirCam * (fInputVert * fSpeedCamCurr));

            m_camera_SceneMain.transform.position = v3PosCam;
        }


        //@ Zoom In Out Move
        float fWheelMoveCurr = Input.GetAxis("Mouse ScrollWheel");
        if (fWheelMoveCurr != 0.0f)
        {
            float fDistanceWheelCurr = fWheelMoveCurr * _fSpdMoveWheel_CAM * fTimeDelta * fWeightMoveSpeed;

            Vector3 v3PosCam = m_camera_SceneMain.transform.position;
            Vector3 v3DirCam = m_camera_SceneMain.transform.forward;

            v3PosCam += v3DirCam * fDistanceWheelCurr;

            m_camera_SceneMain.transform.position = v3PosCam;
        }

        _v2MousePoint_previous.x = Input.GetAxis("Mouse X");
        _v2MousePoint_previous.y = Input.GetAxis("Mouse Y");
    } // void Update_inputControl_CameraMain()

    //@ Batch, Delete objects on user request position.
    void Update_inputControl_Unitwalking()
    {
        //@ Be Ready to Specific Mode - Object Batch
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _bReadyToAssign_OBJ = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _bReadyToAssign_OBJ = false;
        }

        //@ Batch Object    // �����̽��� + ���콺 ��Ŭ���� ��ŷ���� �Ѱ��� ����
        if (true == _bReadyToAssign_OBJ)
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool bIntersected = rayIntersect_MousePoint(out _rayhitNaviMesh_OBJ,
                                                            8);

                if (true == bIntersected)
                {
                    Vector3 v3PosIntersected = _rayhitNaviMesh_OBJ.point;

                    //@ UnitModel using UnitFactory
                    int iModelType = m_arrIDUnitModel[m_combo_modeltype_unitwalking.GetIdxSelected()];


                    IntervalUnitWalking.newUnitWalkingModel(v3PosIntersected, iModelType, _randomUnitPresent);
                }
            } 
             
            //@ Picking UnitWalking deletion.
            if (Input.GetMouseButtonDown(1))
            {
                
            }

        } // if( true==_bReadyToAssign_OBJ )
        //@ Batch Object

    } // void Update_inputControl_Unitwalking()

    void Update_inputControl_TestDebug()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _bOutputDetectLeaks = !_bOutputDetectLeaks;
        }

        if(Input.GetKeyDown(KeyCode.F2))
        {
            _bRequestRepresentPanelNavi = true;
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            _bOnGUI = !_bOnGUI;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            _bRequestRepresentStartToGoal = true;
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            GameContext.GetInstance.GameSpeed = GameContext.GetInstance.GameSpeed + gamespeed_unit;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            GameContext.GetInstance.GameSpeed = GameContext.GetInstance.GameSpeed - gamespeed_unit;
        }

        if (true == _bRequestRepresentPanelNavi)
        {
            _bRepresentPanelNavi = !_bRepresentPanelNavi;
            _bRequestRepresentPanelNavi = false;

            m_toolmoduleNavimesh.drawAllTri_eachFunctional(m_meshNavi_input.triCollector, _bRepresentPanelNavi);
        }

        if (true == _bRequestRepresentStartToGoal)
        {
            _bRepresentStartToGoal = !_bRepresentStartToGoal;
            _bRequestRepresentStartToGoal = false;

            if (true == _bRepresentStartToGoal)
            {
                m_toolmoduleNavimesh.drawStartCellToGoal(m_meshNavi_input.navigation, m_triCollector);
            }
            else
            {
                m_toolmoduleNavimesh.UndrawStartCellToGoal();
            }
        }
    } // void Update_inputControl_TestDebug()

    //@ Input Process for test verify navigation mesh.
    void Update_inputControl_NaviMesh()
    {
        //@ Render for Debug Line, Plane
        if (Input.GetKeyDown(KeyCode.F6))
        {
            _bReadyToRenderLineDebug_NAVIMESH = !_bReadyToRenderLineDebug_NAVIMESH;

            if (true == _bReadyToRenderLineDebug_NAVIMESH)
            {
                m_toolmoduleNavimesh._drawRenderLine_AllTris_Debug( m_meshNavi_input.triCollector );
            }
            else
            {
                m_toolmoduleNavimesh._desetAllTris_RenderLine_Debug();
            }
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            _bReadyToRenderMeshDebug_NAVIMESH = !_bReadyToRenderMeshDebug_NAVIMESH;

            if (true == _bReadyToRenderMeshDebug_NAVIMESH)
            {
                m_toolmoduleNavimesh.drawAllTrisOneDrawcall(m_meshNavi_input.triCollector.getTris(), DFLT_COLOR_DRAWALLTRIANGLES_DEBUG);
            }
            else
            {
                m_toolmoduleNavimesh.undrawAllTri();
            }
        }

    } // void Update_inputControl_NaviMesh()

    //@ Process input for navigation, Insert GOALS, BLOCKS
    void Update_inputControl_Navigation()
    {
        //@ Be Ready to Specific Mode - Navigation Batch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _bReadyToAssign_NAVI = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _bReadyToAssign_NAVI = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            _bReadyToAssign_NAVIADJACENT = !_bReadyToAssign_NAVIADJACENT;

            if (false == _bReadyToAssign_NAVIADJACENT)
            {
                int iBasisUniqueNum = CMATH.INT_MAX_DIV2;
                m_drawRenderMesh.DeleteDrawMesh_Range(
                                            iBasisUniqueNum,
                                            m_triCollector.getCountTris());
            }

            if (true == _bReadyToAssign_NAVIADJACENT)
            {
                if (true == m_meshNavi_input.navigation.didBuildupGoal())
                {
                    RaycastHit raycastHit;
                    bool bIntersected = rayIntersect_MousePoint(out raycastHit, 8);

                    if (true == bIntersected)
                    {
                        int iTriLandingCurr = raycastHit.triangleIndex;
                        List<int> listTrisAdj = new List<int>();

                        m_meshNavi_input.navigation.getAdjacentTris(iTriLandingCurr, ref listTrisAdj);

                        if (listTrisAdj.Count > 0)
                        {
                            int iBasisUniqueNum = CMATH.INT_MAX_DIV2;

                            m_drawRenderMesh.DeleteDrawMesh_Range(
                                                        iBasisUniqueNum,
                                                        m_triCollector.getCountTris());

                            for (int iTriAdj = 0; iTriAdj < listTrisAdj.Count; ++iTriAdj)
                            {
                                int iSeqUniqueNum = iBasisUniqueNum + listTrisAdj[iTriAdj];
                                CTRI triAdj_ = m_triCollector.getTri(listTrisAdj[iTriAdj]);

                                m_drawRenderMesh.DrawNewRendermesh(iSeqUniqueNum,
                                                                    triAdj_._arrv3PT,
                                                                    Color.green,
                                                                    CMATH.FEPSILON_F2,
                                                                    CDrawRenderMesh.scaleadjust_render_093);
                            }
                        }
                    } // if(true == bIntersected)
                } // if (true == m_meshNavi_input.navigation.didBuildupGoal())
            } // if(true==_bReadyToAssign_NAVIADJACENT)
        } // if (Input.GetKeyDown(KeyCode.C))

        //@ Batch Navigation
        if (false == _bReadyToAssign_NAVI)
        {
            // pick for block select type
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool bIntersectedMousePoint = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                if (true == bIntersectedMousePoint)
                {
                    int iIdxTri = raycastHit.triangleIndex;
                    m_baseBlockCollector.takeSelectedBase_bytri(iIdxTri);
                }
            }
        }
        else
        {
            //@ Assign Blocks
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool bIntersected = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);

                int iIdxTri = raycastHit.triangleIndex;

                if (true == bIntersected)
                {
                    bool bExistedAlreadyOther = (-1 < m_triCollector.m_listTris_naviGoal.BinarySearch(iIdxTri));
                    bExistedAlreadyOther |= m_triCollector.searchtriByLinear(iIdxTri, m_triCollector.m_listTris_naviStart);

                    if (false == bExistedAlreadyOther)
                    {
                        //@ Block Cell
                        bool bExistedAlreadyBlock = (-1 < m_triCollector.m_listTris_naviBlock.BinarySearch(iIdxTri));
                        bool bExistedAlreadyBlockRoad = (-1 < m_triCollector.m_listTris_naviBlockRoad.BinarySearch(iIdxTri));

                        m_baseBlockCollector.newSquareBaseBlockFromTri(iIdxTri, false, m_triCollector);
                        m_baseBlockCollector._SetDrawDebug_Base__All();

                        //@ Check existance blocks, then delete or not.
                        if (true == (bExistedAlreadyBlock | bExistedAlreadyBlockRoad))
                        {
                            if (true == bExistedAlreadyBlock)
                            {
                                m_triCollector.m_listTris_naviBlock.Remove(iIdxTri);
                                int iHypotenuseTri = m_triCollector.searchTriHypotenuse(iIdxTri);
                                if (iHypotenuseTri > -1)
                                {
                                    m_triCollector.m_listTris_naviBlock.Remove(iHypotenuseTri);
                                }

                                m_triCollector.m_listTris_naviBlock.Sort();
                            }

                            if (true == bExistedAlreadyBlockRoad)
                            {
                                m_triCollector.m_listTris_naviBlockRoad.Remove(iIdxTri);
                                int iHypotenuseTri = m_triCollector.searchTriHypotenuse(iIdxTri);
                                if (iHypotenuseTri > -1)
                                {
                                    m_triCollector.m_listTris_naviBlockRoad.Remove(iHypotenuseTri);
                                }

                                m_triCollector.m_listTris_naviBlockRoad.Sort();
                            }
                        } // if (true == (bExistedAlreadyBlock | bExistedAlreadyBlockRoad))
                        else
                        {
                            if (_bBlockTypeRoad)
                            {
                                m_triCollector.m_listTris_naviBlockRoad.Add(iIdxTri);
                                int iHypotenuseTri = m_triCollector.searchTriHypotenuse(iIdxTri);
                                if (iHypotenuseTri > -1)
                                {
                                    m_triCollector.m_listTris_naviBlockRoad.Add(iHypotenuseTri);
                                }

                                m_triCollector.m_listTris_naviBlockRoad.Sort();
                            }
                            else
                            {
                                m_triCollector.m_listTris_naviBlock.Add(iIdxTri);
                                int iHypotenuseTri = m_triCollector.searchTriHypotenuse(iIdxTri);
                                if (iHypotenuseTri > -1)
                                {
                                    m_triCollector.m_listTris_naviBlock.Add(iHypotenuseTri);
                                }
                                m_triCollector.m_listTris_naviBlock.Sort();
                            }

                        } // if (true == (bExistedAlreadyBlock | bExistedAlreadyBlockRoad))

                        m_triCollector._bReadyToExecuteBuildup_NAVI = true;

                    } // if (false == bExistedAlreadyOther)
                }
            } // if(Input.GetMouseButtonDown(0))

            //@ Assign Starts
            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit raycastHit;
                bool bIntersected = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                int iIdxTri = raycastHit.triangleIndex;

                if (true == bIntersected)
                {
                    bool bExistedAlreadyOthers = m_triCollector.m_listTris_naviBlock.Remove(iIdxTri);
                    bExistedAlreadyOthers |= m_triCollector.m_listTris_naviBlockRoad.Remove(iIdxTri);
                    bExistedAlreadyOthers |= m_triCollector.m_listTris_naviGoal.Remove(iIdxTri);

                    if (false == bExistedAlreadyOthers)
                    {
                        bool bExistedAlready = m_triCollector.searchtriByLinear(iIdxTri, m_triCollector.m_listTris_naviStart);

                        if (true == bExistedAlready)
                        {
                            //m_triCollector.m_listTris_naviStart.Remove(iIdxTri);
                            //m_triCollector.m_listTris_naviStart.Sort();

                            m_triCollector.deletetriByLinear(iIdxTri, ref m_triCollector.m_listTris_naviStart);

                            m_intervalUnitwalking.deleteInterval(iIdxTri);

                        }
                        else
                        {
                            m_triCollector.m_listTris_naviStart.Add(iIdxTri);
                            //m_triCollector.m_listTris_naviStart.Sort();

                            Vector3 v3PosCenterTri = m_triCollector.getTri(iIdxTri).GetCenterTri();
                            m_intervalUnitwalking.newIntervalConstantly(iIdxTri,
                                                            _respawnunitwalking,
                                                            _timeinterval_unitwalking,
                                                            v3PosCenterTri);
                        }

                        m_toolmoduleNavimesh._drawText3D_SeqCells(m_triCollector, m_triCollector.m_listTris_naviStart);
                    } // if(false==bExistedAlreadyStart)

                    m_triCollector._bReadyToExecuteBuildup_NAVI = true;
                } // if(true == bIntersected)


            } // if(Input.GetMouseButtonDown(2))
            //@ Assign Starts

            //@ Assign Goals
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit raycastHit;
                bool bIntersected = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);

                int iIdxTri = raycastHit.triangleIndex;
                if (true == bIntersected)
                {
                    bool bExistedAlreadyOthers = (-1 < m_triCollector.m_listTris_naviBlock.BinarySearch(iIdxTri));
                    bExistedAlreadyOthers |= (-1 < m_triCollector.m_listTris_naviBlockRoad.BinarySearch(iIdxTri));
                    bExistedAlreadyOthers |= m_triCollector.searchtriByLinear(iIdxTri, m_triCollector.m_listTris_naviStart);

                    if (false == bExistedAlreadyOthers)
                    {
                        bool bExistedAlready = (-1 < m_triCollector.m_listTris_naviGoal.BinarySearch(iIdxTri));
                        int iHypotenuseTri = m_triCollector.searchTriHypotenuse(iIdxTri);
                        if (iHypotenuseTri > -1)
                        {
                            bExistedAlready = bExistedAlready | (-1 < m_triCollector.m_listTris_naviGoal.BinarySearch(iHypotenuseTri));
                        }

                        if (true == bExistedAlready)
                        {
                            bool bResultDeleted = m_triCollector.m_listTris_naviGoal.Remove(iIdxTri);
                            if (true == bResultDeleted)
                            {
                                m_triCollector.m_listTris_naviGoal.Sort();
                            }

                            if (iHypotenuseTri > -1)
                            {
                                bResultDeleted = m_triCollector.m_listTris_naviGoal.Remove(iHypotenuseTri);
                            }

                            m_triCollector.m_listTris_naviGoal.Sort();
                        }
                        else
                        {
                            m_triCollector.m_listTris_naviGoal.Add(iIdxTri);
                            if (iHypotenuseTri > -1)
                            {
                                m_triCollector.m_listTris_naviGoal.Add(iHypotenuseTri);
                            }

                            m_triCollector.m_listTris_naviGoal.Sort();
                        }

                        m_triCollector._bReadyToExecuteBuildup_NAVI = true;
                    } // if(false==bExistedAlreadyBlock)


                } // if(true == bIntersected)

            } // if(Input.GetMouseButtonDown(1))
            //@ Assign Goals //


        } // if(true == _bReadyToAssign_NAVI)

        //@ Mapping Navigation and Draw Navigation Way to Goal
        if (true == m_triCollector._bReadyToExecuteBuildup_NAVI)
        {

            if (m_triCollector.getCount_NaviFunction_Tris() > 0)
            {
                m_meshNavi_input.navigation.mappingNavigation(m_triCollector.m_listTris_naviGoal,
                                                        m_triCollector.m_listTris_naviBlock,
                                                        m_triCollector.m_listTris_naviBlockRoad,
                                                        m_triCollector.m_listTris_naviStart,
                                                        false);
            }

            m_toolmoduleNavimesh.drawAllTri_eachFunctional(m_meshNavi_input.triCollector, _bRepresentPanelNavi);

            m_triCollector._bReadyToExecuteBuildup_NAVI = false;

            #region fornavigationtool
            if (true == _bRepresentStartToGoal)
            {
                m_toolmoduleNavimesh.drawStartCellToGoal(m_meshNavi_input.navigation, m_triCollector);
            }
            #endregion 

        } // if(true==m_triCollector._bReadyToExecuteBuildup_NAVI)

    } // void Update_inputControl_Navigation()


    //@ Update batch basecore
    void Update_inputControl_BatchBaseCore()
    {
        if (null == m_baseCoreCollector)
        {
            return;
        }

        //@ Be Ready to Specific Mode - Navigation Batch
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = false;
        }

        if (true == _bReadyToAssign_BASE)
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit raycastHit;
                bool bIntersected = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                int iIdxTri = raycastHit.triangleIndex;

                if (true == bIntersected)
                {
                    bool bExistAlreadyBaseCore = (null != m_baseCoreCollector.findBaseByTri(iIdxTri));
                    if (true == bExistAlreadyBaseCore)
                    {
                        bool bDeletedBase = m_baseCoreCollector.delete_Base_byTri(iIdxTri);

                        if (true == bDeletedBase)
                        {
                            m_baseCoreCollector._SetDrawDebug_Base__All(); //@ Reset Draw Base 
                        }
                    }
                    else
                    {
                        bool bThis_GoalCell = (-1 < m_triCollector.m_listTris_naviGoal.BinarySearch(iIdxTri));

                        if (true == bThis_GoalCell)
                        {
                            m_baseCoreCollector.insertNewInstantTri(iIdxTri);
                        } 
                        else 
                        {
                            m_baseCoresubCollector.newSquareBaseFromTri(iIdxTri, false, m_triCollector);
                            m_baseCoresubCollector._SetDrawDebug_Base__All();
                        }
                    } // if (true == bExistAlreadyBaseCore)

                } // if (true == bIntersected)


            } // if(Input.GetMouseButtonDown(1))

        } //if(true==_bReadyToAssign_BASE)


        //@ Set new base core
        if (false == _bReadyToAssign_BASE)
        {
            List<int> listTriInstant_ = m_baseCoreCollector.getListInstantTri();
            if (0 < listTriInstant_.Count)
            {
                int iIdxBase = m_baseCoreCollector.setNewBase__();

                if (iIdxBase < 0)
                {
                    Debug.Log("Problem!! Not Inserted newBase. Base not Constructed!! m_baseCoreCollector.setNewBase(). ///"
                               );
                }

                m_baseCoreCollector._SetDrawDebug_Base__All();

                return;
            } 
            else
            {
                // pick for select type
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit raycastHit;
                    bool bIntersectedMousePoint = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                    if (true == bIntersectedMousePoint)
                    {
                        int iIdxTri = raycastHit.triangleIndex;
                        m_baseCoreCollector.takeSelectedBase_bytri(iIdxTri);
                    }
                } 
            }

        } // if(false == _bReadyToAssign_BASE)
    } // void Update_inputControl_BatchBaseCore()

    //@ Update batch baseStart
    void Update_inputControl_BatchBaseStart()
    {
        if (null == m_baseStartCollector)
        {
            return;
        }

        //@ Be Ready to Specific Mode - Navigation Batch
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = false;
        }

        if (true == _bReadyToAssign_BASE)
        {
            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit raycastHit;
                bool bIntersected = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                int iIdxTri = raycastHit.triangleIndex;

                if (true == bIntersected)
                {
                    bool bExistAlreadyBaseStart = (null != m_baseStartCollector.findBaseByTri(iIdxTri));
                    if (true == bExistAlreadyBaseStart)
                    {
                        bool bDeletedBase = m_baseStartCollector.delete_Base_byTri(iIdxTri);

                        if (true == bDeletedBase)
                        {
                            m_baseStartCollector._SetDrawDebug_Base__All(); //@ Reset Draw Base 
                        }
                    }
                    else
                    {
                        bool bThisis_StartCell = m_triCollector.searchtriByLinear(iIdxTri, m_triCollector.m_listTris_naviStart);

                        if (true == bThisis_StartCell)
                        {
                            m_baseStartCollector.insertNewInstantTri(iIdxTri);
                        } 
                    } 
                } // if (true == bIntersected)

            } // if(Input.GetMouseButtonDown(1))

        } //if(true==_bReadyToAssign_BASE)
        else
        {
            List<int> listTriInstant_ = m_baseStartCollector.getListInstantTri();
            if (0 < listTriInstant_.Count)
            {
                int iIdxBase = m_baseStartCollector.setNewBase__();

                if (iIdxBase < 0)
                {
                    Debug.Log("ERROR!! Not Inserted newBase.//");
                }

                m_baseStartCollector._SetDrawDebug_Base__All();

                return;
            } 
            else
            {
                // pick for select type
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit raycastHit;
                    bool bIntersectedMousePoint = m_meshNavi_input.rayIntersect_ScreenPoint(Input.mousePosition, out raycastHit);
                    if (true == bIntersectedMousePoint)
                    {
                        int iIdxTri = raycastHit.triangleIndex;
                        m_baseStartCollector.takeSelectedBase_bytri(iIdxTri);
                    }
                } 
            }

        } // if(false == _bReadyToAssign_BASE)
    } // void Update_inputControl_BatchBaseStart()

    void Update_inputControl_BatchBaseTower()
    {
        if (null == m_baseTowerCollector)
        {
            return;
        }

        //@ Be Ready to Specific Mode - Navigation Batch
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            _bReadyToAssign_BASE = false;   
        }

        if (true == _bReadyToAssign_BASE)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool bIntersected = rayIntersect_MousePoint(out raycastHit, 8 << 1);
                int iIdxTri = raycastHit.triangleIndex;

                if (true == bIntersected)
                {                 
                    bool bDeletedBase = m_baseTowerCollector.delete_Base_byTri(iIdxTri);

                    if (false == bDeletedBase)
                    {
                        if (false == m_baseTowerCollector.deleteInstantTriOnAB(iIdxTri))
                        {
                            bool bExistAlreadyAsGoal = (-1 < m_triCollector.m_listTris_naviGoal.BinarySearch(iIdxTri));

                            if (false == bExistAlreadyAsGoal)
                            {
                                m_baseTowerCollector.newSquareBaseFromTri(iIdxTri, false, m_triCollector);
                            }
                        }

                    }

                    m_baseTowerCollector._SetDrawDebug_Base__All();

                } 

            } // if(Input.GetMouseButtonDown(0))

        } //if(true==_bReadyToAssign_BASE)

    } // void Update_inputControl_BatchBaseTower()


    void Update_inputControl_CurvePathUnit()
    {
        //@ Be Ready to Specific Mode - Navigation Batch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _bReadyToActivate_FlyPath = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            _bReadyToActivate_FlyPath = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _bReadyToAssign_FlyPath = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _bReadyToAssign_FlyPath = false;
        }

        if (true == _bReadyToAssign_FlyPath)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool bIntersectedSelected = rayIntersect_MousePoint(out raycastHit, 10 << 1);
                Vector3 v3PositionOfNewCurvePath = getPosScreenToWorld_FarFrom(Input.mousePosition, DFLT_DISTANCEOF_CURVEPATHUNIT_FROM_CAM);

                if (true == bIntersectedSelected)
                {
                    CCurvePathUnit flypathUnit_hit = (CCurvePathUnit)(raycastHit.collider.gameObject.GetComponent("CCurvePathUnit"));
                    if (null == flypathUnit_hit)
                    {
                        m_SplineCurve.InsertNewCurvePathUnit_BLANK(v3PositionOfNewCurvePath);
                    }
                    else
                    {
                        m_SplineCurve.DeleteCurvePathUnitInclude(flypathUnit_hit);
                    }
                } 
                else
                {
                    m_SplineCurve.InsertNewCurvePathUnit_BLANK(v3PositionOfNewCurvePath);
                } 
            } // if (Input.GetMouseButtonDown(0))
        } // if (true == _bReadyToAssign_FlyPath)

        if (true == _bReadyToActivate_FlyPath)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                bool bIntersectedSelected = rayIntersect_MousePoint(out raycastHit, 10 << 1);

                if (true == bIntersectedSelected)
                {
                    CCurvePathUnit flypathUnit_hit = (CCurvePathUnit)(raycastHit.collider.gameObject.GetComponent("CCurvePathUnit"));

                    if (null != flypathUnit_hit)
                    {
                        m_SplineCurve.ActivateOrUnactivateCurvePoint(flypathUnit_hit);
                    } 
                } 
            }
        } // if(true == _bReadyToAssign_BASE)

    } // void Update_inputControl_CurvePathUnit()

    //@ GUI Input Control Select map
    void OnGUI_inputControl_SELMAP()
    {
        Rect rectWindow = new Rect(m_propertyInputGUI._rect_Text_NameNavimesh);
        GUI.TextField(rectWindow, m_datanavimeshs.getNaviMeshName(m_selectbox_Navimesh.GetIdxSelect()));

        string strBtnOutput = "";

        //@ Collect Perfect Squares for Navigation.
        strBtnOutput = "select navimesh";
        if (GUI.Button(m_propertyInputGUI._rect_BtnSelect_Navimesh, strBtnOutput))
        {
            _bExpendToSelectNavigation = !_bExpendToSelectNavigation;
        } 

        //@ Collect Perfect Squares for Navigation.
        strBtnOutput = "save all";
        if (GUI.Button(m_propertyInputGUI._rect_BtnSave_Navimesh, strBtnOutput))
        {
            _SaveAll();
        }

        //@ Selection Box Navigation Mesh
        if (true == _bExpendToSelectNavigation)
        {
            m_selectbox_Navimesh.visible = true;

            m_selectbox_Navimesh.OnGUI_SelectBoxCustom();

            if (true == m_selectbox_Navimesh.GetChangedSelect())
            {
                int iIDX_NavigationMesh = m_selectbox_Navimesh.GetIdxSelect();

                //@ Initialize all managers
                bool bResult = false;
                bResult = processCycle.GetInstance.resetGlobal(iIDX_NavigationMesh, false);
                if (false == bResult)
                {
                    Debug.Log("!ERROR resetGlobal. NaviMesh=" + iIDX_NavigationMesh + "//");
                }

                _bExpendToSelectNavigation = false;
            }
        } 
        else
        {
            m_selectbox_Navimesh.visible = false;
        }
    } // void OnGUI_inputControl_SELMAP()


#region panelNavigation
    void OnGUI_inputControl_EDITOR_TEST()
    {
        Vector2 v2ScreenSize = new Vector2();
        v2ScreenSize.x = Screen.width;
        v2ScreenSize.y = Screen.height;

        GUI.BeginGroup(new Rect(
                v2ScreenSize.x - m_propertyInputGUI._rect_GroupPanelTest.xMin,
                v2ScreenSize.y - m_propertyInputGUI._rect_GroupPanelTest.yMin,
                m_propertyInputGUI._rect_GroupPanelTest.width,
                m_propertyInputGUI._rect_GroupPanelTest.height));
        GUI.Box(new Rect(0, 0, 400, 190), "TEST FUNCTION");

        //@ Label : Navigation
        GUI.Label(new Rect(m_propertyInputGUI._v2Position_Label_Navigation.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                        m_propertyInputGUI._v2Position_Label_Navigation.y,
                        DFLT_V2SIZE_BUTTON.x,
                        DFLT_V2SIZE_BUTTON.y),
                        "NAVIGATION");

        //@ Label : Information
        GUI.Label(new Rect(m_propertyInputGUI._v2Position_Label_Information.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                            m_propertyInputGUI._v2Position_Label_Information.y,
                            DFLT_V2SIZE_BUTTON.x,
                            DFLT_V2SIZE_BUTTON.y),
                            "INFORMATION");

        string strBtnOutput;

        //@ Instant Object on StartCell of navigation
        strBtnOutput = UnitWalking.getStringStatus();

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnObjStateSwitch.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnObjStateSwitch.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            UnitWalking.SetStatus_UnitWalking_switching();
            m_meshNavi_input.navigation.setClearAllPortals();
        }


        //@ Object Instant  on StartCell of navigation
        strBtnOutput = "";
        if (m_intervalUnitwalking.workprocessInterval == true)
        {
            strBtnOutput = "obj_instant(on)";
        }
        else
        {
            strBtnOutput = "obj_instant(off)";
        }

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnObjStartInst.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnObjStartInst.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            m_intervalUnitwalking.workprocessInterval = !m_intervalUnitwalking.workprocessInterval;
        }

        //@ Object Clear 
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnObjClearAll.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnObjClearAll.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
            "clear_allunits"))
        {
            _UnitWalkingClearAll();
        }

        //@ BLOCKTYPE Toggle box.
        _bBlockTypeRoad = GUI.Toggle(m_propertyInputGUI._rect_Toggle_BlockTypeNAVICELL, _bBlockTypeRoad, "blocktype:road");

        if (null != m_toolmoduleNavimesh)
        {   
            //@ View level cells
            if (m_toolmoduleNavimesh.getDrawLevelCells() == true)
            {
                strBtnOutput = "ViewAllLevels(on)";
            }
            else
            {
                strBtnOutput = "ViewAllLevels(off)";
            }
            if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnDrawLevelCellsAll.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                    m_propertyInputGUI._v2Position_BtnDrawLevelCellsAll.y,
                                    DFLT_V2SIZE_BUTTON.x,
                                    DFLT_V2SIZE_BUTTON.y),
                                    strBtnOutput))
            {
                m_toolmoduleNavimesh.drawLevelAllCells(m_meshNavi_input.navigation, !(m_toolmoduleNavimesh.getDrawLevelCells()));
            }
        }

        //@ Clear Navigation
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnClearNavi.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnClearNavi.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                "clear_navi"))
        {
            _ClearNavi();
        }

        //@ Which startbase.
        if (true == m_combo_modeltype_unitwalking.IsClickedCombo())
        {

        }
        m_combo_modeltype_unitwalking.OnGUI_ComboBoxCustom();

        //@ Information
        string stroutputInformation = "1.GameSpeed(" + GameContext.GetInstance.GameSpeed.ToString()+")";
        stroutputInformation += "  2.CountAllUnits(" + UnitPool.GetInstance.GetUnitCount()+")";

        GUI.Label(new Rect(m_propertyInputGUI._v2Position_LabelInformation01.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_LabelInformation01.y,
                                DFLT_V2SIZE_LABEL_INFO.x,
                                DFLT_V2SIZE_LABEL_INFO.y),
                                stroutputInformation );
        


        GUI.EndGroup();
    } // void OnGUI_inputControl_EDITOR_TEST()
#endregion // panelNavigation

    //@ GUI Input Control Editor
    void OnGUI_inputControl_EDITOR()
    {
        Vector2 v2ScreenSize = new Vector2();
        v2ScreenSize.x = Screen.width;
        v2ScreenSize.y = Screen.height;

        GUI.BeginGroup(new Rect(
                        v2ScreenSize.x - m_propertyInputGUI._rect_GroupPanelBatch.xMin, 
                        v2ScreenSize.y - m_propertyInputGUI._rect_GroupPanelBatch.yMin, 
                        m_propertyInputGUI._rect_GroupPanelBatch.width, m_propertyInputGUI._rect_GroupPanelBatch.height));
        GUI.Box(new Rect(0, 0, 400, 400), "FUNCTION");

        //@ Label : Collect 
        GUI.Label(new Rect(m_propertyInputGUI._v2Position_Label_ToolMode.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                        m_propertyInputGUI._v2Position_Label_ToolMode.y, 
                        DFLT_V2SIZE_BUTTON.x, 
                        DFLT_V2SIZE_BUTTON.y),
                                        "MODE");

        //@ Label : Spline Curve
        GUI.Label(new Rect(m_propertyInputGUI._v2Position_Label_CurveSpline.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                            m_propertyInputGUI._v2Position_Label_CurveSpline.y, 
                            DFLT_V2SIZE_BUTTON.x, 
                            DFLT_V2SIZE_BUTTON.y),
                            "SPLINE CURVE");

        //@ Label : Base
        GUI.Label(new Rect(m_propertyInputGUI._v2Position_Label_Base.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                            m_propertyInputGUI._v2Position_Label_Base.y, 
                            DFLT_V2SIZE_BUTTON.x, 
                            DFLT_V2SIZE_BUTTON.y),
                            "BASEMENT");


        //@ _Mode Change
        string strBtnOutput = "";
        switch (_eInput_Mode)
        {
            case E_INPUT_MODE.E_INPUT_MODE_NAVIGATION:
                {
                    strBtnOutput = "mode(navi)";
                }
                break;
            case E_INPUT_MODE.E_INPUT_MODE_CURVELINE:
                {
                    strBtnOutput = "mode(curve)";
                }
                break;
        } 

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnModeChange.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnModeChange.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
            strBtnOutput))
        {
            _ModeSwitching();


        }

        //@ Load All
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnReload_ALL.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnReload_ALL.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                "reload_all"))
        {
            int iIdxNaviMesh = m_selectbox_Navimesh.GetIdxSelect();

            if (iIdxNaviMesh < 0 || iIdxNaviMesh >= m_selectbox_Navimesh.getCount())
            {
                iIdxNaviMesh = 0;
            }

            //@ Initialize all managers
            bool bResult = false;
            bResult = processCycle.GetInstance.resetGlobal(iIdxNaviMesh, false);
            if (false == bResult)
            {
                Debug.Log("Error. OnGUI_inputControl_SELMAP()//resetGlobal//NaviMesh=" + iIdxNaviMesh + "//");
            } 
        }


        //@ Clear All(Navi, Base, Object Dyn)
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnClearAll.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnClearAll.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                "clear_all"))
        {
            _ClearAll();
            Debug.Log("Clear All. Complete!");
        }

        //@ Collect Perfect Squares for Navigation.
        strBtnOutput = "CollectSqre(Tower)";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnCollectPerfectSquares.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnCollectPerfectSquares.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            List<CBASE__> listBase = new List<CBASE__>();
            CBaseCollector.CollectAll_perfectSqre_exceptGoalTri(m_meshNavi_input.navigation.getNaviCellsAll(), m_triCollector, ref listBase);

            m_baseTowerCollector.setNewBase_FromListBase(listBase);
            m_baseTowerCollector._SetDrawDebug_Base__All();
        }


        //@ Which curve selected. combo box
        m_combo_LayerCurve.OnGUI_ComboBoxCustom();
        if (true == m_combo_LayerCurve.IsClickedCombo())
        {
            if (true == m_combo_LayerCurve.IsLastSelected())
            {
                int iProcessLine = m_SplineCurve.setNewProcessorSpline();
                m_combo_LayerCurve.InsertNewItem("> CurvePath" + iProcessLine);
            }
            else
            {
                int iIdxCurve = m_combo_LayerCurve.GetIdxSelected();
                int iResult = m_SplineCurve.setActivitySpline(iIdxCurve, true);

                if (-1 < iResult)
                {
                    if (null != m_SplineCurve.getLineCurvePnt_Activate() &&
                        0 < m_SplineCurve.getLineCurvePnt_Activate().Length)
                    {
                        m_SplineCurve.newAssignLineGuideActive(iIdxCurve);
                        UnitFlying._MovementUnitFlying = true;
                    }
                }
            } 
        } 

        //@ Which curvetype selected. 
        m_combo_TypeCurve.OnGUI_ComboBoxCustom();

        E_TYPE_SPLINE eTypeCurve_Sel;
        strBtnOutput = "build_curve" + "(" + m_SplineCurve.GetCountPntCurve() + ")";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnBuildCurve.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnBuildCurve.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            if (m_SplineCurve.GetCountPntCurve() > 0)
            {
                eTypeCurve_Sel = (E_TYPE_SPLINE)m_combo_TypeCurve.GetIdxSelected();

                m_SplineCurve.BuildupCurveCurrent(eTypeCurve_Sel,
                                                    CSplineGenerator.DFLT_WEIGHT_TIMEDIVIDE);

                if (null != m_SplineCurve.getLineCurvePnt_Activate() && m_SplineCurve.getLineCurvePnt_Activate().Length > 0)
                {
                    int iIdxCurve = m_combo_LayerCurve.GetIdxSelected();

                    //@ Apply all point of Curve to FlyManager
                    m_SplineCurve.newAssignLineGuideActive(iIdxCurve);
                    UnitFlying._MovementUnitFlying = true;

                    m_SplineCurve.Set_DrawLineCurve(true);
                }
            }
            else
            {
                Debug.Log("Please select curve point first. There's no selection curve point.");
            }
        }

        //@ Delete Curve 
        strBtnOutput = "Delete_SelCurve";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnDeleteSelectCurve.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnDeleteSelectCurve.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            int iIdxSelected = m_combo_LayerCurve.GetIdxSelected();
            m_SplineCurve.DeleteCurve(iIdxSelected, true);

            int iCountSpline = m_SplineCurve.getCountSpline();
            Debug.Log("strBtnOutput = Delete_SelCurve//iCountSpline=" + iCountSpline);

            SetComboBox_Curve(ref m_SplineCurve, DFLT_SELECTED_CURVE, true);
        }

        //@ Clear Curve Line
        strBtnOutput = "";
        if (true == _bDoDrawCurveLine)
        {
            strBtnOutput = "GuideLine(ON)";
        }
        else
        {
            strBtnOutput = "GuideLine(OFF)";
        }

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnClearCurveLine.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnClearCurveLine.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            _bDoDrawCurveLine = !_bDoDrawCurveLine;

            m_SplineCurve.Set_DrawLineCurve(_bDoDrawCurveLine);
        }

        //@ Show ALL Curve
        strBtnOutput = "";
        if (true == _bShowAllCurveLine)
        {
            strBtnOutput = "showAllCur(on)";
        }
        else
        {
            strBtnOutput = "showAllCur(off)";
        }

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnShowAllCurve.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnShowAllCurve.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            _bShowAllCurveLine = !(_bShowAllCurveLine);
            if (_bShowAllCurveLine)
            {
                m_SplineCurve.setActivitySplines_ALL(true);
            }
            else
            {
                m_SplineCurve.setUnActivitySplines_ALL();
            }
        }

        //@ New curve path
        strBtnOutput = "new_curvepath";
        if (GUI.Button(new Rect(m_propertyInputGUI._v3Position_BtnNewCurvePath.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v3Position_BtnNewCurvePath.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            Vector3 v3ScreenCenter2D = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f);
            Vector3 v3PositionOfNewCurvePath = getPosScreenToWorld_FarFrom(v3ScreenCenter2D, DFLT_DISTANCEOF_CURVEPATHUNIT_FROM_CAM);

            m_SplineCurve.InsertNewCurvePathUnit_BLANK(v3PositionOfNewCurvePath);
        }

        //@ New curve path
        strBtnOutput = "del_curvepaths";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnDeleteCurvePath.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnDeleteCurvePath.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            m_SplineCurve.DeleteCurvePathUnitAll_Unselect();
        }


        //@ New Test FlyUnit
        strBtnOutput = "new_flyunit";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnNewTestFlyObj.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnNewTestFlyObj.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            int iSeqActivateSpline = m_SplineCurve.getActivityProcessorSpline();

            UnitFlying unitFlyingNew = (UnitFlying)GameObject.Instantiate(
                            processCycle.GetInstance.m_unitFlying_src);

            unitFlyingNew.seqSplineGuide = iSeqActivateSpline;
            unitFlyingNew.InitUnitMovement();
            unitFlyingNew.transform.root.localScale = unitFlyingNew.transform.root.localScale * 0.5f;
        }

        //@ Fly Object State
        strBtnOutput = "";
        if (true == UnitFlying._MovementUnitFlying)
        {
            strBtnOutput = "curveguide(play)";
        }
        else
        {
            strBtnOutput = "curveguide(stop)";
        }

        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnSwitchStateFlyunit.x + m_propertyInputGUI._fMarginfromLeft_Row01, 
                                m_propertyInputGUI._v2Position_BtnSwitchStateFlyunit.y, 
                                DFLT_V2SIZE_BUTTON.x, 
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            UnitFlying._MovementUnitFlying = !UnitFlying._MovementUnitFlying;
        }

        //@ Clear Test FlyUnit
        strBtnOutput = "clear_allunits";
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnClearTestFlyObj.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnClearTestFlyObj.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                strBtnOutput))
        {
            UnitPool.GetInstance.TruncateAll();
        }

        //@ Which corebase.
        if (true == m_combo_CoreBaseType.IsClickedCombo())
        {
            int iIdxTypeCore = m_combo_CoreBaseType.GetIdxSelected();

            m_baseCoreCollector.SetType_Base_Selected(iIdxTypeCore);
        }
        m_combo_CoreBaseType.OnGUI_ComboBoxCustom();

        //@ Which startbase.
        if (true == m_combo_StartBaseType.IsClickedCombo())
        {
            int iIdxTypeStart = m_combo_StartBaseType.GetIdxSelected();

            m_baseStartCollector.SetType_Base_Selected(iIdxTypeStart);
        }
        m_combo_StartBaseType.OnGUI_ComboBoxCustom();


        //@ Which blockbase.
        if (true == m_combo_BlockBaseType.IsClickedCombo())
        {
            int iIdxTypeBlock = m_combo_BlockBaseType.GetIdxSelected();

            m_baseBlockCollector.SetType_Base_Selected(iIdxTypeBlock);
        }
        m_combo_BlockBaseType.OnGUI_ComboBoxCustom();

        //@ Clear BaseTower
        if (GUI.Button(new Rect(m_propertyInputGUI._v2Position_BtnClearBase.x + m_propertyInputGUI._fMarginfromLeft_Row01,
                                m_propertyInputGUI._v2Position_BtnClearBase.y,
                                DFLT_V2SIZE_BUTTON.x,
                                DFLT_V2SIZE_BUTTON.y),
                                "clear_base"))
        {
            _ClearBase();
        }

        GUI.EndGroup();
    } // void OnGUI_inputControl_EDITOR()

    void UpdateProcess_WindowTitle(int iWindowID)
    {
        //GUI.TextField(new Rect(1.0f, 1.0f, 250.0f, 50.0f), "Navigation / Base Tool");
    }

    public void DestructInput()
    {
        if (null != m_drawRenderMesh)
        {
            m_drawRenderMesh.DeleteDrawMesh_All();
        }
    }

    void OnDestroy()
    {
        DestructInput();

        if (m_combo_CoreBaseType)
            Destroy(m_combo_CoreBaseType.gameObject);

        if (m_combo_StartBaseType)
            Destroy(m_combo_StartBaseType.gameObject);

        if (m_combo_BlockBaseType)
            Destroy(m_combo_BlockBaseType.gameObject);

        if (m_combo_LayerCurve)
            Destroy(m_combo_LayerCurve.gameObject);

        if (m_combo_modeltype_unitwalking)
            Destroy(m_combo_modeltype_unitwalking.gameObject);

        if (m_combo_TypeCurve)
            Destroy(m_combo_TypeCurve.gameObject);

        if (m_selectbox_Navimesh)
            Destroy(m_selectbox_Navimesh.gameObject);
    }

    void _UnitWalkingClearAll()
    {
        UnitPool.GetInstance.TruncateAll();
    }

    void _ClearNavi()
    {
        m_triCollector.Clear_functionalTris();

        //@ Release & Clear Triangle, DrawMesh, Object Dyn
        m_drawRenderMesh.DeleteDrawMesh_All();


        List<CTRI> listTris;
        m_triCollector.getTris(out listTris);
        m_meshNavi_input.navigation.ConstructNavi(listTris);
    } // void _ClearAll()

    void _ClearBase()
    {
        //@ Release & Clear Triangle, DrawMesh, Object Dyn
        m_baseTowerCollector.ClearBaseAll();
        m_baseCoreCollector.ClearBaseAll();
        m_baseCoresubCollector.ClearBaseAll();
        m_baseStartCollector.ClearBaseAll();
        m_baseBlockCollector.ClearBaseAll();
    }

    void _ClearCurvepath()
    {
        m_SplineCurve.destructSplinecurveAll();
        UnitPool.GetInstance.TruncateAll();
    }

    void _ClearAll()
    {
        processCycle.GetInstance.resetGlobal(m_selectbox_Navimesh.GetIdxSelect(), true); 
    } // void _ClearAll()


    //@ Process : Save 
    void _SaveAll()
    {
        //  MapSave
        m_stageMap.Release();
        _SaveNavigation_toCMapTemplate();
        _SaveBaseTower_toCMapTemplate();
        _SaveBaseCore_toCMapTemplate();
        _SaveBaseCoreSub_toCMapTemplate();

        _SaveBaseStart_toCMapTemplate();
        _SaveBaseBlock_toCMapTemplate();

        _SaveSplineCurve_ToCmapTemplate();

        m_stageMap.m_Filename = m_datanavimeshs.getNaviMeshName(m_selectbox_Navimesh.GetIdxSelect());
        m_stageMap.SaveStage();   
    } // void _SaveAll()

    void _SaveNavigation_toCMapTemplate()
    {
        //  Save triangle mesh
        foreach (CTRI tri in m_triCollector.m_listTris)
            m_stageMap.AddTriangle(tri);

        //  Save block point
        foreach (int block in m_triCollector.m_listTris_naviBlock)
            m_stageMap.AddGroundBlockPoint(block);

        //  Save blockroad point
        foreach (int blockroad in m_triCollector.m_listTris_naviBlockRoad)
            m_stageMap.AddGroundBlockRoadPoint(blockroad);

        //  Save goal point
        foreach (int block in m_triCollector.m_listTris_naviGoal)
            m_stageMap.AddGoalPoint(block);

        //  Save GroudStartPoint
        foreach (int block in m_triCollector.m_listTris_naviStart)
            m_stageMap.AddGroundStartPoint(block);
    } // void _SaveNavigation_toCMapTemplate()

    void _SaveBaseTower_toCMapTemplate()
    {
        //  Save BaseTower
        foreach (CBASE__ arrBase in m_baseTowerCollector.m_listBase__)
            m_stageMap.AddBaseTower(arrBase);       
    }

    void _SaveBaseCore_toCMapTemplate()
    {
        BaseInfo core = null;
        //  Save BaseCore
        foreach (CBASE__ arrBase in m_baseCoreCollector.m_listBase__)
        {
            core = new BaseInfo();
            core.Type = arrBase.getIdxType();
            core.CellIndex = new List<int>(arrBase._listIdxTris);
            core.CoreTriPnt = new List<Vector3>(arrBase._listv3Pnts);
            core.CoreTriPntSrc = new List<Vector3>(arrBase._listv3PntsSrc);
            core.CenterPos = arrBase._v3PositionCenter;
            m_stageMap.AddMainCore(core);
        }
    }

    void _SaveBaseCoreSub_toCMapTemplate()
    {
        BaseInfo core = null;
        //  Save BaseCore
        foreach (CBASE__ arrBase in m_baseCoresubCollector.m_listBase__)
        {
            core = new BaseInfo();
            core.Type = arrBase.getIdxType();
            core.CellIndex = new List<int>(arrBase._listIdxTris);
            core.CoreTriPnt = new List<Vector3>(arrBase._listv3Pnts);
            core.CoreTriPntSrc = new List<Vector3>(arrBase._listv3PntsSrc);
            core.CenterPos = arrBase._v3PositionCenter;
            m_stageMap.AddSubCore(core);
        }
    }

    void _SaveBaseStart_toCMapTemplate()
    {
        BaseInfo core = null;

        //Save Startbase
        foreach (CBASE__ arrBase in m_baseStartCollector.m_listBase__)
        {
            core = new BaseInfo();
            core.Type = arrBase.getIdxType();
            core.CellIndex = new List<int>(arrBase._listIdxTris);
            core.CoreTriPnt = new List<Vector3>(arrBase._listv3Pnts);
            core.CoreTriPntSrc = new List<Vector3>(arrBase._listv3PntsSrc);
            core.CenterPos = arrBase._v3PositionCenter;
            m_stageMap.AddStartBase(core);
        }
    }

    void _SaveBaseBlock_toCMapTemplate()
    {
        BaseInfo basement = null;
        //  Save BaseBlock
        foreach (CBASE__ arrBase in m_baseBlockCollector.m_listBase__)
        {
            basement = new BaseInfo();
            basement.Type = arrBase.getIdxType();
            basement.CellIndex = new List<int>(arrBase._listIdxTris);
            basement.CoreTriPnt = new List<Vector3>(arrBase._listv3Pnts);
            basement.CoreTriPntSrc = new List<Vector3>(arrBase._listv3PntsSrc);
            basement.CenterPos = arrBase._v3PositionCenter;
            m_stageMap.AddBlockBase(basement);
        }
    }

    void _SaveSplineCurve_ToCmapTemplate()
    {
        foreach (CSplineGenerator Spline in m_SplineCurve.m_listProcessorSpline)
        {
            //  Save - AddFlyPath 
            m_stageMap.AddFlyPath(Spline);
        }
    } 

    void _ModeSwitching()
    {
        _eInput_Mode = (E_INPUT_MODE)((((int)_eInput_Mode) + 1) % (int)(E_INPUT_MODE.E_INPUT_MODE_END));
    } // void _ModeSwitching()

    // Use this for initialization
    void Start()
    {
        m_camera_SceneMain = Camera.main;

        //@ Init
        m_drawRenderMesh.InitDrawRenderMesh();

        _v2Rot_CAM.x = m_camera_SceneMain.transform.eulerAngles.y;
        _v2Rot_CAM.y = m_camera_SceneMain.transform.eulerAngles.x;
    } 

    // Update is called once per frame
    void Update()
    {
        ////WORKING DUMMY ���� ����
        //for (int iForced = 0; iForced < 400000; ++iForced)
        //{
        //    Vector3 v3Random = new Vector3(UnityEngine.Random.Range(1.9f, 199.9f), UnityEngine.Random.Range(1.9f, 199.9f), UnityEngine.Random.Range(1.9f, 199.9f)).normalized;
        //}

        processCycle.GetInstance.m_intervalUnitWalking.UpdateIntervalProcess(Time.deltaTime,
                                    this.m_arrIDUnitModel[m_combo_modeltype_unitwalking.GetIdxSelected()]);
    }

    //@ all user interface events.
    void LateUpdate()
    {
        //@ Control Camera
        Update_inputControl_CameraMain();

        //@ Control Objects
        Update_inputControl_Unitwalking();

        //@ Control gameTest
        Update_inputControl_TestDebug();

        //@ Control NaviMesh
        Update_inputControl_NaviMesh();

        switch (_eInput_Mode)
        {
            case E_INPUT_MODE.E_INPUT_MODE_NAVIGATION:
                {
                    //@ Control Navigation
                    Update_inputControl_Navigation();

                    //@ Control Base
                    Update_inputControl_BatchBaseTower();
                    Update_inputControl_BatchBaseCore();
                    Update_inputControl_BatchBaseStart();
                }
                break;
            case E_INPUT_MODE.E_INPUT_MODE_CURVELINE:
                {
                    //@ Control Batch Flypath unit
                    Update_inputControl_CurvePathUnit();
                }
                break;
        } // switch(_eInput_Mode)
    } // void LateUpdate()

    void OnGUI()
    {
        if (false == _bOnGUI)
        {
            return;
        }

        //@for debugging
        if(true == _bOutputDetectLeaks)
        {
            GUILayout.BeginArea(new Rect(
                                    Screen.width - m_propertyInputGUI._rect_GroupPanelBatch.xMin,
                                    Screen.height - 200,
                                    m_propertyInputGUI._rect_GroupPanelBatch.width, m_propertyInputGUI._rect_GroupPanelBatch.height));

            GUILayout.Label(CFORDEBUG.detectleak(), GUILayout.MinHeight(100.0f), GUILayout.MinWidth(100.0f), GUILayout.Height(400.0f), GUILayout.Width(400.0f));
            GUILayout.EndArea();
        }

        //@ Title Window
        OnGUI_inputControl_SELMAP();

        //@ Input Control GUI
        OnGUI_inputControl_EDITOR();

        //@ Input Control GUI:NAVI
        if (true == _bRepresentPanelNavi)
        {
            OnGUI_inputControl_EDITOR_TEST();
        }
    } // void OnGUI()

} // public class CProcessInput : MonoBehaviour
