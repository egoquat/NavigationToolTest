using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ To find shortest path, Remapping all navigation triangles synchronized with TriCollector 
//  when request all changed mapping event.
public class CBaseStartCollector : CBaseCoreCollector
{
    //@ Process
    override public void InitBaseCollector()
    {
        base.InitBaseCollector();
        m_colorBasemesh = Color.Lerp(Color.black, Color.green, 0.6f);
        m_colorBase_Selected = Color.Lerp(Color.Lerp(Color.yellow, Color.green, 0.7f), Color.white, 0.8f);
        m_v3PosAdjustText3D = new Vector3(0.2f, 0.6f, 0.0f);
    }

    //@ Debug Draw
    override public void _SetDrawDebug_Base__All()
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        _SetUndrawDebugBase_All();
        base._SetDraw_Bases();

        if (getCountBase() > 0)
        {
            List<CBASE__> listBase = m_listBase__;
            Quaternion quaternionCoreText3D = new Quaternion();
            Vector3 v3Position;

            for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)
            {
                CBASE__ baseCurr = listBase[iSeqBase];

                v3Position = baseCurr.getCenterPosition();
                v3Position = v3Position + m_v3PosAdjustText3D;

                m_drawText3D.setNew3DText(v3Position,
                                        quaternionCoreText3D,
                                        m_iFontSizeText3D,
                                        m_colorText3D,
                                        "Start" + iSeqBase + "(" + baseCurr.getIdxType() + ")",
                                        true);

            }
        }

    } // override public void _SetDrawDebug_Base__All()

} // public class CBaseStartCollector : CBaseCoreCollector
