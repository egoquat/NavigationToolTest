using UnityEngine;
using System;
using System.Collections;

public class CSelectBoxCustom : MonoBehaviour
{
    #region constparameter
    public static readonly Vector2 DFLT_SIZE_SELECTBOX = new Vector2(130.0f, 24.0f);
    public static readonly int DFLT_INDEX_SELECT = 0;
    #endregion // #region constparameter

    //@ ITEMS
    protected GUIContent[] _arrItemContents = new GUIContent[0];       

    protected int _IdxSelect_request = DFLT_INDEX_SELECT;
    protected int _IdxSelected = DFLT_INDEX_SELECT;
    protected bool _bVisible = true;
    protected Vector2 _v2Pos_Select = new Vector2();
    protected Vector2 _v2Size_AUnit = new Vector2();
    protected Vector2 _v2Size_Units = new Vector2();

    protected bool _IsSelectChangeSelBox = false;

    //@ Get/Set
    public int getCount()
    {
        if (null == _arrItemContents)
        {
            return -1;
        }

        return _arrItemContents.Length;
    } // public int getCount()

    public bool visible
    {
        get { return _bVisible; }
        set { _bVisible = value;}
    }

    public void setPosition(float fLeft, float fTop, float fWidth, float fHeight)
    {
        _v2Pos_Select.x = fLeft;
        _v2Pos_Select.y = fTop;
        _v2Size_AUnit.x = fWidth;
        _v2Size_AUnit.y = fHeight;
    } // public void setPosition(float fLeft, float fTop, float fWidth, float fHeight)

    public Rect getPosition()
    {
        return new Rect(_v2Pos_Select.x, _v2Pos_Select.y, _v2Size_AUnit.x, _v2Size_AUnit.y);
    }

    public int InsertNewItem(string strItem)
    {
        int iLengthList = _arrItemContents.Length;
        int iIdxItem = iLengthList;

        Array.Resize(ref _arrItemContents, iLengthList + 1);
        _arrItemContents[iIdxItem] = new GUIContent(strItem);

        _v2Size_Units.y = _v2Size_AUnit.y * (iLengthList + 1);

        //SetSelectItem(iIdxItem);

        return iIdxItem;
    } // public int InsertNewItem(string strItem)

    public int GetIdxSelect()
    {
        return _IdxSelected;
    }

    public bool GetChangedSelect()
    {
        return _IsSelectChangeSelBox;
    }

    public void SetSelectItem(int iSelectIdx)
    {
        if (iSelectIdx < 0 || iSelectIdx >= _arrItemContents.Length)
        {
            Debug.Log("OutOfRange ComboBox Select Idx.if (iSelectIdx < 0 || iSelectIdx >= m_comboBoxList.Length)//iSelectIdx=" + iSelectIdx + "//"
                    + UnityEngine.Random.Range(0.0f, 10000.0f));
            return;
        }

        _IdxSelect_request = iSelectIdx;
    } // public void SetSelectItem(int iSelectIdx)

    public void SetSelectItemForced(int iSelectIdx)
    {
        if (iSelectIdx < 0 || iSelectIdx >= _arrItemContents.Length)
        {
            Debug.Log("OutOfRange ComboBox Select Idx.if (iSelectIdx < 0 || iSelectIdx >= m_comboBoxList.Length)//iSelectIdx=" + iSelectIdx + "//"
                    + UnityEngine.Random.Range(0.0f, 10000.0f));
            return;
        }

        _IdxSelected = _IdxSelect_request = iSelectIdx;
    }

    public void SetClear()
    {
        _arrItemContents = new GUIContent[0];
    }

    //@ Process
    public void InitSelectBox(float fPosLeft, float fPosTop, float fWidth, float fHeight)
    {        
        SetClear();

        _v2Pos_Select.x = fPosLeft;
        _v2Pos_Select.y = fPosTop;

        _v2Size_AUnit.x = fWidth;
        _v2Size_AUnit.y = fHeight;

        _v2Size_Units.x = fWidth;
        _v2Size_Units.y = fHeight;

        _IdxSelect_request = _IdxSelected = 0;
    } // public void Initialize(float fPosLeft, float fPosTop, float fWidth, float fHeight)

    public void InitSelectBox(Rect rectSelectBox)
    {
        InitSelectBox(rectSelectBox.xMin, rectSelectBox.yMin, rectSelectBox.width, rectSelectBox.height);
    } 

	// Use this for initialization
	void Start () {
	
	}

    public void OnGUI_SelectBoxCustom()
    {
        //@ At least 1 more ComboItem in combo list.
        if (true==_bVisible)
        {
            int iIdxSelected_Previous = _IdxSelected;
            _IsSelectChangeSelBox = false;

            Rect rectSelectBox = new Rect(_v2Pos_Select.x, _v2Pos_Select.y, _v2Size_Units.x+200, _v2Size_Units.y-200);
            _IdxSelected = GUI.SelectionGrid(rectSelectBox, _IdxSelect_request, _arrItemContents, 4);

            if (iIdxSelected_Previous != _IdxSelected)
            {
                _IdxSelect_request = _IdxSelected;
                _IsSelectChangeSelBox = true;
            }
        } // if (m_comboBoxList.Length > 1)

    } // void OnGui()
} // public class CSelectBoxCustom : MonoBehaviour
