using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectorButton : MonoBehaviour {
	public Text startButtonText;
	private Button btn;
	void Start() {
		btn = this.GetComponent<Button> ();
        btn.onClick.AddListener(OnClick);

		if (MusicManager.selected ==  GetComponentsInChildren<Text>()[0].text) {
			btn.Select();
		}
	}

	private void OnClick () {
		btn.Select();
		string songName = GetComponentsInChildren<Text>()[0].text;
		MusicManager.Select(songName);
		startButtonText.text = "Start " + songName;
		if (Input.GetKeyDown(KeyCode.Return)) {
			SceneManager.LoadScene("MainScene");
		}
	}
}
