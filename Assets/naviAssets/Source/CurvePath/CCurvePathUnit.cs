using UnityEngine;
using System.Collections;

public class CCurvePathUnit : MonoBehaviour
{
    #region constparameter
    static readonly int null_seqpath = -1;
    static readonly Vector3 pos_adjust_3dtext = new Vector3(0.0f, 1.0f, 0.0f);
    static readonly Vector3 pos_adjust_3dtext_info = new Vector3(0.0f, 0.7f, 0.0f);
    static readonly int size_font3d = 20;
    static readonly int size_font3d_info = 14;
    static readonly Color color_font3d = Color.yellow;
    static readonly Color color_font3d_info = Color.white;
    static readonly bool billboard_font3d = true;
    #endregion // #region constparameter

    int m_iSeqpathunit = null_seqpath;

    bool m_bSelectedUnit = false;

    Vector3 m_v3ScaleSel = new Vector3(1.2f, 1.2f, 1.2f);
    Vector3 m_v3ScaleNonSel = new Vector3(1.0f, 1.0f, 1.0f);

    //@ Property
    Renderer m_rendererCurvePath = null;

    //@ Numbering
    public C3DText_ m_Text3D_src = null;

    C3DText_ m_Text3D_Numbering = null;
    C3DText_ m_Text3D_Position = null;

    //@ Get/Set
    public void setVisibleCurvePath(bool bVisibleCurvePath)
    {
        m_rendererCurvePath.enabled = bVisibleCurvePath;
        gameObject.SetActiveRecursively(bVisibleCurvePath);

        if (false == bVisibleCurvePath)
        {
            if (null != m_Text3D_Position)
            {
                Destroy(m_Text3D_Position.gameObject);
                m_Text3D_Position = null;
            }

            if (null != m_Text3D_Position)
            {
                Destroy(m_Text3D_Numbering.gameObject);
                m_Text3D_Numbering = null;
            }
        }
        else
        {
            if(null==m_Text3D_Numbering)
            {
                m_Text3D_Numbering = (C3DText_)Instantiate(m_Text3D_src);
            }

            if(null==m_Text3D_Position)
            {
                m_Text3D_Position = (C3DText_)Instantiate(m_Text3D_src);
            }
        } 
    } // public void setVisibleCurvePath(bool bVisibleCurvePath)

    public void setSelectFlyPathUnit(int iSectionPath)
    {
        if (null_seqpath != iSectionPath)
        {
            m_iSeqpathunit = iSectionPath;
        }

        m_bSelectedUnit = true;

        if (null != m_Text3D_Numbering)
        {
            m_Text3D_Numbering.SetVisible(true);
        }

        SetSelectMaterial();
    }

    public void setUnselectFlyPathUnit()
    {
        m_bSelectedUnit = false;

        if (null != m_Text3D_Numbering)
        {
            m_Text3D_Numbering.SetVisible(false);
        }

        SetUnselectMaterial();

        m_iSeqpathunit = null_seqpath;
    }

    public bool getSelect()
    {
        return m_bSelectedUnit;
    }

    public Vector3 getPosUnit()
    {
        return gameObject.transform.position;
    }

    public int GetSeqPathunit()
    {
        return m_iSeqpathunit;
    }

    //@ Process
    void SetSelectMaterial()
    {
        Material materialFlyPath = gameObject.GetComponent<MeshRenderer>().material;

        materialFlyPath.color = new Color(0.09f, 1.0f, 0.184f, 1.0f);
        gameObject.transform.localScale = m_v3ScaleSel;
    }

    void SetUnselectMaterial()
    {
        Material materialFlyPath = gameObject.GetComponent<MeshRenderer>().material;
        materialFlyPath.color = new Color(0.69f, 0.76f, 0.294f, 1.0f);
        gameObject.transform.localScale = m_v3ScaleNonSel;
    }

    public void InitializeFlyPathUnit()
    {
        if (null != m_Text3D_src)
        {
            m_Text3D_Numbering = (C3DText_)Instantiate(m_Text3D_src);
            m_Text3D_Numbering.SetFont("", size_font3d, color_font3d, billboard_font3d);


            m_Text3D_Position = (C3DText_)Instantiate(m_Text3D_src);
            m_Text3D_Position.SetFont("", size_font3d_info, color_font3d_info, billboard_font3d);
            m_Text3D_Position.SetVisible(true);
        }

        m_v3ScaleSel = CMATH.V3Multiply(transform.localScale, m_v3ScaleSel);
        m_v3ScaleNonSel = CMATH.V3Multiply(transform.localScale, m_v3ScaleNonSel); 

        m_rendererCurvePath = gameObject.GetComponent<Renderer>();
        SetUnselectMaterial();
    }

    public void Release_curvepathUnit()
    {
        if (null != m_Text3D_Position)
        {
            Destroy(m_Text3D_Position.gameObject);
            m_Text3D_Position = null;
        }

        if (null != m_Text3D_Numbering)
        {
            Destroy(m_Text3D_Numbering.gameObject);
            m_Text3D_Numbering = null;
        }
    } 

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
        if (m_iSeqpathunit != null_seqpath)
        {
            if (null != m_Text3D_Numbering)
            {
                m_Text3D_Numbering.SetFont(m_iSeqpathunit + "",
                                    size_font3d, color_font3d, billboard_font3d);
                m_Text3D_Numbering.SetPosition(getPosUnit() + pos_adjust_3dtext);
            }
        }

        if (null != m_Text3D_Position)
        {
            m_Text3D_Position.SetFont("( " + transform.position.x.ToString("####.##")
                                                + " , " + transform.position.y.ToString("####.##")
                                                + " , " + transform.position.z.ToString("####.##") + " )",
                                                size_font3d_info, color_font3d_info, billboard_font3d);
            m_Text3D_Position.SetPosition(getPosUnit() + pos_adjust_3dtext_info);
        }
    } // void Update ()
}
