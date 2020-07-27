using UnityEngine;
using System;
using System.Collections;

public class CFORDEBUG {
    protected static long m_lTimeStart_Proliler = 0;

    public static void profilerStart()
    {
        m_lTimeStart_Proliler = DateTime.Now.Ticks;
    }

    public static float profilerEnd()
    {
        if (0 == m_lTimeStart_Proliler)
        {
            return 0.0f;
        }

        long lTimeSpot = DateTime.Now.Ticks;
        long lTimeProcess = lTimeSpot - m_lTimeStart_Proliler;

        m_lTimeStart_Proliler = 0;

        return (float)lTimeProcess / 10000000.0f;
    }


    static public string detectleak()
    {
        string strDetectText;
        strDetectText = "GameSpeed = " + GameContext.GetInstance.GameSpeed + "\n";
        strDetectText += "All Object = " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length + "\n";
        strDetectText += "Textures = " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length + "\n";
        strDetectText += "Meshes = " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length + "\n";
        strDetectText += "Materials = " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length + "\n";
        strDetectText += "GameObjects = " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length + "\n";
        strDetectText += "GameObjects(UnitAll)= " + UnitPool.GetInstance.GetUnitCount() + "\n";
        strDetectText += "Components = " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length + "\n";
        strDetectText += "Polygon(NaviMesh) = " + processCycle.GetInstance.m_triCollector.getCountTris() + "\n";
        strDetectText += "AudioClips = " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length + "\n";
        return strDetectText;
    }
} // public class CFORDEBUG {
