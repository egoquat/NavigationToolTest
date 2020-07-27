using UnityEngine;
using System.Collections;

//@ Instantiate to this object class which only worked Prefab for Rendering Line.
public class CRenderLineDebug : MonoBehaviour
{
    public Color c1 = Color.Lerp(Color.red, Color.blue, 0.1f);
    public Color c2 = Color.Lerp(Color.red, Color.blue, 0.1f);
    public float fThicknessOfLine = 2.0f;

    //@ 
    public void DrawLine_user(Vector3[] arrv3PT)
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(fThicknessOfLine, fThicknessOfLine);

        lineRenderer.SetVertexCount(arrv3PT.Length);
        for (int iPnt = 0; iPnt < arrv3PT.Length; ++iPnt)
        {
            Vector3 v3Curr = arrv3PT[iPnt];
            lineRenderer.SetPosition(iPnt, v3Curr);
        }
    } // public void DrawLine_user(Vector3[] arrv3PT)
} // public class CRenderLineDebug : MonoBehaviour
