using UnityEngine;
using System.Collections;

public class ambientGlow : MonoBehaviour {
	public Shader swapToShader;
	public RenderTexture rTexture;
	public Material compositeMat;
	GameObject shaderCamera;

	// Use this for initialization
	void Start () {
		shaderCamera = new GameObject("ambientCamera");
		shaderCamera.AddComponent<Camera>();
		shaderCamera.AddComponent <BlurEffect>();
		shaderCamera.hideFlags = HideFlags.HideAndDontSave;
		
		rTexture = new RenderTexture ((int)Camera.main.pixelWidth/2, (int)Camera.main.pixelHeight/2, 16);
		rTexture.wrapMode = TextureWrapMode.Clamp;
	}
	
/*
	void LateUpdate () {
	}
*/
	void OnPreRender(){
		Camera cam = shaderCamera.GetComponent<Camera>();
		cam.CopyFrom (Camera.main);
		cam.backgroundColor = new Color(0,0,0,1);
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.targetTexture = rTexture;
		cam.SetReplacementShader(swapToShader, "AmbientType");
		cam.Render();
	}
/*
	void OnRenderImage (RenderTexture source, RenderTexture destination){
		Graphics.Blit(source, destination, compositeMat);
	}
*/
	void OnPostRender (){
		Graphics.Blit(rTexture, null, compositeMat);
	}
}
