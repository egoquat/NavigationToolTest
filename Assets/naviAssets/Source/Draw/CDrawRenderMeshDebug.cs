using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class CDrawRenderMesh
{
    #region constparameter
    public static float scaleadjust_render = 1.0f;
    public static float scaleadjust_render_095 = 0.95f;
    public static float scaleadjust_render_093 = 0.92f;
    public static float scaleadjust_render_090 = 0.90f;
    public static float scaleadjust_render_085 = 0.85f;
    public static float scaleadjust_render_070 = 0.70f;
    #endregion // #region constparameter

    private  CRenderMeshDebug m_renderMeshSource;

    private  List<CRenderMeshDebug> m_listRenderMesh = new List<CRenderMeshDebug>();

    //@ Rendered 1Drawcall Mesh for debug order by IV array. 
    //  -fEpsilonForZFight : For not to hide or override, give it to some hovering space.
    public int DrawNewRendermeshIV(int iIDX_, Vector3[] arrV3VB, int[] arriIV, Color colorUser_, float fEpsilonForZFight, float fScale)
    {
        DeleteDrawMesh(iIDX_);

        GameObject gorendermeshSource = new GameObject();
        CRenderMeshDebug renderMeshNew = gorendermeshSource.AddComponent<CRenderMeshDebug>();

        renderMeshNew.DrawMesh_user(iIDX_, arrV3VB, arriIV, colorUser_, fScale, fEpsilonForZFight);

        m_listRenderMesh.Add(renderMeshNew);

        return m_listRenderMesh.Count - 1;
    }

    //@ Rendered 1Drawcall Mesh for debug order by Sequencial VB array.
    public int DrawNewRendermesh(int iIDX_, Vector3[] arrV3VB, Color colorUser_, float fEpsilonForZFight, float fScale)
    {
        if (null == m_renderMeshSource)
        {
            return -1;
        }
        CRenderMeshDebug renderMeshNew = (CRenderMeshDebug)GameObject.Instantiate(m_renderMeshSource);
        DeleteDrawMesh(iIDX_);

        int[] arriIV_ = new int[arrV3VB.Length];
        for (int iIV = 0; iIV < arriIV_.Length; ++iIV)
        {
            arriIV_[iIV] = iIV;
        }

        renderMeshNew.DrawMesh_user(iIDX_, arrV3VB, arriIV_, colorUser_, fScale, fEpsilonForZFight);

        m_listRenderMesh.Add(renderMeshNew);

        return iIDX_;
    }

    //@ Clear and destroy meshs by range Min to Max Index as CRenderMeshDebug.m_iIDX. 
    public void DeleteDrawMesh_Range(int iIdxMin, int iCnt)
    {
        if (null == m_listRenderMesh)
        {
            return;
        }

        List<CRenderMeshDebug> listDelete = new List<CRenderMeshDebug>();

        int iIdxMax = iIdxMin + iCnt;

        for (int iSeq = 0; iSeq < m_listRenderMesh.Count; ++iSeq)
        {
            CRenderMeshDebug renderMeshDebug = m_listRenderMesh[iSeq];

            if (renderMeshDebug.m_iIDX >= iIdxMin && renderMeshDebug.m_iIDX <= iIdxMax)
            {
                listDelete.Add(renderMeshDebug);
            }
        } // for(int iSeq=0; iSeq<m_listRenderMesh.Count; ++iSeq)

        for (int iSeqDel = 0; iSeqDel < listDelete.Count; ++iSeqDel)
        {
            CRenderMeshDebug renderMeshDebug = listDelete[iSeqDel];
            m_listRenderMesh.Remove(renderMeshDebug);

            if (renderMeshDebug.gameObject)
            {
                GameObject.Destroy(renderMeshDebug.gameObject);
            }
        }
    } // public void DeleteDrawMesh_Range(int iIdxMin, int iCnt)

    //@ Clear and destroy a mesh. Index as CRenderMeshDebug.m_iIDX. 
    public bool DeleteDrawMesh(int iIDX_)
    {
        if (0 > iIDX_)
        {
            return false;
        }

        if (1 > m_listRenderMesh.Count)
        {
            return false;
        }

        CRenderMeshDebug renderMeshCurr = null;

        List<CRenderMeshDebug> listDelete = new List<CRenderMeshDebug>();

        for (int iR_ = 0; iR_ < m_listRenderMesh.Count; ++iR_)
        {
            renderMeshCurr = (CRenderMeshDebug)m_listRenderMesh[iR_];
            if (renderMeshCurr.m_iIDX == iIDX_)
            {
                listDelete.Add(renderMeshCurr);

                if (renderMeshCurr.gameObject)
                {
                    GameObject.Destroy(renderMeshCurr.gameObject);
                }
            }
        }

        for (int iD_ = 0; iD_ < listDelete.Count; ++iD_)
        {
            m_listRenderMesh.Remove(listDelete[iD_]);
        }

        return true;
    } // public bool DeleteDrawMesh(int iIDX_)

    //@ Clear and destroy meshs ALL.
    public void DeleteDrawMesh_All()
    {
        foreach (CRenderMeshDebug rendermeshDebug in m_listRenderMesh)
        {
            if (null != rendermeshDebug)
            {
                GameObject.Destroy(rendermeshDebug.gameObject);
            }
        }

        m_listRenderMesh.Clear();
    }

    public void InitDrawRenderMesh()
    {
        m_listRenderMesh = new List<CRenderMeshDebug>();

        if (true == processCycle.GetInstance._modeTool)
        {
            GameObject goRenderMeshSource = new GameObject();
            m_renderMeshSource = goRenderMeshSource.AddComponent<CRenderMeshDebug>();
            m_renderMeshSource.m_materialUser = new Material(Shader.Find("Diffuse"));
            m_renderMeshSource.m_materialUser.color = new Color(230.0f, 0.0f, 0.0f);
        }
    }

    //@ Init List Instantiate of RenderLine.
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
} // public class CDrawRenderMesh : MonoBehaviour
