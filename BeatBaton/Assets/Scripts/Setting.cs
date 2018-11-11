using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour {
	public static int cameraIndex;
	public static int cameraMultipiler;
	public static bool cameraWindowEnable;
	public static string level;

	static Setting() {
		LoadDefault();
	}

	static void LoadDefault () {
		cameraIndex = 0;
		cameraMultipiler = 2;
		cameraWindowEnable = true;
		level = "WakeMeUp";
	}
}
