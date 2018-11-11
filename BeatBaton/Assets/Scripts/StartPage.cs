using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class StartPage : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnButtonStart () {
		Setting.level = "WakeMeUp";
		SceneManager.LoadScene("LevelSelect");
	}

	public void onButtonOptions () {
		SceneManager.LoadScene("Options");
	}

	public void onButtonExit () {
		Application.Quit();
	}
}
