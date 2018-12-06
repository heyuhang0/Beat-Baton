using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using NWH;

public class ProfileManager : MonoBehaviour {

	public RawImage cameraPreview;
	public GameObject listContentItemTemplate;
	public Transform parentTransform;
	public GameObject contentView;
	public GameObject helpText2;
	public GameObject helpText3;
	public int serverPort;

	private WebCamTexture webCamTexture;
	private Texture2D tex;
	private Mat frame;
	private BatonProfile newProfile;

	private enum State {Normal, Waitting, OnPress, Calibrating};
	private State state = State.Normal;

	public static string selectedProfile = "";

	void Start () {
		SetupCamera();
		frame = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4);

		RefreshProfilesList();

		MyNetworkServer.RegisterHandler(64, OnServerReceived);
	}
	
	void Update () {
		if (webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
        {
            CamUpdate();
        }
	}

	private int hl, hh, sl, sh, vl, vh;
	void CamUpdate() {
		CvUtil.GetWebCamMat(webCamTexture, ref frame);
		Cv2.Flip(frame, frame, FlipMode.Y);
		switch (state) {
			case State.Normal:
			break;

			case State.Waitting:
				Cv2.Circle(frame, 320, 240, 30, new Scalar(255, 0, 0), 3);
				break;

			case State.OnPress:
				Mat blurred = new Mat();
				Mat hsvMat = new Mat();

				Cv2.GaussianBlur(frame, blurred, new Size(9, 9), 0);
				Cv2.CvtColor(blurred, hsvMat, ColorConversionCodes.RGB2HSV);
				
				Vec3b rgb = blurred.Get<Vec3b>(240, 320);
				Vec3b hsv = hsvMat.Get<Vec3b>(240, 320);

				blurred.Release();
				hsvMat.Release();

				newProfile.color = new Color((float)rgb.Item0/255, (float)rgb.Item1/255, (float)rgb.Item2/255);

				hl = hsv.Item0 - 5;
				sl = Mathf.Max(hsv.Item1 - 50, 60);
				vl = Mathf.Max(hsv.Item2 - 50, 30);
				hh = hsv.Item0 + 5;
				sh = Mathf.Min(hsv.Item1 + 50, 255);
				vh = Mathf.Min(hsv.Item2 + 50, 255);
				
				state = State.Calibrating;
				helpText2.SetActive(false);
				break;

			case State.Calibrating:
				newProfile.hsvLower = new Vector3(hl, sl, vl);
				newProfile.hsvUpper = new Vector3(hh, sh, vh);
				Debug.Log("Color range: " + newProfile.hsvLower.ToString() + '\t' + newProfile.hsvUpper.ToString());
				Setting.a.batonProfiles.Add(newProfile);
				RefreshProfilesList();
				state = State.Normal;
				break;
		}
		UpdatePreview(frame);
	}

	public void OnNew () {
		if (state != State.Normal)
			return;
		state = State.Waitting;
		helpText2.SetActive(true);
		newProfile = new BatonProfile();
	}

	public void OnDelete () {
		foreach (BatonProfile p in Setting.a.batonProfiles) {
			if (p.profileName == selectedProfile) {
				Setting.a.batonProfiles.Remove(p);
				break;
			}
		}
		RefreshProfilesList();
	}

	private void OnServerReceived(NetworkMessage netMsg)
    {
		if (state == State.Waitting) {
			string nameRecv = netMsg.ReadMessage<UserMessage>().profile;

			foreach (BatonProfile p in Setting.a.batonProfiles) {
				if (p.profileName == nameRecv) {
					return;
				}
			}
			Debug.Log("Calibrating...");
			newProfile.profileName = nameRecv;
			state = State.OnPress;
		}
    }

	void RefreshProfilesList () {
		listContentItemTemplate.SetActive(false);
		contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 30 * Setting.a.batonProfiles.Count);

		foreach (Transform child in contentView.transform) {
			if (child.gameObject != listContentItemTemplate) {
				Destroy(child.gameObject);
			}
		}

		foreach (BatonProfile p in Setting.a.batonProfiles) {
			GameObject newOne = Instantiate(listContentItemTemplate, parentTransform);
			Text[] texts = newOne.GetComponentsInChildren<Text>();
			texts[0].text = p.profileName;
			newOne.SetActive(true);
		}
	}

	void OnDestroy () {
        webCamTexture.Stop();
		MyNetworkServer.UnregisterHandler(64);
    }

	void SetupCamera () {
		webCamTexture = new WebCamTexture(WebCamTexture.devices[Setting.a.cameraIndex].name, 640, 480, 120);
        webCamTexture.Play();
	}

	void UpdatePreview(Mat frame, bool blackAndWhite = false)
    {
		if (webCamTexture.width < 100)
			return;
		if (tex == null)
			tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
		CvConvert.MatToTexture2D(frame, ref tex);
		cameraPreview.texture = tex;
    }
}
