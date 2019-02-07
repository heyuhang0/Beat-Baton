using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

	public GameObject listContentItemTemplate;
	public Transform parentTransform;
	public GameObject contentView;

	// Use this for initialization
	void Start () {
		MusicManager.ReloadMusic();
		listContentItemTemplate.SetActive(false);

		foreach (var item in MusicManager.playlist) {
			AddItem(item.Key, (int)item.Value.length);
		}
	}

	private int levelsContentHeight = 0;
	private void AddItem(string name, int seconds) {
		levelsContentHeight += 30;
		contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(600, levelsContentHeight);
		GameObject newOne = Instantiate(listContentItemTemplate, parentTransform);
		Text[] texts = newOne.GetComponentsInChildren<Text>();
		texts[0].text = name;
		string timeText = string.Format("{0:D2}:{1:D2}", seconds / 60, seconds % 60);
		texts[1].text = timeText;
		newOne.SetActive(true);
	}

	public void OnDestroy () {
		Setting.SaveGameData();
	}

	public void OnBack() {
		SceneManager.LoadScene("StartPage");
	}

	public void OnStart() {
		SceneManager.LoadScene("MainScene");
	}
}
