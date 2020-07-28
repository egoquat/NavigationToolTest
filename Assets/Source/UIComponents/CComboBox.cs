using UnityEngine;

public class CComboBox
{
    #region constparameter
    public static int _DFLT_CNT_EVENTCLICKED = 15;
    #endregion // #region constparameter

    private static bool _forceToUnShow = false;
    private static int _useControlID = -1;

    private bool _isClickedComboButton = false;

    private bool _isClickedMouseButton = false;

    private int _isClickedComboButton_Current = 0;

    private bool _bChangedSelect = false;

    private int _selectedItemIndex = 0;
    private int _selectedItemIndex_current = 0;

    public int SetComboList(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle listStyle)
    {
        return SetComboList(rect, new GUIContent(buttonText), listContent, "button", "box", listStyle);
    }

    public int SetComboList(Rect rect, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
    {
        return SetComboList(rect, buttonContent, listContent, "button", "box", listStyle);
    }

    public int SetComboList(Rect rect, string buttonText, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
    {
        return SetComboList(rect, new GUIContent(buttonText), listContent, buttonStyle, boxStyle, listStyle);
    }

    public int SetComboList(Rect rect, GUIContent buttonContent, GUIContent[] listContent,
                                    GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
    {
        if(_forceToUnShow)
        {
            _forceToUnShow = false;
            _isClickedComboButton = false;
        }

        bool done = false;
        _isClickedMouseButton = false;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);

        //@ Event UI
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseUp:
                {
                    if(_isClickedComboButton)
                    {
                        done = true;
                    }
                }
                break;
            case EventType.MouseDown:
                {
                    _isClickedMouseButton = true;
                }
                break;
        }

        //@ Select Button Clicked or Not.
        if(GUI.Button(rect, buttonContent, buttonStyle))
        {
            if(_useControlID != -1)
            {
                _useControlID = controlID;
                _isClickedComboButton = false;
            }

            if (_useControlID != controlID)
            {
                _forceToUnShow = true;
                _useControlID = controlID;
            }
            _isClickedComboButton = true;
        } // if(GUI.Button(rect, buttonContent, buttonStyle))

        _bChangedSelect = false;

        if (true == _isClickedComboButton || _selectedItemIndex_current != _selectedItemIndex)
        {
            Rect listRect = new Rect(rect.x, rect.y + listStyle.CalcHeight(listContent[0], 1.0f),
                                rect.width, listStyle.CalcHeight(listContent[0], 1.0f) * listContent.Length);

            GUI.Box(listRect, "", boxStyle);
            _selectedItemIndex_current = GUI.SelectionGrid(listRect, _selectedItemIndex, listContent, 1, listStyle);

            if (_selectedItemIndex_current != _selectedItemIndex)
            {
                _selectedItemIndex = _selectedItemIndex_current;
                _bChangedSelect = true;
            }

            if(_isClickedMouseButton)
            {
                _isClickedComboButton_Current = _DFLT_CNT_EVENTCLICKED;
            }
        } // if(_isClickedComboButton)

        if(done)
            _isClickedComboButton = false;

        return GetSelectedItemIndex();
    }

    //@Get/Set
    public bool IsSelectChangedComboBox()
    {
        return _bChangedSelect;   
    }

    public int GetSelectedItemIndex()
    {
        //return _selectedItemIndex;
        return _selectedItemIndex_current;
    }

    public void SetSelectItemIndex(int iIdxSelect)
    {
        _selectedItemIndex = iIdxSelect;
    }

    public bool IsClickedComboBox()
    {
        if(0<_isClickedComboButton_Current--)
        {
            return true;
        }
        return false;
    } // public bool IsClickedComboBox()

    public void setClickedComboBox()
    {
        _isClickedComboButton_Current = _DFLT_CNT_EVENTCLICKED;
    }
} // public class CComboBox

