using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class  BaseInfo
{
    public BaseInfo()
    {
        m_CellIndex = new List<int>();
        m_CoreTriPnt = new List<Vector3>();
        m_CoreTriPntSrc = new List<Vector3>();
    }

    ~BaseInfo()
    {
        Realse();
    }
   
    public void AddCellIndex(int index)
    {
        m_CellIndex.Add(index);
    }
    public void AddCoreTriPnt(Vector3 pos)
    {
        m_CoreTriPnt.Add(pos);
    }
    public void AddCoreTriPntSrc(Vector3 pos)
    {
        m_CoreTriPntSrc.Add(pos);
    }

    public void CoreSave(BinaryWriter bw)
    {
        bw.Write(m_Type);

        bw.Write(m_CellIndex.Count);
        foreach( int index in m_CellIndex)
        {
            bw.Write(index);
        }
        bw.Write(m_CoreTriPnt.Count);
        foreach (Vector3 vec in m_CoreTriPnt)
        {
            bw.Write(vec.x);
            bw.Write(vec.y);
            bw.Write(vec.z);
        }
        bw.Write(m_CoreTriPntSrc.Count);
        foreach (Vector3 vec in m_CoreTriPntSrc)
        {
            bw.Write(vec.x);
            bw.Write(vec.y);
            bw.Write(vec.z);
        }
        bw.Write(CenterPos.x);
        bw.Write(CenterPos.y);
        bw.Write(CenterPos.z);

    }
    public void CoreLoad(BinaryReader br)
    {
        Realse();

        int count = 0;
        Vector3 vec;

        m_Type = br.ReadInt32();
        count = br.ReadInt32();
        for ( int i=0; i<count ; i++)
        {
            m_CellIndex.Add(br.ReadInt32());
        }
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            m_CoreTriPnt.Add(vec);
        }
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            m_CoreTriPntSrc.Add(vec);
        }
        vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        m_CenterPos = vec;
    }


    protected int m_Type;                          //  core identifier
    public int Type
    {
        get { return m_Type; }
        set { m_Type = value; }
    }
    protected List<int> m_CellIndex;               //  core member
    public List<int> CellIndex
    {
        get { return m_CellIndex; }
        set { m_CellIndex = value; }
    }
    protected Vector3 m_CenterPos;                //  core center pos
    public Vector3 CenterPos
    {
        get { return m_CenterPos; }
        set { m_CenterPos = new Vector3(value.x, value.y, value.z); }
    }
    protected List<Vector3> m_CoreTriPnt;          //  core tri pos
    public List<Vector3> CoreTriPnt
    {
        get { return m_CoreTriPnt; }
        set { m_CoreTriPnt = value; }
    }
    protected List<Vector3> m_CoreTriPntSrc;       //  core member tri pos
    public List<Vector3> CoreTriPntSrc
    {
        get { return m_CoreTriPntSrc; }
        set { m_CoreTriPntSrc = value; }
    }
    protected void Realse()
    {
        if (m_CellIndex != null)
            m_CellIndex.Clear();
        if (m_CoreTriPnt != null)
            m_CoreTriPnt.Clear();
        if (m_CoreTriPntSrc != null)
            m_CoreTriPntSrc.Clear();
    }
            
}

public class CMapTemplate
{
    //  파일경로 관련
    public static readonly string m_Ext = ".map";         //  파일 확장자
    public static readonly string m_Forder = "stage/";     //  파일이 있는곳
    public string m_Filename = null;


    //  map에 저장할것들 추가 
    public void AddMainCore(BaseInfo MainCore)      //  Add maincore base
    {
        m_MainCore_List.Add(MainCore);
    }

    public void AddSubCore(BaseInfo SubCore)        //  Add subcore base
    {
        m_SubCore_List.Add(SubCore);
    }

    public void AddStartBase(BaseInfo startbase)    //  Add start base
    {
        m_StartBase_List.Add(startbase);
    }

    public void AddBlockBase(BaseInfo blockbase)    //  Add block base
    {
        m_BlockBase_List.Add(blockbase);
    }

    public void AddGoalPoint(int goalindex)         // Add goal point
    {
        m_GoalPoint_List.Add(goalindex);
    }
    public void AddGroundStartPoint(int startindex)     //  Add wave start point
    {
        m_GroundStartPoint_List.Add(startindex);
    }
    public void AddGroundBlockPoint(int blockindex)     //  Add block point
    {
        m_GroundBlockPoint_List.Add(blockindex);
    }
    public void AddGroundBlockRoadPoint(int blockroadindex)     //  Add blockroad point
    {
        m_GroundBlockRoadPoint_List.Add(blockroadindex);
    }
    public void AddFlyPath(CSplineGenerator path)  //  Add fly path
    {
        m_FlyUnitPath_List.Add(path);
    }
    public void AddBaseTower(CBASE__ towerbase)       //  Add base
    {
        m_BaseTower.Add(towerbase);
    }
    public void AddTriangle(CTRI tri)                   //  Add tri
    {
        m_MapTriangle.Add(tri);
    }

