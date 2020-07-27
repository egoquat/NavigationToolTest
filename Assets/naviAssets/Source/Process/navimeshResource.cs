using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class navimeshResource : MonoBehaviour
{
    #region constparameter
    public static int null_navimesh_instance = -1;
    #endregion // #region constparameter

    struct navimesh_name
    {
        public CNaviMesh _naviMesh;
        public string _strNameNaviMesh;
    };

    //@ Prefab NaviMesh All
    public string[] arrayStageName;
    public CNaviMesh[] arrayStageMesh;
    public int StartNaviMesh;

    //@ DataStructure
    List<navimesh_name> m_listNameNavimesh = new List<navimesh_name>();

    //@ Accessor for Filetype
    CNaviMesh m_naviMesh_Instance = null;
    int m_iIdxNaviMesh_Instance = navimeshResource.null_navimesh_instance;

    //@Process private
    void Destroy_activeNaviMesh()
    {
        if (null != m_naviMesh_Instance && null != m_naviMesh_Instance.gameObject)
        {
            Destroy(m_naviMesh_Instance.gameObject);
        }

        m_naviMesh_Instance = null;

        m_iIdxNaviMesh_Instance = navimeshResource.null_navimesh_instance;
    }

    void setNew_NaviMesh(string strKey, CNaviMesh navimeshValue)
    {
        navimesh_name navimeshName = new navimesh_name();
        navimeshName._naviMesh = navimeshValue;
        navimeshName._strNameNaviMesh = strKey;

        m_listNameNavimesh.Add(navimeshName);
    }

    //@Get/Set
    public int getIdxActive_Navimesh()
    {
        return m_iIdxNaviMesh_Instance;
    }

    public int getCountNaviMesh()
    {
        return m_listNameNavimesh.Count;
    }

    bool outOfRangeNAVIMESH(int iSeqKey)
    {
        if (m_listNameNavimesh.Count <= iSeqKey || iSeqKey < 0)
        {
            Debug.Log("ERROR. CNaviMesh OutofRange!!//m_listNameNavimesh.Count=" + m_listNameNavimesh.Count + "//iSeqKey=" + iSeqKey + "//");   
            return true;
        }

        return false;
    }

    public CNaviMesh getNavigationMesh(int iSeqKey)
    {
        if (true == outOfRangeNAVIMESH(iSeqKey))
        {
            return null;
        }

        Destroy_activeNaviMesh();

        m_iIdxNaviMesh_Instance = iSeqKey;
        m_naviMesh_Instance = (CNaviMesh)Instantiate(m_listNameNavimesh[iSeqKey]._naviMesh);

        return m_naviMesh_Instance;
    }

    public CNaviMesh getNavigationMesh(string strKeyNaviMesh)
    {
        foreach (navimesh_name naviMeshNAME in m_listNameNavimesh)
        {
            if (strKeyNaviMesh == naviMeshNAME._strNameNaviMesh)
            {
                return naviMeshNAME._naviMesh;
            }
        }

        Debug.Log("CNaviMesh getNaviMesh(string strKeyNaviMesh)//strKeyNaviMesh=" + strKeyNaviMesh + "//");

        return null;
    }

    public string getNaviMeshName(int iSeqKey)
    {
        if (true == outOfRangeNAVIMESH(iSeqKey))
        {
            return null;
        }

        return m_listNameNavimesh[iSeqKey]._strNameNaviMesh;
    }

    public string[] getArrayNaviMeshNeme()
    {
        List<string> listNaviMeshName = new List<string>();
        getListNaviMeshName(ref listNaviMeshName);

        return listNaviMeshName.ToArray();
    } 

    public bool getListNaviMeshName(ref List<string> listNaviMeshNames)
    {
        listNaviMeshNames.Clear();

        foreach (navimesh_name naviMeshNAME in m_listNameNavimesh)
        {
            listNaviMeshNames.Add(naviMeshNAME._strNameNaviMesh);
        }

        return true;
    } 

    void Awake()
    {
        for(int iSeqNavimesh = 0; iSeqNavimesh < arrayStageMesh.Length; ++iSeqNavimesh)
        {
            CNaviMesh naviMesh = arrayStageMesh[iSeqNavimesh];
            string strNavimesh = arrayStageName[iSeqNavimesh];

            setNew_NaviMesh(strNavimesh, naviMesh);
        }
    }

    void Start()
    {

    }

    void OnDestroy()
    {
        processCycle.GetInstance.Release();
    }
} // public class navimeshResource : MonoBehaviour
