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

	private enum State {Normal, Waitting, Calibrating};
	private State state = State.Normal;

	public static string selectedProfile = "";

	void Start () {
		SetupCamera();
		frame = new Mat(webCamTexture.height, webCamTexture.width, MatType.CV_8UC4);

		RefreshProfilesList();

		NetworkServer.RegisterHandler(64, OnServerReceived);
        NetworkServer.Listen(serverPort);
	}
	
	void Update () {
		if (webCamTexture.didUpdateThisFrame && webCamTexture.isPlaying)
        {
            CamUpdate();
        }
	}

	void CamUpdate() {
		CvUtil.GetWebCamMat(webCamTexture, ref frame);
		Cv2.Flip(frame, frame, FlipMode.Y);
		switch (state) {
			case State.Normal:
			break;
			case State.Waitting:
				Cv2.Circle(frame, 320, 240, 30, new Scalar(255, 0, 0), 3);
				break;
			case State.Calibrating:

				Mat blurred = new Mat();
				Mat hsvMat = new Mat();

				Cv2.GaussianBlur(frame, blurred, new Size(5, 5), 0);
				Cv2.CvtColor(blurred, hsvMat, ColorConversionCodes.RGB2HSV);
				
				Vec3b rgb = blurred.Get<Vec3b>(240, 320);
				Vec3b hsv = hsvMat.Get<Vec3b>(240, 320);

				blurred.Release();
				hsvMat.Release();

				newProfile.color = new Color((float)rgb.Item0/255, (float)rgb.Item1/255, (float)rgb.Item2/255);
				int h = Mathf.Max(hsv.Item0 - 10, 0);
				int s = Mathf.Max(hsv.Item1 - 50, 0);
				int v = Mathf.Max(hsv.Item2 - 50, 0);
				newProfile.hsvLower = new Vector3(h, s, v);
				h = Mathf.Max(hsv.Item0 + 10, 0);
				s = Mathf.Max(hsv.Item1 + 50, 0);
				v = Mathf.Max(hsv.Item2 + 50, 0);
				newProfile.hsvUpper = new Vector3(h, s, v);
				
				state = State.Normal;
				Setting.a.batonProfiles.Add(newProfile);
				helpText2.SetActive(false);
				RefreshProfilesList();
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
			state = State.Calibrating;
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
		NetworkServer.Shutdown();
    }

	void SetupCamera () {
		webCamTexture = new WebCamTexture(WebCamTexture.devices[Setting.a.cameraIndex].name, 640, 480, 120);
        webCamTexture.Play();

        tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
	}

	void UpdatePreview(Mat frame, bool blackAndWhite = false)
    {
		CvConvert.MatToTexture2D(frame, ref tex);
		cameraPreview.texture = tex;
    }
}
