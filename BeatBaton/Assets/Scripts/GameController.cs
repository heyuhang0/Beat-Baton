using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public GameObject cubeTemplate, AudioSamplerObj;
	public AudioSource SilentAudio, NormalAudio;
	public float initialZ;
	public Vector2 cubeRangeX, cubeRangeY;
	public float MusicDelay;
	public GameObject finalCanvas, pauseCanvas, debugCanvas;
	public Text finalSocre;

	private bool gameEnded = false;
	// Use this for initialization
	void Start () {
		CubeBehaviors.scoreCount = 0;
		Time.timeScale = 1;
		cubeTemplate.SetActive(false);
		pauseCanvas.SetActive(false);
		finalCanvas.SetActive(false);
		debugCanvas.SetActive(true);

		string fullLevelName = "Levels/" + Setting.a.level;
		LoadMusic(fullLevelName);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) && !gameEnded) {
			if (IsPaused()) {
				Resume();
			} else {
				Pause();
			}
		}

		if (!SilentAudio.isPlaying && !NormalAudio.isPlaying && Time.timeScale == 1 && !gameEnded) {
			gameEnded = true;
			finalCanvas.SetActive(true);
			finalSocre.text = CubeBehaviors.scoreCount.ToString();
		}
	}

	public void onFutureBeat() {
		CreateNewCube(Random.Range(cubeRangeX.x, cubeRangeX.y), Random.Range(cubeRangeY.x, cubeRangeY.y));
	}

	private bool IsPaused() {
		return Time.timeScale == 0;
	}

	private bool musicPlayingBeforePause;
	private void Pause() {
		pauseCanvas.SetActive(true);
		Time.timeScale = 0;

		musicPlayingBeforePause = AudioVisualizer.AudioSampler.instance.IsPlaying();
		if (musicPlayingBeforePause)
			AudioVisualizer.AudioSampler.instance.Pause();
	}

	private void Resume() {
		pauseCanvas.SetActive(false);
		Time.timeScale = 1;
		if (musicPlayingBeforePause) {
			AudioVisualizer.AudioSampler.instance.Play();
		}
	}

	private void LoadMusic(string name) {
		AudioClip ac = Resources.Load<AudioClip>(name);
		AudioSource[] ass = AudioSamplerObj.GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audioSource in ass) {
			audioSource.clip = ac;
		}
		AudioVisualizer.AudioSampler.instance.PlayWithPreBeatOffset(MusicDelay);
	}

	public void OnResumeClick() {
		Resume();
	}

	public void OnRestart() {
		SceneManager.LoadScene("MainScene");
	}

	public void OnExit() {
		SceneManager.LoadScene("LevelSelect");
	}

	void CreateNewCube (float x, float y) {
		GameObject newCube = Instantiate(cubeTemplate);
		newCube.transform.SetPositionAndRotation(new Vector3(x, y, initialZ), new Quaternion());
		newCube.SetActive(true);
	}
}
