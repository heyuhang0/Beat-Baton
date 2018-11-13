using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsPage : MonoBehaviour {
	public Text cameraName, camResolution, camWindowEnable;
	// Use this for initialization
	void Start () {
		refreshCameraName();
		refreshResolution();
		refreshShowCam();
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
		refreshCameraName();
	}

	public void onCamResSelect() {
		if (Setting.a.cameraMultipiler == 1) {
			Setting.a.cameraMultipiler = 2;
		} else {
			Setting.a.cameraMultipiler = 1;
		}
		refreshResolution();
	}

	public void OnBack() {
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
		int m = Setting.a.cameraMultipiler;
		camResolution.text = (320 * m).ToString() + " X " + (240 * m).ToString();
	}

	private void refreshShowCam () {
		camWindowEnable.text = Setting.a.cameraWindowEnable.ToString().ToUpper();
	}
}
