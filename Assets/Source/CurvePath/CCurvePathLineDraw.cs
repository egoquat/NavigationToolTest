using UnityEngine;
using System.Collections;

public class CCurvePathLineDraw : MonoBehaviour
{
    public void SetDiffuseColor(Color colorDiffuse)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = colorDiffuse;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