    public Vector3 GetSatartTriagleCenterPosition(int startIndex)
    {       
        if (m_GroundStartPoint_List.Count <= startIndex)
            return Vector3.zero;

        int index = m_GroundStartPoint_List[startIndex];
        CTRI tri = m_MapTriangle[index];
        Vector3 startPosition = (tri._arrv3PT[0] + tri._arrv3PT[1] + tri._arrv3PT[2]) / 3.0f; ;
        return startPosition;
    }

    public Vector3 GetStartTriangleRandomPosition(int startIndex)
    {
        if (m_GroundStartPoint_List.Count <= startIndex)
            return Vector3.zero;

        int index = m_GroundStartPoint_List[startIndex];
        CTRI tri = m_MapTriangle[index];

        float r1 = Random.Range(0.1f, 0.8f);
        float r2 = Random.Range(0.1f, (1.0f - r1 - 0.1f));
        float r3 = 1.0f - r1 - r2;

        float positionX = tri._arrv3PT[0].x * r1 + tri._arrv3PT[1].x * r2 + tri._arrv3PT[2].x * r3; 
        float positionZ = tri._arrv3PT[0].z * r1 + tri._arrv3PT[1].z * r2 + tri._arrv3PT[2].z * r3;

        return new Vector3(positionX, tri._arrv3PT[0].y, positionZ);
    }

    public bool SaveStage()
    {
        if (null == m_Filename)
            return false;

        string filepath = m_Forder + m_Filename + m_Ext;

        MemoryStream ms = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(ms);

        //  MapTri
        bw.Write(m_MapTriangle.Count);
        foreach (CTRI tri in m_MapTriangle)
        {
            for (int i = 0; i < 3; i++)
            {
                bw.Write(tri._arrv3PT[i].x);
                bw.Write(tri._arrv3PT[i].y);
                bw.Write(tri._arrv3PT[i].z);
                bw.Write(tri._arriIV[i]);
            }
            bw.Write(tri._v3TriNormal.x);
            bw.Write(tri._v3TriNormal.y);
            bw.Write(tri._v3TriNormal.z);
            bw.Write(tri._arrEdgeLength[0]);
            bw.Write(tri._arrEdgeLength[1]);
            bw.Write(tri._arrEdgeLength[2]);
        }
        //  MapArrageBase
        bw.Write(m_BaseTower.Count);
        foreach (CBASE__ arrage in m_BaseTower)
        {
            //  삼각형 정보
            bw.Write(arrage._listIdxTris.Count);        //  갯수
            foreach (int idx in arrage._listIdxTris)    //  삼각형 인덱스들
                bw.Write(idx);

            bw.Write(arrage._listv3Pnts.Count);         //  건물영역 사각형 꼭지점갯수
            foreach (Vector3 pnt in arrage._listv3Pnts)
            {
                bw.Write(pnt.x);
                bw.Write(pnt.y);
                bw.Write(pnt.z);
            }
            //  중앙포지션
            bw.Write(arrage._v3PositionCenter.x);
            bw.Write(arrage._v3PositionCenter.y);
            bw.Write(arrage._v3PositionCenter.z);
            //  삼각형 정보
            bw.Write(arrage._listv3PntsSrc.Count);
            foreach (Vector3 pnt in arrage._listv3PntsSrc)
            {
                bw.Write(pnt.x);
                bw.Write(pnt.y);
                bw.Write(pnt.z);
            }
        }
        //  MainCoreList
        bw.Write(m_MainCore_List.Count);
        foreach (BaseInfo Core in m_MainCore_List)
        {
            Core.CoreSave(bw);
        }
        //  SubCoreList
        bw.Write(m_SubCore_List.Count);
        foreach (BaseInfo Core in m_SubCore_List)
        {
            Core.CoreSave(bw);
        }
        //  StartbaseList
        bw.Write(m_StartBase_List.Count);
        foreach (BaseInfo Core in m_StartBase_List)
        {
            Core.CoreSave(bw);
        }
        //  BlockbaseList
        bw.Write(m_BlockBase_List.Count);
        foreach (BaseInfo Core in m_BlockBase_List)
        {
            Core.CoreSave(bw);
        }
        //  GoalPoint
        bw.Write(m_GoalPoint_List.Count);
        foreach (int index in m_GoalPoint_List)
        {
            bw.Write(index);
        }
        //  GroundMonster_StartPoint
        bw.Write(m_GroundStartPoint_List.Count);
        foreach (int index in m_GroundStartPoint_List)
        {
            bw.Write(index);
        }
        //  BlockPoint
        bw.Write(m_GroundBlockPoint_List.Count);
        foreach (int index in m_GroundBlockPoint_List)
        {
            bw.Write(index);
        }
        //  BlockRoadPoint
        bw.Write(m_GroundBlockRoadPoint_List.Count);
        foreach (int index in m_GroundBlockRoadPoint_List)
        {
            bw.Write(index);
        }
        //  flyPath list
        bw.Write(m_FlyUnitPath_List.Count);
        foreach (CSplineGenerator fly in m_FlyUnitPath_List)
        {
            bw.Write((int)fly.GetTypeSplineCurve());
            bw.Write(fly.GetDivisionWeight());
            bw.Write(fly.m_listPath_Spline.Count);
            foreach (Vector3 pos in fly.m_listPath_Spline)
            {
                bw.Write(pos.x);
                bw.Write(pos.y);
                bw.Write(pos.z);
            }
        }
        //  save
        CFileManager tFile = CFileManager.GetInstance;
        ms.Seek(0, SeekOrigin.Begin);
        tFile.SaveFile(filepath, ms);

        ms.Close();
        
        return true;
    }

