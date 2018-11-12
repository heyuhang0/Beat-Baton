using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour {
	public static int cameraIndex;
	public static int cameraMultipiler;
	public static bool cameraWindowEnable;
	public static string level;
	public static List<BatonProfile> batonProfiles;

	static Setting() {
		LoadDefault();
	}

	static void LoadDefault () {
		cameraIndex = 0;
		cameraMultipiler = 2;
		cameraWindowEnable = true;
		level = "WakeMeUp";

		batonProfiles = new List<BatonProfile>();
		// batonProfiles.Add(new BatonProfile("LightGreen", new Vector3(50, 80, 10), new Vector3(80, 255, 255), Color.green));
	}
}

public class BatonProfile {
	public string profileName;
	public Vector3 hsvLower, hsvUpper;
	public System.DateTime lastConnectionTime;
	public Vector2 position;
	public Quaternion direction;
	public Color color;

	public BatonProfile (string name, Vector3 hsvLower, Vector3 hsvUpper, Color color) {
		this.profileName = name;
		this.hsvLower = hsvLower;
		this.hsvUpper = hsvUpper;
		this.lastConnectionTime = System.DateTime.Now;
		this.color = color;

		this.direction = new Quaternion();
		this.position = new Vector2();
	}

	public BatonProfile () {
		this.lastConnectionTime = System.DateTime.Now;

		this.direction = new Quaternion();
		this.position = new Vector2();
	}

	public void SetActive() {
		this.lastConnectionTime = System.DateTime.Now;
	}

	public bool IsActive() {
		return (System.DateTime.Now - this.lastConnectionTime).Seconds < 3;
	}
}
