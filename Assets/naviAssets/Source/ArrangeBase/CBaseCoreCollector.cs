using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//@ Consist of 1 more triangles on ground basement.
public class CBaseCoreCollector : CBaseCollector
{
    protected Color m_colorText3D = colorfont3d;
    protected int m_iFontSizeText3D = sizefont3D;
    protected Vector3 m_v3PosAdjustText3D = new Vector3(0.2f, 0.4f, 0.0f);

    //@ parameter
    protected int m_iIdxDrawBase_Selected = CMATH.INT_MAX_DIV32;
    protected int m_iIdxBase_Selected = null_idxbase;
    protected Color m_colorBase_Selected = Color.Lerp(Color.red, Color.white, 0.75f);
    protected Color m_colorBasemesh = Color.Lerp(Color.Lerp(Color.red, Color.black, 0.7f), Color.yellow, 0.2f);
    protected float m_fEpsilon_forZfight_Selected = CMATH.FEPSILON_F1 + CMATH.FEPSILON_F1;

    //@ Internal
    public bool OutRangeOfBase(int iIdxBase)
    {
        if (m_listBase__.Count <= iIdxBase || iIdxBase < 0)
        {
            return true;
        }

        return false;
    }

    //@ Get/Set
    public void takeSelectedBase_bytri(int iIdxTri)
    {
        int iIdxBase_Selected_Curr = findBasebyTri(iIdxTri);

        if (iIdxBase_Selected_Curr == null_idxbase)
        {
            return;
        }

        if (m_iIdxBase_Selected == iIdxBase_Selected_Curr)
        {
            iIdxBase_Selected_Curr = null_idxbase;
        }

        m_iIdxBase_Selected = iIdxBase_Selected_Curr;
        _SetUndrawDebug_Base_Selected();
        _SetDrawDebug_Base_Selected();

    }

    public void SetType_Base_Selected(int iIdxTypeCore)
    {
        if (false == OutRangeOfBase(m_iIdxBase_Selected))
        {
            CBASE__ baseCurr = m_listBase__[m_iIdxBase_Selected];
            baseCurr.setIdxType(iIdxTypeCore);
        }

        _SetDrawDebug_Base__All();
    }

    //@ Debug Draw
    public void _SetDrawDebug_Base_Selected()
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        if (true == OutRangeOfBase(m_iIdxBase_Selected))
        {
            return;
        }

        _SetUndrawDebug_Base_Selected();

        CBASE__ baseCurr = m_listBase__[m_iIdxBase_Selected];
        Vector3[] arrv3PntsSrc = baseCurr.getArrayPoint_src();

        m_drawRenderMesh.DrawNewRendermesh(m_iIdxDrawBase_Selected,
                                            arrv3PntsSrc,
                                            m_colorBase_Selected,
                                            m_fEpsilon_forZfight_Selected,
                                            CDrawRenderMesh.scaleadjust_render_093);
    } 

    public void _SetUndrawDebug_Base_Selected()
    {
        if (null == m_drawRenderMesh)
        {
            return;
        }

        m_drawRenderMesh.DeleteDrawMesh_Range(m_iIdxDrawBase_Selected, m_listTrisAll_copy.Count);  //@ Delete Instance Tris
    }

    override public bool delete_Base_byTri(int iIdxTri)
    {
        int iSeqBase = findBasebyTri(iIdxTri);

        if (-1 < iSeqBase)
        {
            m_listBase__.RemoveAt(iSeqBase);

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

        _SetUndraw_Bases();
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
                                        "Core" + iSeqBase + "(" + baseCurr.getIdxType() + ")", 
                                        true );

            }
        }

    } // override public void _SetDrawDebug_Base__All()

    override protected void _SetUndrawDebugBase_All()
    {
        base._SetUndraw_Bases();

        if (m_drawText3D)
        {
            m_drawText3D.clear3DTextAll();
        }
    }

    //@ Process
    override public void setTrisallToBase(List<CTRI> listTriAll)
    {
        base.setTrisallToBase(listTriAll);

        m_iID_Base_DrawForDEBUG = CMATH.INT_MAX_DIV2416;
        m_iID_Instant_DrawForDEBUG = CMATH.INT_MAX_DIV2816;

        m_colorBase__ = m_colorBasemesh;

        m_fEpsilon_forZfight = CMATH.FEPSILON_F1;
    } 
} // public class CBaseCoreCollector : CBaseCollector