    public bool LoadStage()
    {
        if (null == m_Filename)
            return false;
        Release();

        string filepath = m_Forder + m_Filename + m_Ext;

        CFileManager tFile = CFileManager.GetInstance;

        //  load stream
        Stream ms = tFile.LoadFile(filepath);
        if (null == ms)
        {
            return false;
        }

        BinaryReader br = new BinaryReader(ms);

        //  MapTri  
        int count = 0;
        int count2 = 0;
        count = br.ReadInt32();
        CTRI tri = null;
        for (int i = 0; i < count; i++)
        {
            tri = new CTRI();
            for (int j = 0; j < 3; j++)
            {
                tri._arrv3PT[j].x = br.ReadSingle();
                tri._arrv3PT[j].y = br.ReadSingle();
                tri._arrv3PT[j].z = br.ReadSingle();
                tri._arriIV[j] = br.ReadInt32();
            }

            tri._v3TriNormal.x = br.ReadSingle();
            tri._v3TriNormal.y = br.ReadSingle();
            tri._v3TriNormal.z = br.ReadSingle();
            tri._arrEdgeLength[0] = br.ReadSingle();
            tri._arrEdgeLength[1] = br.ReadSingle();
            tri._arrEdgeLength[2] = br.ReadSingle();

            AddTriangle(tri);

        }
        //  MapArrageBase
        count = br.ReadInt32();
        CBASE__ towerArrange = null;
        for (int i = 0; i < count; i++)
        {
            towerArrange = new CBASE__();
            count2 = br.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                towerArrange._listIdxTris.Add(br.ReadInt32());
            }

            count2 = br.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                Vector3 vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                towerArrange._listv3Pnts.Add(vec);
            }
            //  중앙포지션
            towerArrange._v3PositionCenter.x = br.ReadSingle();
            towerArrange._v3PositionCenter.y = br.ReadSingle();
            towerArrange._v3PositionCenter.z = br.ReadSingle();

