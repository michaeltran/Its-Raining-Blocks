using UnityEngine;
using System.Collections;

public class DetectorCreator : MonoBehaviour {
	
	public int numberOfDetectors;
	
	public GameObject prefabColumnDetector;

	void Start () {
		CreateDetectors ();
	}
	
	void CreateDetectors()
	{
		Vector3 position = new Vector3(3f,16f,0f);
		for(int i = 0; i < numberOfDetectors; i++)
		{
			Instantiate(prefabColumnDetector, position, Quaternion.identity);
			position.x += 2.5f;
		}
	}
	
}
