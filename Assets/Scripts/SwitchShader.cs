using UnityEngine;
using System.Collections;

public class SwitchShader : MonoBehaviour {
	
	public string shaderString = "tk2d/LitBlendVertexColor";
	
	void Start () {
		renderer.material.shader = Shader.Find (shaderString);
	}
}
