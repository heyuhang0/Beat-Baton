using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManagerButton : MonoBehaviour {

	private Button btn;
	private static List<Button> btns = new List<Button>();

	void Start () {
		btn = this.GetComponent<Button> ();
        btn.onClick.AddListener(OnClick);
		btns.Add(btn);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnClick () {
		btn.Select();
		ProfileManager.selectedProfile = GetComponentInChildren<Text>().text;
	}
}
