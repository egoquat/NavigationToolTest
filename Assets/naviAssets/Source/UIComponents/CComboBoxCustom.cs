using UnityEngine;
using System;
using System.Collections;

//@ ComboBox For Custom
public class CComboBoxCustom : MonoBehaviour
{
    #region constparameter
    public static Rect DFLT_RECT_COMBOBOXCUSTOM = new Rect(0, 100, 100, 20);
    public static int NULL_SELECT_ITEM = -1;
    #endregion // #region constparameter

    protected GUIContent[] m_comboBoxList = new GUIContent[1];

    protected CComboBox m_comboBoxControl = new CComboBox();
    protected GUIStyle m_listStyle = new GUIStyle();
    protected Rect m_rectComboBoxCustom = DFLT_RECT_COMBOBOXCUSTOM;
    protected String m_strSetLast;

    protected int m_IdxSelect_request = NULL_SELECT_ITEM;
    protected bool m_bVisible = true;

    //@ Get/Set
    public int getCount()
    {
        return m_comboBoxList.Length - 1;
    }

    public int getIdxLast()
    {
        return m_comboBoxList.Length - 1;
    }

    public bool visible
    {
        set { m_bVisible = value; }
        get { return m_bVisible; }
    }

    public void setPosition(float fLeft, float fTop, float fWidth, float fHeight)
    {
        m_rectComboBoxCustom.xMin = fLeft;
        m_rectComboBoxCustom.yMin = fTop;
        m_rectComboBoxCustom.width = fWidth;
        m_rectComboBoxCustom.height = fHeight;
    } // public void setPosition(float fLeft, float fTop, float fWidth, float fHeight)

    public int InsertNewItem( string strItem )
    {
        int iLengthList = m_comboBoxList.Length;
        int iIdxItem = iLengthList - 1;

        Array.Resize(ref m_comboBoxList, iLengthList + 1);
        m_comboBoxList[iIdxItem] = new GUIContent(strItem);
        SetStringLast(m_strSetLast);

        SetSelectItem(iIdxItem);
        return iIdxItem;
    }

    public int GetIdxSelected()
    {
        return m_comboBoxControl.GetSelectedItemIndex();
    }

    public void SetSelectItem(int iSelectIdx)
    {
        if (iSelectIdx < 0 || iSelectIdx >= m_comboBoxList.Length)
        {
            Debug.Log("OutOfRange ComboBox Select Idx.if (iSelectIdx < 0 || iSelectIdx >= m_comboBoxList.Length)//iSelectIdx=" + iSelectIdx + "//"
                    + UnityEngine.Random.Range(0.0f, 10000.0f));
            return;
        }

        m_IdxSelect_request = iSelectIdx;
        m_comboBoxControl.SetSelectItemIndex(m_IdxSelect_request);
    } // public void SetSelectItem(int iSelectIdx)

    public bool IsLastSelected()
    {
        bool bLastSelected = (m_comboBoxList.Length - 1 <= m_comboBoxControl.GetSelectedItemIndex());
        return bLastSelected;
    }

    public void setClickedCombo()
    {
        m_comboBoxControl.setClickedComboBox();
    }

    public bool IsClickedCombo()
    {
        return m_comboBoxControl.IsClickedComboBox();
    }

    public bool IsSelectChanged()
    {
        return m_comboBoxControl.IsSelectChangedComboBox();
    }

    public void SetClear()
    {
        m_comboBoxList = new GUIContent[1];
        SetStringLast(m_strSetLast);
        SetSelectItem(0);
    }

    //@ Private
    private void SetStringLast(string strSetNew_)
    {
        int iIdxLast = m_comboBoxList.Length-1;
        m_comboBoxList[iIdxLast] = new GUIContent(strSetNew_);
    }

    //@ Process
    public void Initialize( float fPosLeft_Margin, float fPosTopPercent, float fWidth, float fHeight, string strSetLast )
    {
        m_rectComboBoxCustom = new Rect(fPosLeft_Margin, fPosTopPercent, fWidth, fHeight);

        m_strSetLast = strSetLast;
        SetClear();
    }

    void Start()
    {
        Texture2D texBack = new Texture2D(2,2);
        Color[] arrColorBack = new Color[] { Color.white, Color.white, Color.white, Color.white };

        texBack.SetPixels(arrColorBack);
        texBack.Apply();

        m_listStyle.normal.textColor = Color.white;
        m_listStyle.onHover.background = m_listStyle.hover.background = texBack;
        m_listStyle.padding.left = m_listStyle.padding.right = m_listStyle.padding.top = m_listStyle.padding.bottom = 4;
    }

    public void OnGUI_ComboBoxCustom()
    {
        if(true==m_bVisible)
        {
            //@ At least 1 more ComboItem in combo list.
            //if (m_comboBoxList.Length > 0)
            {
                int selectItemIndex = m_comboBoxControl.GetSelectedItemIndex();

                if (selectItemIndex > getIdxLast())
                {
                    selectItemIndex = 0;
                    m_comboBoxControl.SetSelectItemIndex(selectItemIndex);
                }

                selectItemIndex = m_comboBoxControl.SetComboList(   m_rectComboBoxCustom,
                                                                    m_comboBoxList[selectItemIndex].text,
                                                                    m_comboBoxList,
                                                                    m_listStyle);
            }

        } 
    } // private void OnGUI () 

} // public class CComboBoxCustom : MonoBehaviour
