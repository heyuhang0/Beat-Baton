using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainController : MonoBehaviour {
	public GameObject baton;
	public int serverPort;
	public Text ipTextInput, profileTextInput;
	public Text ipTextPH, profileTextPH;
	public Text connectButtonText;

	private Transform batonTrans;
	private NetworkClient client;

	enum NetworkState {Disconnected, Connecting, OnConnected, Connected};
	private NetworkState networkState = NetworkState.Disconnected;
	private float deltaAngle = 0;

	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Input.gyro.enabled = true;
		batonTrans = baton.GetComponent<Transform>();

		client = new NetworkClient();
		client.RegisterHandler(44, OnVibrationCommand);

		ipTextPH.GetComponent<Text>().text = Setting.a.ip;
		profileTextPH.GetComponent<Text>().text = Setting.a.name;
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion rawRotation = Input.gyro.attitude;
		batonTrans.rotation = rawRotation;
		batonTrans.Rotate(90, 0, 0, Space.World);
		batonTrans.Rotate(0, -deltaAngle, 0, Space.World);

		batonTrans.position = new Vector3(0, 0, 0);
		batonTrans.Translate(new Vector3(0, -batonTrans.localScale.y, 0), Space.Self);

		if (client.isConnected) {
			if (networkState == NetworkState.Connecting) {
				Debug.Log("Connected");
				networkState = NetworkState.Connected;
				client.SetMaxDelay(0);
			}

			UserMessage msg = new UserMessage();
			msg.orientation = batonTrans.rotation;
			msg.profile = Setting.a.name;

			client.Send(64, msg);
			Debug.Log("Sending");
		} else if (networkState == NetworkState.Connected) {
			Debug.Log("Disconnected, trying to reconnect...");
			client.Disconnect();
			Connect();
		}
	}
	void Connect() {
		client.Connect(Setting.a.ip, serverPort);
		networkState = NetworkState.Connecting;
		Debug.Log("Connecting...");
	}


	private void OnVibrationCommand(NetworkMessage netMsg) {
		Debug.Log("Vibrate...");
		Handheld.Vibrate();
	}

	public void onClickConnect() {
		if (ipTextInput.GetComponent<Text>().text != "")
			Setting.a.ip = ipTextInput.GetComponent<Text>().text;
		if (profileTextInput.GetComponent<Text>().text != "")
			Setting.a.name = profileTextInput.GetComponent<Text>().text;

		if (networkState == NetworkState.Disconnected) {
			Connect();
			deltaAngle = getBatonTrueAngleY() + deltaAngle - 270;
			Debug.Log(deltaAngle);
			SetButtonText("Disconnect");
		} else {
			client.Disconnect();
			networkState = NetworkState.Disconnected;
			SetButtonText("Connect");
		}
		Setting.SaveGameData();
	}

	void SetButtonText(string newText) {
		connectButtonText.GetComponent<Text>().text = newText;
	}

	float getBatonTrueAngleY() {
		float angle_x = batonTrans.transform.rotation.eulerAngles.x * Mathf.PI / 180;
        float angle_z = batonTrans.transform.rotation.eulerAngles.z * Mathf.PI / 180;

        float x = Mathf.Sin(angle_z);
        float y = Mathf.Cos(angle_z) * Mathf.Sin(angle_x);

        return Mathf.Atan2(y, x) * 180 / Mathf.PI + batonTrans.transform.rotation.eulerAngles.y;
	}
}

public class UserMessage : MessageBase
{	
	public string profile;
	public Quaternion orientation;
}
