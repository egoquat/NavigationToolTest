using UnityEngine;
using System.Collections;

public class shadowReceiver : MonoBehaviour {
	public Camera shadowCam;
	public Material mat;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		mat.SetMatrix("depthcam_viewproj_matrix", shadowCam.projectionMatrix*shadowCam.worldToCameraMatrix);
		mat.SetMatrix("depthcam_view_matrix", shadowCam.worldToCameraMatrix);		
	}
}
