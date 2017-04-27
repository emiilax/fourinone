using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PromptLobbyLoadingPanel : PromptWindow {

	/* UI Elements */
	public Text playerCounter;
	public Text waitingLbl;
	public Button startGame;

	/* Private variables */
	private int nbrOfPlayers;
	private int minNumberOfPlayers;


	public void SetUpPanel(int minPlayers){

		nbrOfPlayers = 0;
		minNumberOfPlayers = minPlayers;
		playerCounter.text = nbrOfPlayers + "/" + minNumberOfPlayers + "...";

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
