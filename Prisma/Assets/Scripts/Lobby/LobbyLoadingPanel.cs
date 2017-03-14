using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyLoadingPanel : PromptWindow {

	/* UI Elements */
	public Text playerCounter;
	public Text waitingLbl;
	public Button startGame;

	/* Private variables */
	private int nbrOfPlayers;
	private int minNumberOfPlayers;


	public void Display(int minPlayers){

		nbrOfPlayers = 0;
		minNumberOfPlayers = minPlayers;
		playerCounter.text = nbrOfPlayers + "/" + minNumberOfPlayers + "...";

		gameObject.SetActive (true);

	}

	public void ButtonsInteractiable(bool isInteractiable) {



	}


	public void NumberOfPlayersChanged(int i){

		nbrOfPlayers += i;

		playerCounter.text = nbrOfPlayers + "/" + minNumberOfPlayers + "...";

	}
		
	public void CancelButtonClicked(){

		MyNetworkLobbyManager.singelton.CancelConnection ();

		this.gameObject.SetActive (false);

	}
}
