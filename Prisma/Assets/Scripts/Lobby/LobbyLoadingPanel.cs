using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyLoadingPanel : MonoBehaviour {

	public MyNetworkLobbyManager lobbyManager;
	public Text playerCounter;
	public Image backgroundColor;
	public Image blur;

	private int nbrOfPlayers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Display(Color color){
		nbrOfPlayers = 1;
		playerCounter.text = nbrOfPlayers + "/4...";
		backgroundColor.color = color;
		blur.color = new Color (0f, 0f, 0f, 0.7f);

		gameObject.SetActive (true);

	}
		


	public void incPlayers(){
		nbrOfPlayers += 1;

		if (nbrOfPlayers > 4) {
			lobbyManager.startGame ();
		}
		playerCounter.text = nbrOfPlayers + "/4...";

	}
}
