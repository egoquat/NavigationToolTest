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

        lineRenderer.startColor = c1;
        lineRenderer.endColor = c2;
        lineRenderer.startWidth = fThicknessOfLine;
        lineRenderer.endWidth = fThicknessOfLine;

        lineRenderer.positionCount = arrv3PT.Length;
        for (int iPnt = 0; iPnt < arrv3PT.Length; ++iPnt)
        {
            Vector3 v3Curr = arrv3PT[iPnt];
            lineRenderer.SetPosition(iPnt, v3Curr);
        }
    } // public void DrawLine_user(Vector3[] arrv3PT)
} // public class CRenderLineDebug : MonoBehaviour
