using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class StartPage : MonoBehaviour {

	public Text cameraName;
	private int cameraIndex = 0;
	// Use this for initialization
	void Start () {
		if (WebCamTexture.devices.Length == 0) {
			cameraName.text = "No camera found";
		} else {
			refreshCameraName();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnButtonStart () {
		Setting.level = "WakeMeUp";
		SceneManager.LoadScene("MainScene");
	}

	public void OnButtonStart2 () {
		Setting.level = "60_bpm";
		SceneManager.LoadScene("MainScene");
	}

	public void OnCameraSelect () {
		cameraIndex ++;
		if (cameraIndex >= WebCamTexture.devices.Length) {
			cameraIndex = 0;
		}
		refreshCameraName();
		Setting.cameraIndex = cameraIndex;
	}

	private void refreshCameraName () {
		cameraName.text = WebCamTexture.devices[cameraIndex].name;
	}
}
