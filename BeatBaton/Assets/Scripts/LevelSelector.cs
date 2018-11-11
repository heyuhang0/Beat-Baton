using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

	public GameObject listContentItemTemplate;
	public Transform parentTransform;
	public GameObject contentView;

	private string[] indexRaw;
	// Use this for initialization
	void Start () {
		listContentItemTemplate.SetActive(false);
		indexRaw = Resources.Load<TextAsset>("Levels/_index").text.Split('\n');

		contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 30*indexRaw.Length);
		foreach (string line in indexRaw) {
			GameObject newOne = Instantiate(listContentItemTemplate, parentTransform);
			Text[] texts = newOne.GetComponentsInChildren<Text>();
			string[] indexes = line.Split(',');
			string assetName = indexes[1].Replace(" ", "");
			texts[0].text = indexes[0];
			texts[1].text = indexes[3].Replace(" ", "");
			texts[2].text = indexes[2].Replace(" ", "");
			texts[3].text = assetName;
			newOne.SetActive(true);
		}
	}

	public void OnBack() {
		SceneManager.LoadScene("StartPage");
	}

	public void OnStart() {
		SceneManager.LoadScene("MainScene");
	}
}
