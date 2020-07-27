using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CDrawText3D : MonoBehaviour
{
    //@ 3D Text Draw
    protected List<C3DText_> m_list3DTextPath = new List<C3DText_>();
    public C3DText_ m_Text3D_src = null;

    public void setRotate(float fAngleX, float fAngleY, float fAngleZ)
    {
        
    }

    public void setNew3DText(Vector3 v3PosText3D, Quaternion quatText3D, int iSizeFont3D, Color colorText3D, string strText3D, bool bBillboard)
    {
        if (null == m_Text3D_src)
        {
            return;
        }

        C3DText_ textNumbering = (C3DText_)Instantiate(m_Text3D_src, v3PosText3D, quatText3D);
        textNumbering.SetFont(strText3D, iSizeFont3D, colorText3D, bBillboard);
        m_list3DTextPath.Add(textNumbering);
    }

    public void clear3DTextAll()
    {
        foreach (C3DText_ textCurr in m_list3DTextPath)
        {
            if (textCurr )
                Destroy(textCurr.gameObject);
        }
        m_list3DTextPath.Clear();
    }

    void Awake()
    {
        if (null == m_Text3D_src)
        {
            


            return;
        }
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
