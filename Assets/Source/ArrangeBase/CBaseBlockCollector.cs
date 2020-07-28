using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ To find shortest path, Remapping all navigation triangles synchronized with TriCollector 
//  when request all changed mapping event.
public class CBaseBlockCollector : CBaseCoreCollector
{
    int m_itypeblockNum = -1;
    int typeblockNum
    {
        get { return m_itypeblockNum; }
        set { m_itypeblockNum = value; }
    }

    //@ Process
    public void InitBaseBlockCollector(int itypeblockCnt)
    {
        typeblockNum = itypeblockCnt;
        InitBaseCollector();
    }

    override public void InitBaseCollector()
    {
        base.InitBaseCollector();
        m_colorBasemesh = Color.Lerp(Color.black, Color.blue, 0.6f);
        m_colorBase_Selected = Color.Lerp(Color.Lerp(Color.yellow, Color.blue, 0.7f), Color.white, 0.8f);
        m_v3PosAdjustText3D = new Vector3(0.2f, 0.6f, 0.0f);
    }

    public bool newSquareBaseBlockFromTri(int iIdxTri, bool bOnlyPair, CTriCollector tricollector)
    {
        bool bSetNew = base.newSquareBaseFromTri(iIdxTri, bOnlyPair, tricollector);
        if(true == bSetNew)
        {
            if (m_listBase__.Count < 1)
            {
                return false;
            }

            int irandomizeBlocktype = UnityEngine.Random.Range(0, m_itypeblockNum);
            CBASE__ baseBlockLastof =  m_listBase__[m_listBase__.Count-1];
            baseBlockLastof.setIdxType(irandomizeBlocktype);

            return true;
        }
        return false;
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
                                        "Block" + "(" + baseCurr.getIdxType() + ")",
                                        true);

            }
        }

    } // override public void _SetDrawDebug_Base__All()

} // public class CBaseBlockCollector : CBaseCoreCollector
