using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviors : MonoBehaviour {

	public float speed;
	public GameObject scoreObj;
	public GameObject brokenCube;
	public GameObject playerCamera;
	
	private TextMesh scoreText;
	private static float scoreCount = 0;

	void Start () {
		scoreText = scoreObj.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(0, 0, -speed) * Time.deltaTime);
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Wall")) {
			Selfdestruct();
			ShakeCamera();
		} else if (other.gameObject.CompareTag("Baton")) {
			Selfdestruct();
			scoreCount ++;
			UpdateScore();
		}
	}

	void ShakeCamera() {
		iTween.ShakeRotation(playerCamera, new Vector3(2, 2, 2), 1);
	}

	void Selfdestruct() {
		Instantiate(brokenCube, transform.position, transform.rotation).SetActive(true);
		Destroy(gameObject);
	}

	void UpdateScore() {
		scoreText.text = "Score: " + scoreCount.ToString();
	}
}
