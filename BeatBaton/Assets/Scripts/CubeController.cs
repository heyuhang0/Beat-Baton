using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

	public GameObject cubeTemplate;
	public float initialZ;
	private float nextCubeTime;

	// Use this for initialization
	void Start () {
		cubeTemplate.SetActive(false);

		nextCubeTime = Time.time + 0.4f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextCubeTime) {
			nextCubeTime = Time.time + 1/Setting.cubeFreq;
			CreateNewCube(Random.Range(-3, 3), Random.Range(0.5f, 3));
		}
	}

	void CreateNewCube (float x, float y) {
		GameObject newCube = Instantiate(cubeTemplate);
		newCube.transform.SetPositionAndRotation(new Vector3(x, y, initialZ), new Quaternion());
		newCube.SetActive(true);
	}
}
