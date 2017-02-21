using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyLoadingPanel : MonoBehaviour {

	public MyNetworkLobbyManager lobbyManager;
	public Text playerCounter;
	public Image backgroundColor;
	public Image blur;
	public Text waitingLbl;
	public Button startGame;

	private int nbrOfPlayers;
	private int minNumberOfPlayers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Display(int players){
		nbrOfPlayers = 0;
		minNumberOfPlayers = players;
		playerCounter.text = nbrOfPlayers + "/" + minNumberOfPlayers + "...";
		//backgroundColor.color = color;
		blur.color = new Color (0f, 0f, 0f, 0.7f);

		gameObject.SetActive (true);

	}
		

	public void SetColor(Color color){
		backgroundColor.color = color;
	}


	public void IncPlayers(){
		nbrOfPlayers += 1;


		playerCounter.text = nbrOfPlayers + "/" + minNumberOfPlayers + "...";

	}
		
	public void CancelButtonClicked(){


		MyNetworkLobbyManager.singelton.CancelConnection ();
		this.gameObject.SetActive (false);

	}
}
