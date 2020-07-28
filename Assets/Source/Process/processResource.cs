using UnityEngine;
using System.Collections;

public class processResource : MonoBehaviour 
{
    //@ Ex-link resource
    public navimeshResource _navimeshResource;
    public CUnitFactory _unitfactory;
    public CProcessInput _processInput;
    public CCurvePathUnit _curvePathUnit_src;
    public CCurvePathLineDraw _curvePathLineDraw_src;
    public CDrawText3D _drawText3D_src;
    public UnitFlying _unitFlying_src;
    public UnitWalking _unitWalking_src;
    //@Ex-link - UI
    public CSelectBoxCustom _selectBox_src;        //Select box 
    public CComboBoxCustom _combo_src;             //Combo box // curve path, navigation mesh

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {
        processCycle.GetInstance.Initialize(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
