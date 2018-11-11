using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviors : MonoBehaviour {

	public float  endZ, travelTime;
	public AudioSource timeReference;
	public GameObject scoreObj;
	public GameObject brokenCube;
	public GameObject playerCamera;
	
	private float startZ;
	private TextMesh scoreText;
	public static int scoreCount = 0;
	private float startTime;

	void Start () {
		scoreText = scoreObj.GetComponent<TextMesh>();
		startTime = getExactTime();
		startZ = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 oldPosition = transform.position;
		float x = oldPosition.x;
		float y = oldPosition.y;
		float z = Lib.MapRange(getExactTime(), startTime, startTime + travelTime, startZ, endZ);
		transform.position = new Vector3(x, y, z);
	}

	private float getExactTime() {
		float t = timeReference.time;
		if (t != 0) {
			return t;
		}
		return Time.timeSinceLevelLoad - travelTime;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Wall")) {
			Selfdestruct();
			ShakeCamera();
			scoreCount -= 1;
			UpdateScore();
		} else if (other.gameObject.CompareTag("Baton")) {
			Selfdestruct();
			scoreCount += 1;
			UpdateScore();
		}	else if (other.gameObject.CompareTag("DebugWall")) {
			Selfdestruct();
			scoreCount += 1;
			UpdateScore();
		}
	}

	void ShakeCamera() {
		iTween.ShakeRotation(playerCamera, new Vector3(1, 1, 1), 0.7f);
		BatonController.vibrate();
	}

	void Selfdestruct() {
		Instantiate(brokenCube, transform.position, transform.rotation).SetActive(true);
		Destroy(gameObject);
	}

	void UpdateScore() {
		scoreText.text = "Score: " + scoreCount.ToString();
	}
}
