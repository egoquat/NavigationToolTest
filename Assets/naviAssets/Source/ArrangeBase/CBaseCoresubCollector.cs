using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Consist of 1 more triangles on ground basement.
public class CBaseCoreSubCollector : CBaseCollector
{
    //@ 3D Text Draw
    Vector3 m_v3PosAdjustText3D = new Vector3(0.2f, 0.4f, 0.0f);

    public Color m_colorBaseCoresub = Color.Lerp(Color.Lerp(Color.red, Color.black, 0.4f), Color.yellow, 0.3f);

    public Color m_colorText3D = colorfont3d;
    public int m_iFontSizeText3D = sizefont3D;

    //@ Debug Draw
    override public void _SetDrawDebug_Base__All()
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        m_drawRenderMesh.DeleteDrawMesh_All();  //@ Delete Instance Tris

        if (getCountBase() > 0)
        {
            List<CBASE__> listBase = m_listBase__;

            for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)
            {
                CBASE__ baseCurr = listBase[iSeqBase];

                Vector3[] arrv3PntsSrc = baseCurr.getArrayPoint_src();

                m_drawRenderMesh.DrawNewRendermesh(m_iID_Base_DrawForDEBUG + iSeqBase,
                                                        arrv3PntsSrc,
                                                        m_colorBase__,
                                                        m_fEpsilon_forZfight,
                                                        CDrawRenderMesh.scaleadjust_render_093);
            } // for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)
        } // if (getCountBase() > 0)

        _DrawMarking_Text3D();
    } // virtual public void _SetDrawDebug_Base__All()

    override protected void _SetUndrawDebugBase_All()
    {
        if (null != m_drawRenderMesh)
        {
            m_drawRenderMesh.DeleteDrawMesh_All();
        }

        _UndrawMarking_Text3D();
    }

    private void _DrawMarking_Text3D()
    {
        if (null == m_drawText3D) return;

        _UndrawMarking_Text3D();

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
                                        "" + iSeqBase,
                                        true );

            } // for (int iSeqBase = 0; iSeqBase < listBase.Count; ++iSeqBase)


        }
    } // private void _DrawMarking_Text3D()


    private void _UndrawMarking_Text3D()
    {
        if (m_drawText3D)
        {
            m_drawText3D.clear3DTextAll();
        }
    }

    //@ Process
    override public void setTrisallToBase(List<CTRI> listTriAll)
    {
        base.setTrisallToBase(listTriAll);

        m_iID_Base_DrawForDEBUG = CMATH.INT_MAX_DIV3216;

        m_colorBase__ = m_colorBaseCoresub;
        
        //@ 3D TEXT
        m_colorText3D = colorfont3d;
        m_iFontSizeText3D = sizefont3D;
    } // virtual public void Initialize(List<CTRI> listTris)

} // public class CBaseCoreSubCollector : CBaseCollector
