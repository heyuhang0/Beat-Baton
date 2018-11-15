using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting {
	public static UserSetting a;
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
            a = JsonUtility.FromJson<UserSetting> (dataAsJson);
			Debug.Log("Game data loaded");
        } else {
            LoadDefault();
			Debug.Log("Game data sets to default");
        }
    }

	private static void LoadDefault () {
		a = new UserSetting();
		a.name = "";
		a.ip = "";
		a.h = 0;
        a.s = 0;
        a.v = 0;
	}
}

[Serializable]
public class UserSetting {
    public string name;
    public string ip;
    public float h, s, v;
}