using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetting : MonoBehaviour {
	public Slider sliderH, sliderS, sliderV;
	// Use this for initialization
	void Start () {
		sliderH.onValueChanged.AddListener(OnHValueChange);
		sliderS.onValueChanged.AddListener(OnSValueChange);
		sliderV.onValueChanged.AddListener(OnVValueChange);
		sliderH.value = Setting.a.h;
		sliderS.value = Setting.a.s;
		sliderV.value = Setting.a.v;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<RawImage>().color = Color.HSVToRGB(Setting.a.h, Setting.a.s,  Setting.a.v);
	}

	public void OnHValueChange (float newValue) {
		Setting.a.h = newValue;
	}

	public void OnSValueChange (float newValue) {
		Setting.a.s = newValue;
	}

	public void OnVValueChange (float newValue) {
		Setting.a.v = newValue;
	}
}
