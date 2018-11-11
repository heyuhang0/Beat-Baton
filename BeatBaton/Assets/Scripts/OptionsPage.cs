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

	public void OnCameraSelect () {
		Setting.cameraIndex ++;
		if (Setting.cameraIndex >= WebCamTexture.devices.Length) {
			Setting.cameraIndex = 0;
		}
		refreshCameraName();
	}

	public void onCamResSelect() {
		if (Setting.cameraMultipiler == 1) {
			Setting.cameraMultipiler = 2;
		} else {
			Setting.cameraMultipiler = 1;
		}
		refreshResolution();
	}

	public void OnBack() {
		SceneManager.LoadScene("StartPage");
	}

	public void OnShowCam () {
		Setting.cameraWindowEnable = !Setting.cameraWindowEnable;
		refreshShowCam();
	}

	private void refreshCameraName () {
		if (WebCamTexture.devices.Length == 0) {
			cameraName.text = "No camera found";
		} else {
			if (Setting.cameraIndex >= WebCamTexture.devices.Length) {
				Setting.cameraIndex = 0;
			}
		}
		cameraName.text = WebCamTexture.devices[Setting.cameraIndex].name;
	}

	private void refreshResolution () {
		int m = Setting.cameraMultipiler;
		camResolution.text = (320 * m).ToString() + " X " + (240 * m).ToString();
	}

	private void refreshShowCam () {
		camWindowEnable.text = Setting.cameraWindowEnable.ToString().ToUpper();
	}
}