            //  삼각형 정보
            count2 = br.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                Vector3 vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                towerArrange._listv3PntsSrc.Add(vec);
            }

            AddBaseTower(towerArrange);
            m_BaseTowerMeshIndex.Add(towerArrange._listIdxTris[0], i);
            m_BaseTowerMeshIndex.Add(towerArrange._listIdxTris[1], i);
        }
        //  MainCoreList
        count = br.ReadInt32();
        BaseInfo core;
        for (int i = 0; i < count; i++)
        {
            core = new BaseInfo();
            core.CoreLoad(br);
            AddMainCore(core);
        }
        //  SubCoreList
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            core = new BaseInfo();
            core.CoreLoad(br);
            AddSubCore(core);
        }
        //  StartBaseList
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            core = new BaseInfo();
            core.CoreLoad(br);
            AddStartBase(core);
        }
        //  BlockBaseList
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            core = new BaseInfo();
            core.CoreLoad(br);
            AddBlockBase(core);
        }
        //  GoalPoint
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            AddGoalPoint(br.ReadInt32());
        }
        //  GroundMonster_StartPoint
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            AddGroundStartPoint(br.ReadInt32());
        }
        //  BlockPoint
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            AddGroundBlockPoint(br.ReadInt32());
        }

        //  BlockRoadPoint
        count = br.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            AddGroundBlockRoadPoint(br.ReadInt32());
        }

        //  flyPath list
        count = br.ReadInt32();
        CSplineGenerator path = null;
        for (int i = 0; i < count; i++)
        {
            path = new CSplineGenerator();

            path.SetTypeSplineCurve((E_TYPE_SPLINE)br.ReadUInt32());
            path.SetDivisionWeight(br.ReadSingle());

            count2 = br.ReadInt32();
            for (int j = 0; j < count2; j++)
            {
                Vector3 vec = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                path.SetNewCurvePoint_spline(vec);
            }

            AddFlyPath(path);
        }

        //  파일포인터 맨처음으로 
        ms.Seek(0, SeekOrigin.Begin);

        return true;
    }
    
    public void Release()
    {
        if (null != m_MapTriangle)
        {
            m_MapTriangle.Clear();
        }
        if (null != m_BaseTower)
        {
            m_BaseTower.Clear();
        }
        if (null != m_MainCore_List)
        {
            m_MainCore_List.Clear();
        }
        if (null != m_SubCore_List)
        {
            m_SubCore_List.Clear();
        }
        if (null != m_StartBase_List)
        {
            m_StartBase_List.Clear();
        }
        if (null != m_BlockBase_List)
        {
            m_BlockBase_List.Clear();
        }
        if (null != m_GoalPoint_List)
        {
            m_GoalPoint_List.Clear();
        }
        if (null != m_GroundStartPoint_List)
        {
            m_GroundStartPoint_List.Clear();
        }
        if (null != m_GroundBlockPoint_List)
        {
            m_GroundBlockPoint_List.Clear();
        }
        if (null != m_GroundBlockRoadPoint_List)
        {
            m_GroundBlockRoadPoint_List.Clear();
        }
        if (null != m_FlyUnitPath_List)
        {
            m_FlyUnitPath_List.Clear();
        }
        if( null != m_BaseTowerMeshIndex)
        {
            m_BaseTowerMeshIndex.Clear();
        }
    }

    public CMapTemplate()
    {
        m_MapTriangle = new List<CTRI>();
        m_BaseTower = new List<CBASE__>();
        m_MainCore_List = new List<BaseInfo>();
        m_SubCore_List = new List<BaseInfo>();
        m_StartBase_List = new List<BaseInfo>();
        m_BlockBase_List = new List<BaseInfo>();
        m_GoalPoint_List = new List<int>();
        m_GroundStartPoint_List = new List<int>();
        m_GroundBlockPoint_List = new List<int>();
        m_GroundBlockRoadPoint_List = new List<int>();
        m_FlyUnitPath_List = new List<CSplineGenerator>();
        m_BaseTowerMeshIndex = new SortedDictionary<int, int>();
    }

    ~CMapTemplate()
    {
        Release();
    }

    // private
    //  맵 베이스 정보
    private List<CTRI> m_MapTriangle;            //  맵정보(메쉬)
    public List<CTRI> MapTriangle
    {
        get { return m_MapTriangle; }
    }
    private List<CBASE__> m_BaseTower;          //  타워건설가능지역
    public List<CBASE__> BaseTower
    {
        get { return m_BaseTower; }
    }
    private SortedDictionary<int, int> m_BaseTowerMeshIndex;
    public SortedDictionary<int, int> BaseTowerMeshIndex
    {
        get { return m_BaseTowerMeshIndex; }
    }
    //  코어들 위치 정보
    private List<BaseInfo> m_MainCore_List = null;    //  주코어
    public List<BaseInfo> MainCoreList
    {
        get { return m_MainCore_List; }
    }
    private List<BaseInfo> m_SubCore_List = null;     //  부코어
    public List<BaseInfo> SubCoreList
    {
        get { return m_SubCore_List; }
    }

    // 시작셀 그룹
    private List<BaseInfo> m_StartBase_List = null;    // 시작셀베이스
    public List<BaseInfo> StartbaseList
    {
        get { return m_StartBase_List; }
    }

    private List<BaseInfo> m_BlockBase_List = null;     //  블럭베이스
    public List<BaseInfo> BlockBaseList
    {
        get { return m_BlockBase_List; }
    }

    //  navi info( goal, start, block  )
    private List<int> m_GoalPoint_List = null;          //  Goal Index list
    public List<int> GoalPoint_List
    {
        get { return m_GoalPoint_List; }
    }
    private List<int> m_GroundStartPoint_List = null;      //  지상유닛 시작위치
    public List<int> GroundStartPoint_List
    {
        get { return m_GroundStartPoint_List; }
    }
    private List<int> m_GroundBlockPoint_List = null;       //  블럭위치
    public List<int> GroundBlockPoint_List
    {
        get { return m_GroundBlockPoint_List; }
    }
    private List<int> m_GroundBlockRoadPoint_List = null;       //  블럭로드위치
    public List<int> GroundBlockRoadPoint_List
    {
        get { return m_GroundBlockRoadPoint_List; }
    }
    private List<CSplineGenerator> m_FlyUnitPath_List = null;  //  공중유닛 비행경로 리스트
    public List<CSplineGenerator> FlyUnitPath_List
    {
        get { return m_FlyUnitPath_List; }
    }

}
