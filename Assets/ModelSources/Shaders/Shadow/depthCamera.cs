using UnityEngine;
using System.Collections;

public class depthCamera : MonoBehaviour {
	public Shader depthShader;
	// Use this for initialization
	void Start () {
		GetComponent<Camera>().SetReplacementShader (depthShader, null);
		GetComponent<Camera>().targetTexture.format = RenderTextureFormat.Depth;
//		camera.depth = Camera.main.depth - 1;
		
//		GL.Viewport(new Rect(0, 0, Screen.width, Screen.height));
//		camera.viewport.y = Mathf.Min(viewport.y,Screen.height+viewport.height );
//		camera.rect = Rect (0, 0, 1 - , 1);
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalMatrix("depthcam_viewproj_matrix", GetComponent<Camera>().projectionMatrix*GetComponent<Camera>().worldToCameraMatrix);

		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		if(ray.direction.y != 0){
			float lengthToY0 = Mathf.Abs(ray.origin.y / ray.direction.y);
			
			Vector3 pointY0 = ray.origin + ray.direction * lengthToY0;
			
			Ray lightRay = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			
			transform.position = pointY0 - lightRay.direction* lengthToY0*3;
			GetComponent<Camera>().orthographicSize = lengthToY0*1.5f;
		}
	}
}
