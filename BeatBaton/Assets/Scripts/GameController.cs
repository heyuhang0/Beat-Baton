using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject cubeTemplate;
	public float initialZ;
	public Vector2 cubeRangeX, cubeRangeY;
	public float MusicDelay;

	private float nextCubeTime;
	private CubeSequence cubeSequence;
	private AudioSource audioSource;
	// Use this for initialization
	void Start () {
		cubeTemplate.SetActive(false);

		string fullLevelName = "Levels/" + Setting.level;
		LoadMusic(fullLevelName);
		LoadSequence(fullLevelName);

		nextCubeTime = cubeSequence.GetNextTime();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale == 0) {
				Time.timeScale = 1;
				audioSource.Play();
			} else {
				Time.timeScale = 0;
				audioSource.Pause();
			}
		}
		if (Time.time > MusicDelay) {
			audioSource.Play();
			MusicDelay = float.MaxValue;
		}

		Debug.Log(Time.time);

		if (Time.time > nextCubeTime) {
			Vector2[] cubes = cubeSequence.GetNext();

			foreach (Vector2 cube in cubes) {
				float x = MapRange(cube.x, -1, 1, cubeRangeX.x, cubeRangeX.y);
				float y = MapRange(cube.y, -1, 1, cubeRangeY.x, cubeRangeY.y);
				CreateNewCube(x, y);
			}

			if (cubeSequence.HasNext()) {
				nextCubeTime = cubeSequence.GetNextTime();
			} else {
				nextCubeTime = float.MaxValue;
			}
		}
	}

	void LoadMusic(string name) {
		audioSource = gameObject.GetComponent<AudioSource>();
		audioSource.clip = Resources.Load<AudioClip>(name);
	}

	void LoadSequence(string name) {
		TextAsset cubeSequenceText = Resources.Load<TextAsset>(name);
		cubeSequence = new CubeSequence(cubeSequenceText.text);
	}

	void CreateNewCube (float x, float y) {
		GameObject newCube = Instantiate(cubeTemplate);
		newCube.transform.SetPositionAndRotation(new Vector3(x, y, initialZ), new Quaternion());
		newCube.SetActive(true);
	}

	float MapRange(float value, float sourceLow, float sourceHigh, float dstLow, float dstHigh)
    {
        return (value - sourceLow) / (sourceHigh - sourceLow) * (dstHigh - dstLow) + dstLow;
    }

	class CubeSequence {
		private float[] timestamps;
		private Vector2[][] cubes;
		private int index = 0;

		public CubeSequence(string raw) {
			ArrayList timestampsL = new ArrayList();
			ArrayList cubesL = new ArrayList();

			raw = raw.Replace("\r", "");
			raw = raw.Replace("[", "");
			raw = raw.Replace("]", "");

			string[] lines = raw.Split('\n');
			foreach (string line in lines) {
				string[] parts = line.Split('(');

				if (parts.Length < 2) {
					continue;
				}

				float time = ToTime(parts[0]);

				Vector2[] cubesPerFrame = new Vector2[parts.Length - 1];
				for (int i = 1; i < parts.Length; i++) {
					cubesPerFrame[i - 1] = ToPos(parts[i]);
				}

				timestampsL.Add(time);
				cubesL.Add(cubesPerFrame);
			}

			timestamps = (float[])timestampsL.ToArray(typeof(float));
			cubes = (Vector2[][])cubesL.ToArray(typeof(Vector2[]));

			/*
			for (int i = 0; i < timestamps.Length; i++) {
				string debugMes = timestamps[i].ToString() + ": ";

				foreach (Vector2 pos in cubes[i]) {
					debugMes += pos.ToString() + ",";
				}
				Debug.Log(debugMes);
			}*/
		}

		public bool HasNext() {
			return index < timestamps.Length;
		}

		public Vector2[] GetNext() {
			return cubes[index ++];
		}

		public float GetNextTime() {
			return timestamps[index];
		}

		private float ToTime(string timestamp) {
			string[] parts = timestamp.Split(':');
			int minute = int.Parse(parts[0]);
			float seconds = float.Parse(parts[1]);

			return minute * 60 + seconds;
		}

		private Vector2 ToPos(string coordinate) {
			coordinate = coordinate.Replace("(", "");
			coordinate = coordinate.Replace(")", "");
			string[] parts = coordinate.Split(',');
			float x = float.Parse(parts[0]);
			float y = float.Parse(parts[1]);
			return new Vector2(x, y);
		}
	}
}
