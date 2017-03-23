using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingelPlayer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* Action handler for back-button to last scene*/
	public void BackButtonPressed() {

		SceneManager.LoadScene ("Lobby");
		MyNetworkLobbyManager.singelton.gameObject.SetActive (true);

	}
}
