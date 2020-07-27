using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Consist of 1 more triangles on ground basement.
public class CBaseTowerCollector : CBaseCollector
{
    //@ Process
    override public void InitBaseCollector()
    {
        base.InitBaseCollector();
        m_colorBase__ = Color.Lerp(Color.black, Color.magenta, 0.3f);
        m_colorInstantSelect = Color.Lerp(Color.red, Color.white, 0.5f);
    }
} // public class CBaseTowerCollector : CBaseCollector
