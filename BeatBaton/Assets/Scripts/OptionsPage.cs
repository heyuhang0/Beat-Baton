using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsPage : MonoBehaviour {
	public Text cameraName, camResolution, camWindowEnable;
	public InputField externalMusicPath;
	// Use this for initialization
	void Start () {
		refreshCameraName();
		refreshResolution();
		refreshShowCam();
		externalMusicPath.text = Setting.a.musicFolder;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy () {
		Setting.SaveGameData();
	}

	public void OnCameraSelect () {
		Setting.a.cameraIndex ++;
		if (Setting.a.cameraIndex >= WebCamTexture.devices.Length) {
			Setting.a.cameraIndex = 0;
		}
		SceneManager.LoadScene("Options"); // Reload page to load new camera
	}

	public void onCamResSelect() {
		if (Setting.a.cameraMultiplier == 1) {
			Setting.a.cameraMultiplier = 2;
		} else {
			Setting.a.cameraMultiplier = 1;
		}
		refreshResolution();
	}

	public void OnBack() {
		Setting.a.musicFolder = externalMusicPath.text;
		SceneManager.LoadScene("StartPage");
	}

	public void OnShowCam () {
		Setting.a.cameraWindowEnable = !Setting.a.cameraWindowEnable;
		refreshShowCam();
	}

	private void refreshCameraName () {
		if (WebCamTexture.devices.Length == 0) {
			cameraName.text = "No camera found";
		} else {
			if (Setting.a.cameraIndex >= WebCamTexture.devices.Length) {
				Setting.a.cameraIndex = 0;
			}
		}
		cameraName.text = WebCamTexture.devices[Setting.a.cameraIndex].name;
	}

	private void refreshResolution () {
		int m = Setting.a.cameraMultiplier;
		camResolution.text = (320 * m).ToString() + " X " + (240 * m).ToString();
	}

	private void refreshShowCam () {
		camWindowEnable.text = Setting.a.cameraWindowEnable.ToString().ToUpper();
	}
}
