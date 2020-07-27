using UnityEngine;
using System.Collections;

public class lightingSpecVim : MonoBehaviour {
	public Shader shader;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalFloat("mixRatio", Mathf.Sin(Time.timeSinceLevelLoad));
	}
}
