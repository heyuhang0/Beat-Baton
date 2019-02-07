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
	public GameObject scoreCanvas, scoreViewContent, scoreTemplate;

	private bool gameEnded = false;
	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
		cubeTemplate.SetActive(false);
		pauseCanvas.SetActive(false);
		finalCanvas.SetActive(false);
		debugCanvas.SetActive(true);
		scoreTemplate.SetActive(false);

		LoadMusic();
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
			OnGameOver();
		}
	}

	void OnGameOver() {
		scoreCanvas.SetActive(false);
		finalCanvas.SetActive(true);

		List<BatonProfile> nonZeroBatons = new List<BatonProfile>();
		foreach (BatonProfile b in Setting.a.batonProfiles) {
            if (b.score != 0)
				nonZeroBatons.Add(b);
        }
		if (nonZeroBatons.Capacity == 0) {
			return;
		}
		nonZeroBatons.Sort();
		Debug.Log(nonZeroBatons.Capacity);
		scoreViewContent.GetComponent<RectTransform>().sizeDelta = new Vector2(338, 40*nonZeroBatons.Count);
		int highestScore = nonZeroBatons[0].score;
		foreach (BatonProfile b in nonZeroBatons) {
			CreateNewScoreItem(b.color, b.score, (float)b.score / highestScore);
		}
	}

	void CreateNewScoreItem(Color color, int score, float length) {
		GameObject newScoreItem = Instantiate(scoreTemplate);
		newScoreItem.transform.SetParent(scoreTemplate.transform.parent);
		newScoreItem.GetComponentInChildren<Text>().text = score.ToString();
		newScoreItem.GetComponentInChildren<RawImage>().color = color;
		newScoreItem.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(500*length-200, 40);
		newScoreItem.SetActive(true);
	}

	public void onFutureBeat() {
		float x = 0, y = 0;
		for (int i = 0; i < 3 && !IsGoodPosition(x, y); i++) {
			x = Random.Range(-1f, 1f);
			y = Random.Range(-1f, 1f);
			Debug.Log(x + "\t" + y + "\t" + lastCubeX + "\t" + lastCubeY);
		}
		x = Lib.MapRange(x, -1, 1, cubeRangeX.x, cubeRangeX.y);
		y = Lib.MapRange(y, -1, 1, cubeRangeY.x, cubeRangeY.y);
		CreateNewCube(x, y);
	}

	float lastCubeX = 0, lastCubeY = 0;
	private bool IsGoodPosition(float x, float y) {
		if (Mathf.Abs(x) < 0.2f && Mathf.Abs(y) < 0.5f)
			return false;
		if (Mathf.Pow((x - lastCubeX), 2) + Mathf.Pow((y - lastCubeY), 2) < 0.3*0.3)
			return false;
		lastCubeX = x;
		lastCubeY = y;
		return true;
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

	private void LoadMusic() {
		AudioClip ac = MusicManager.GetSelected();
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
