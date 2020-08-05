using UnityEngine;
using System;
using System.Collections;


//@ Instantiate to this object class which only worked Prefab for Rendering Mesh.
public class CRenderMeshDebug : MonoBehaviour
{
    public int m_iIDX = -1;

    public Material m_materialUser;
    public Color m_colorUser;

    public void DrawMesh_user(int iIDX_,
                                Vector3[] arrVertex_in,
                                int[] arriIB,
                                Color colorUser_,
                                float fScale_,
                                float fEpsilonForZFight)
    {
        m_iIDX = iIDX_;

        int iCntVertices = arrVertex_in.Length;
        Vector3[] arrVertex_to = new Vector3[arrVertex_in.Length];

        if (fEpsilonForZFight > CMATH.FEPSILON_F4)
        {
            for (int iVB = 0; iVB < iCntVertices; ++iVB)
            {
                Vector3 v3Src_ = arrVertex_in[iVB];
                arrVertex_to[iVB] = new Vector3(v3Src_.x, v3Src_.y + fEpsilonForZFight, v3Src_.z);
            }
        }
        else
        {
            Array.Copy(arrVertex_in, arrVertex_to, arrVertex_in.Length);
        }

        if (fScale_ > 0)
        {
            CMATH.rescaleVertices(ref arrVertex_to, fScale_);
        }

        Mesh meshUser = gameObject.AddComponent<MeshFilter>().mesh;
        Renderer rendererNew = gameObject.AddComponent<MeshRenderer>().GetComponent<Renderer>();
        Vector2[] arrv2UV = new Vector2[iCntVertices];

        for (int iVB = 0; iVB < iCntVertices; ++iVB)
        {
            arrv2UV[iVB] = new Vector2(arrVertex_to[iVB].x, arrVertex_to[iVB].z);
        }

        meshUser.vertices = arrVertex_to;
        meshUser.triangles = arriIB;
        meshUser.uv = arrv2UV;
        meshUser.RecalculateNormals(); // Option

        rendererNew.material = m_materialUser; // material Mesh
        rendererNew.material.color = colorUser_; // Color Mesh
    } // public void DrawMesh_user()
} // public class CRenderMeshDebug : MonoBehaviour
