using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPage : MonoBehaviour {
	public Text cameraName;
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

	public void OnCameraSelect () {
		Setting.cameraIndex ++;
		if (Setting.cameraIndex >= WebCamTexture.devices.Length) {
			Setting.cameraIndex = 0;
		}
		refreshCameraName();
	}

	private void refreshCameraName () {
		cameraName.text = WebCamTexture.devices[Setting.cameraIndex].name;
	}
}
