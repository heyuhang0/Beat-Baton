using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Setting : MonoBehaviour {
	public static GameData a;
	private static string dataFilePath = "/data.json";
	static Setting() {
		LoadGameData();
	}

	public static void SaveGameData()
    {
        string dataAsJson = JsonUtility.ToJson(a);

        string filePath = Application.dataPath + dataFilePath;
        File.WriteAllText (filePath, dataAsJson);

		Debug.Log("Game data saved");
    }

	private static void LoadGameData()
    {
        string filePath = Application.dataPath + dataFilePath;

        if (File.Exists (filePath)) {
            string dataAsJson = File.ReadAllText (filePath);
            a = JsonUtility.FromJson<GameData> (dataAsJson);
			Debug.Log("Game data loaded");
        } else {
            LoadDefault();
			Debug.Log("Game data sets to default");
        }
    }

	private static void LoadDefault () {
		a = new GameData();
		a.cameraIndex = 0;
		a.cameraMultipiler = 2;
		a.cameraWindowEnable = true;
		a.level = "WakeMeUp";

		a.batonProfiles = new List<BatonProfile>();
	}
}

[Serializable]
public class GameData {
	public int cameraIndex;
	public int cameraMultipiler;
	public bool cameraWindowEnable;
	public string level;
	public List<BatonProfile> batonProfiles;
}

[Serializable]
public class BatonProfile: IComparable<BatonProfile> {
	public string profileName;
	public Vector3 hsvLower, hsvUpper;
	public System.DateTime lastConnectionTime;
	public Vector2 position;
	public Quaternion direction;
	public Color color;
	public int score;

	public BatonProfile (string name, Vector3 hsvLower, Vector3 hsvUpper, Color color) {
		this.profileName = name;
		this.hsvLower = hsvLower;
		this.hsvUpper = hsvUpper;
		this.lastConnectionTime = System.DateTime.Now;
		this.color = color;

		this.direction = new Quaternion();
		this.position = new Vector2();
	}

	public int CompareTo(BatonProfile other) {
		return other.score - this.score;
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
		return (System.DateTime.Now - this.lastConnectionTime).TotalSeconds < 3;
	}
}
