using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	private AudioSource audioSource;
	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = Resources.Load<AudioClip>("Audio/WakeMeUp");
		audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale == 0) {
				Time.timeScale = 1;
				audioSource.Play();
			} else {
				Time.timeScale = 0;
				audioSource.Pause();
			}
		}
	}
}
