using UnityEngine;
using System.Collections;

public class C3DText_ : MonoBehaviour
{
    protected TextMesh m_textMesh;
    protected Renderer m_Renderer;

    protected string m_strText;
    protected bool m_bBillboard;

    //@ Set/Get
    public void SetFont(string strText, int iFontSize, Color colorFont, bool bBillboard)
    {
        m_textMesh = gameObject.GetComponent<TextMesh>();
        m_Renderer = gameObject.GetComponent<Renderer>();

        m_strText = new string(strText.ToCharArray());

        m_textMesh.text = m_strText;
        SetFontSize(iFontSize);
        SetFontColor(colorFont);

        m_bBillboard = bBillboard;
    }

    public void SetVisible(bool bVisible)
    {
        m_Renderer.enabled = bVisible;
    }

    public bool GetVisible()
    {
        return m_Renderer.enabled;
    }

    public void SetPosition(Vector3 v3Position)
    {
        transform.position = v3Position;
    }

    public void SetText(string strText)
    {
        m_strText = strText;
        m_textMesh.text = strText;
    }

    public string GetText()
    {
        return m_strText;
    }

    public void SetFontColor(Color colorFont)
    {
        m_Renderer.material.color = colorFont;
    }

    public Color GetFontColor()
    {
        return m_Renderer.material.color;
    }

    public void SetFontSize(int iFontSize)
    {
        //m_textMesh.fontSize = iFontSize;
    }

    public int GetFontSize()
    {
        return m_textMesh.fontSize;
    }

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
	}
	
	// Update is called once per frame
	void Update () {
        if (true == m_bBillboard)
        {
            Vector3 v3OppositeCamera = new Vector3(transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, transform.localEulerAngles.z);

            transform.localEulerAngles = v3OppositeCamera;
        }
    } // void Update ()
}
