using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorButton : MonoBehaviour {
	public Text startButtonText;
	private Button btn;
	void Start() {
		btn = this.GetComponent<Button> ();
        btn.onClick.AddListener(OnClick);
	}

	private void OnClick () {
		string songName = GetComponentsInChildren<Text>()[0].text;
		MusicManager.Select(songName);
		startButtonText.text = "Start " + songName;
	}
}
