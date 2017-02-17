using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySelectorManager : MonoBehaviour {

	public MyNetworkLobbyManager lobbyManager;

	private Color color;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void LobbyBtnPressed(){



		GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;


		if(SetLobbyColor(btn))
			lobbyManager.ShowLoadingScreen (color);


	}

	// Used for getting correct color
	protected bool SetLobbyColor(GameObject btn){
		
		if (btn.name.Equals ("btnOrangeLobby")) {

			color = StaticVariables.COLOR_ORANGE;
			lobbyManager.lobbyName = "orange";
			return true;

		} else if (btn.name.Equals ("btnBlueLobby")) {

			color = StaticVariables.COLOR_BLUE;
			lobbyManager.lobbyName = "blue";
			return true;

		} else if (btn.name.Equals ("btnGreenLobby")) {

			color = StaticVariables.COLOR_GREEN;
			lobbyManager.lobbyName = "green";
			return true;

		} else if (btn.name.Equals ("btnRedLobby")) {

			color = StaticVariables.COLOR_RED;
			lobbyManager.lobbyName = "red";
			return true;

		} 

		return false;
	}


}
