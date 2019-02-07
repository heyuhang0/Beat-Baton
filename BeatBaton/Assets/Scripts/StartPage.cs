using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class StartPage : MonoBehaviour {
	public GameObject listContentItemTemplate;
	public Transform parentTransform;
	public GameObject contentView;
	// Use this for initialization
	void Start () {
		string[] ips = LocalIPAddresses();
		listContentItemTemplate.SetActive(false);

		contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 20 * ips.Length);
		foreach (string ip in ips) {
			GameObject newOne = Instantiate(listContentItemTemplate, parentTransform);
			Text text = newOne.GetComponentInChildren<Text>();
			text.text = ip;
			newOne.SetActive(true);
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnButtonStart () {
		SceneManager.LoadScene("LevelSelect");
	}

	public void onButtonOptions () {
		SceneManager.LoadScene("Options");
	}

	public void onButtonExit () {
		Application.Quit();
	}

	private string[] LocalIPAddresses()
     {
         IPHostEntry host;
         host = Dns.GetHostEntry(Dns.GetHostName());
		 ArrayList ipList = new ArrayList();
         foreach (IPAddress ip in host.AddressList)
         {
             if (ip.AddressFamily == AddressFamily.InterNetwork)
             {
				 ipList.Add(ip.ToString());
             }
         }
         return (string[])ipList.ToArray(typeof(string));
     }
}
