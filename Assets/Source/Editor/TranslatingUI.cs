using UnityEngine;
using System.Collections;

public class TranslatingUI : MonoBehaviour
{
    public Vector3 translateAxis = new Vector3(1, 0, 0);

    protected void GetPositionOnPlane(Plane planeTranslate, Ray rayPicking, out Vector3 v3PosPickingTranslate)
    {
        float fDistTranslate = 0.0f;

        planeTranslate.Raycast(rayPicking, out fDistTranslate);
        v3PosPickingTranslate = rayPicking.GetPoint(fDistTranslate);
    }

    protected void GetPositionOnPlaneTranslate(out Vector3 v3PosPickingTranslate)
    {
        Camera c = Camera.main;
        Ray rayPickingMouse = c.ScreenPointToRay(Input.mousePosition);

        Vector3 v3PosTranslate = transform.parent.parent.position;
        Vector3 v3DirTranslateToCam = (rayPickingMouse.direction) * -1.0f;//c.transform.localPosition - gameObject.transform.localPosition;
        GetPositionOnPlane(new Plane(v3DirTranslateToCam, v3PosTranslate), rayPickingMouse, out v3PosPickingTranslate);
    }

    void OnMouseDown()
    {
        lastPosition = transform.parent.parent.position;
        GetPositionOnPlaneTranslate(out lastPositionPicking);
    }

    void OnMouseDrag()
    {
        Vector3 v3PosPickingTranslate_curr;
        GetPositionOnPlaneTranslate(out v3PosPickingTranslate_curr);

        Vector3 v3DistancePickingDrag = v3PosPickingTranslate_curr - lastPositionPicking;
        Vector3 v3PosProjection_adjust, v3ObjDir;

        v3ObjDir = gameObject.transform.up;
        v3PosProjection_adjust = Vector3.Project(v3DistancePickingDrag, v3ObjDir);
        float fDistanceProjection = v3PosProjection_adjust.magnitude;
        float fMul = 1.0f;

        if (0.0f > Vector3.Dot(v3ObjDir, v3PosProjection_adjust))
        {
            fMul = -1.0f;
        }

        transform.parent.parent.position = lastPosition + (v3ObjDir * fDistanceProjection * fMul);
    } // void OnMouseDrag()

    Vector3 lastPosition;
    Vector3 lastPositionPicking = new Vector3();
} // public class TranslatingUI : MonoBehaviour
