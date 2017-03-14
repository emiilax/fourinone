using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySelectorManager : MonoBehaviour {

	private Color color;
	public static LobbySelectorManager singleton;

	public Button btnGreen;
	public Button btnBlue;
	public Button btnOrange;
	public Button btnRed;



	void Start(){
		singleton = this;
	}

	public void ButtonsInteractiable(bool isInteractiable) {

		btnRed.interactable = isInteractiable;
		btnOrange.interactable = isInteractiable;
		btnBlue.interactable = isInteractiable;
		btnGreen.interactable = isInteractiable;

	}

	public void LobbyBtnPressed(){



		GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;


		if(SetLobbyColor(btn))
			MyNetworkLobbyManager.singelton.ShowLoadingScreen (color);


	}

	// Used for getting correct color
	protected bool SetLobbyColor(GameObject btn){
		
		if (btn.name.Equals ("btnOrangeLobby")) {

			color = StaticVariables.COLOR_ORANGE;
			MyNetworkLobbyManager.singelton.lobbyName = "orange";
			return true;

		} else if (btn.name.Equals ("btnBlueLobby")) {

			color = StaticVariables.COLOR_BLUE;
			MyNetworkLobbyManager.singelton.lobbyName = "blue";
			return true;

		} else if (btn.name.Equals ("btnGreenLobby")) {

			color = StaticVariables.COLOR_GREEN;
			MyNetworkLobbyManager.singelton.lobbyName = "green";
			return true;

		} else if (btn.name.Equals ("btnRedLobby")) {

			color = StaticVariables.COLOR_RED;
			MyNetworkLobbyManager.singelton.lobbyName = "red";
			return true;

		} 

		return false;
	}


}
