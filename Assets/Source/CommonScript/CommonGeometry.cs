using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class CommonGeometry
{
    public static Vector3 GetRandomVector3(float rangeMin, float rangeMax)
    {
        return new Vector3(Random.Range(rangeMin, rangeMax), Random.Range(rangeMin, rangeMax), Random.Range(rangeMin, rangeMax));
    }

    public static Quaternion GetRandomRotation(float angleMin, float angleMax)
    {
        return Quaternion.Euler(Random.Range(angleMin, angleMax), Random.Range(angleMin, angleMax), Random.Range(angleMin, angleMax));
    }

    public static bool IsInPlanes(Plane[] planes, Vector3 position)
    {
        foreach (Plane p in planes) { if (p.GetDistanceToPoint(position) < 0) { return false; } }
        return true;
    }

    public static bool IsInPlanes(Plane[] planes, Bounds bounds)
    {
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    //@ Root가 보유한 모든 정점 중 Min, Max 정점을 Get.
    public static void GetPointsMinMaxRecursively(this Transform root, ref Vector3 minimumPoint, ref Vector3 maximumPoint)
    {
        if (null == root) return;

        MeshFilter[] meshfilters = root.GetComponentsInChildren<MeshFilter>();
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

        foreach (MeshFilter f in meshfilters)
        {
            if (null == f.GetComponent<Renderer>()) continue;
            Mesh mesh = f.sharedMesh;
            Transform tm = f.transform;
            Matrix4x4 localToWorld = tm.localToWorldMatrix;
            foreach (Vector3 v in mesh.vertices)
            {
                Vector3 p = localToWorld.MultiplyPoint3x4(v);

                if (min.x > p.x) min.x = p.x;
                if (min.y > p.y) min.y = p.y;
                if (min.z > p.z) min.z = p.z;
                if (max.x < p.x) max.x = p.x;
                if (max.y < p.y) max.y = p.y;
                if (max.z < p.z) max.z = p.z;
            }
        }
        minimumPoint = min; maximumPoint = max;
    }

    public static int CountVertexRecursively(this Transform root)
    {
        if (null == root) return 0;

        int counted = 0;
        {
            MeshFilter[] meshfilters = root.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter f in meshfilters)
            {
                if (null == f.GetComponent<Renderer>()) continue;
                counted += f.sharedMesh.vertexCount;
            }
        }

        return counted;
    }

    //@ Root가 보유한 모든 Mesh들의 정점을 Get.
    public static void GetPointsRecursively(this Transform root, ref List<Vector3> listVertices, bool recursive = true)
    {
        if (null == root) return;

        if (false == recursive)
        {
            MeshFilter filter = root.GetComponent<MeshFilter>();

            if (null != filter)
            {
                if (null == filter.GetComponent<Renderer>()) return;
                Mesh mesh = filter.sharedMesh;
                Matrix4x4 localToWorld = filter.transform.localToWorldMatrix;
                foreach (int idx in mesh.triangles)
                {
                    Vector3 v = mesh.vertices[idx];
                    Vector3 p = localToWorld.MultiplyPoint3x4(v);

                    listVertices.Add(p);
                }
            }
        }
        else
        {
            MeshFilter[] meshfilters = root.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter f in meshfilters)
            {
                if (null == f.GetComponent<Renderer>()) continue;
                Mesh mesh = f.sharedMesh;
                Matrix4x4 localToWorld = f.transform.localToWorldMatrix;
                foreach (int idx in mesh.triangles)
                {
                    Vector3 v = mesh.vertices[idx];
                    Vector3 p = localToWorld.MultiplyPoint3x4(v);

                    listVertices.Add(p);
                }
            }
        }
    }

    //@ Create Simple Plane on Renderer 가로:planeWidth, 세로:planeHeight, 가로정점갯수:gridWidth, 세로정점갯수:gridHeight
    public static void CreateSimplePlaneOnRenderer(GameObject gameobject, Material material,
                        float planeHeight, float planeWidth, int gridHeight, int gridWidth)
    {
        if (null == gameobject) { Debug.LogError("(null == gameobject)"); }

        MeshFilter mf = gameobject.GetComponent<MeshFilter>();
        if (null == mf) { mf = gameobject.AddComponent<MeshFilter>(); }

        Renderer renderer = gameobject.GetComponent<Renderer>();
        if (null == renderer) { renderer = gameobject.AddComponent<MeshRenderer>(); }

        renderer.material = material;
        mf.mesh = CreatePlane(planeHeight, planeWidth, gridHeight, gridWidth);
    }

    //@심플 플랜 생성. 전체 매핑. 가로:planeWidth, 세로:planeHeight, 가로정점갯수:gridWidth, 세로정점갯수:gridHeight
    public static Mesh CreatePlane(float planeHeight, float planeWidth, int gridHeight, int gridWidth)
    {
        Mesh mesh = new Mesh();

        float tileSizeV = 1.0f / (gridHeight);
        float tileSizeU = 1.0f / (gridWidth);
        float tileHeight = planeHeight / (float)gridHeight;
        float tileWidth = planeWidth / (float)gridWidth;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
                vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
                vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
                vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));

                triangles.Add(index + 2);
                triangles.Add(index + 1);
                triangles.Add(index);
                triangles.Add(index);
                triangles.Add(index + 3);
                triangles.Add(index + 2);
                index += 4;

                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);

                uvs.Add(new Vector2(x * tileSizeU, y * tileSizeV));
                uvs.Add(new Vector2((x + 1) * tileSizeU, y * tileSizeV));
                uvs.Add(new Vector2((x + 1) * tileSizeU, (y + 1) * tileSizeV));
                uvs.Add(new Vector2(x * tileSizeU, (y + 1) * tileSizeV));
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    //@ 심플 플랜 생성, 플랜당 매핑 한번씩, 타일 가로:tileWidth, 타일세로:tileHeight, 가로정점갯수:gridWidth, 세로정점갯수:gridHeight
    public static Mesh CreatePlaneTiled(float tileHeight, float tileWidth, int gridHeight, int gridWidth)
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        var index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                vertices.Add(new Vector3((x * tileWidth), (y * tileHeight), 0));
                vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight), 0));
                vertices.Add(new Vector3((x * tileWidth) + tileWidth, (y * tileHeight) + tileHeight, 0));
                vertices.Add(new Vector3((x * tileWidth), (y * tileHeight) + tileHeight, 0));

                triangles.Add(index + 2);
                triangles.Add(index + 1);
                triangles.Add(index);
                triangles.Add(index);
                triangles.Add(index + 3);
                triangles.Add(index + 2);
                index += 4;

                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    public static Mesh copymeshFrom(Mesh meshSrc, Quaternion rotationCopymesh, Vector3 scaleCopymesh, Vector3 translateCopymesh)
    {
        if (meshSrc == null)
            return null;

        Vector3[] verticesNew = new Vector3[meshSrc.vertices.Length];
        int[] trianglesNew = new int[meshSrc.triangles.Length];
        Vector3[] normalsNew = new Vector3[meshSrc.normals.Length];
        Vector2[] uvNew = new Vector2[meshSrc.uv.Length];
        Color[] colorNew = new Color[meshSrc.colors.Length];
        Vector4[] tangentsNew = new Vector4[meshSrc.tangents.Length];

        Array.Copy(meshSrc.triangles, trianglesNew, meshSrc.triangles.Length);
        Array.Copy(meshSrc.normals, normalsNew, meshSrc.normals.Length);
        Array.Copy(meshSrc.uv, uvNew, meshSrc.uv.Length);
        Array.Copy(meshSrc.colors, colorNew, meshSrc.colors.Length);
        Array.Copy(meshSrc.tangents, tangentsNew, meshSrc.tangents.Length);

        if (rotationCopymesh == Quaternion.identity)
        {
            for (int idxVer = 0; idxVer < meshSrc.vertices.Length; ++idxVer)
            {
                verticesNew[idxVer] = (Vector3.Scale(meshSrc.vertices[idxVer], scaleCopymesh)) + translateCopymesh;
            }
        }
        else
        {
            for (int idxVer = 0; idxVer < meshSrc.vertices.Length; ++idxVer)
            {
                verticesNew[idxVer] = (Vector3.Scale((rotationCopymesh * meshSrc.vertices[idxVer]), scaleCopymesh)) + translateCopymesh;
            }
        }

        Mesh meshNew = new Mesh();
        meshNew.vertices = verticesNew;
        meshNew.triangles = trianglesNew;
        meshNew.normals = normalsNew;
        meshNew.uv = uvNew;
        meshNew.colors = colorNew;
        meshNew.tangents = tangentsNew;

        meshNew.RecalculateNormals();
        meshNew.RecalculateBounds();

        return meshNew;
    }

    public static bool collisionCheckSphereAABB(Vector2 sphereCenter, float sphereRadius, Vector2 aabbmin, Vector2 aabbmax)
    {
        float closetx = Mathf.Clamp(sphereCenter.x, aabbmin.x, aabbmax.x);
        float closety = Mathf.Clamp(sphereCenter.y, aabbmin.y, aabbmax.y);

        float distancex = sphereCenter.x - closetx;
        float distancey = sphereCenter.y - closety;
        float distanceSqr = (distancex * distancex) + (distancey * distancey);
        return distanceSqr < (sphereRadius * sphereRadius);
    }

    public static bool LineIntersection(Vector2 p0_1, Vector2 p0_2, Vector2 p1_1, Vector2 p1_2, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
        float x1lo, x1hi, y1lo, y1hi;
        Ax = p0_2.x - p0_1.x;
        Bx = p1_1.x - p1_2.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p0_2.x; x1hi = p0_1.x;
        }
        else
        {
            x1hi = p0_2.x; x1lo = p0_1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p1_2.x || p1_1.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p1_1.x || p1_2.x < x1lo) return false;
        }

        Ay = p0_2.y - p0_1.y;
        By = p1_1.y - p1_2.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p0_2.y; y1hi = p0_1.y;
        }
        else
        {
            y1hi = p0_2.y; y1lo = p0_1.y;
        }

        if (By > 0)
        {
            if (y1hi < p1_2.y || p1_1.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p1_1.y || p1_2.y < y1lo) return false;
        }

        Cx = p0_1.x - p1_1.x;
        Cy = p0_1.y - p1_1.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//

        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//
        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }
        // check if they are parallel
        if (f == 0) return false;
        // compute intersection coordinates //
        num = d * Ax; // numerator //
        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
        //    intersection.x = p0_1.x + (num+offset) / f;
        intersection.x = p0_1.x + num / f;

        num = d * Ay;
        //    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
        //    intersection.y = p0_1.y + (num+offset) / f;

        intersection.y = p0_1.y + num / f;

        return true;
    }

    public static bool CrossOverTest(Vector3 startPosition, Vector3 targetPosition, Vector3 currentPosition)
    {
        Vector3 projectileDir = (targetPosition - currentPosition);
        Vector3 targetLookDir = (targetPosition - startPosition);

        float dot = Vector3.Dot(projectileDir, targetLookDir);
        return dot < 0;
    }
}
