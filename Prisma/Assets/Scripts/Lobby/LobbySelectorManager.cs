using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySelectorManager : MonoBehaviour {

	public LobbyManager lobbyManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void LobbyBtnPressed(){



		GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

		Color lobbyColor = GetLobbyPressed(btn);

		if(lobbyColor != Color.white)
			lobbyManager.ShowLoadingScreen (lobbyColor);





	}

	// Used for getting correct color
	protected Color GetLobbyPressed(GameObject btn){
		
		if (btn.name.Equals ("btnOrangeLobby")) {

			return StaticVariables.COLOR_ORANGE;

		} else if (btn.name.Equals ("btnBlueLobby")) {

			return StaticVariables.COLOR_BLUE;

		} else if (btn.name.Equals ("btnGreenLobby")) {

			return StaticVariables.COLOR_GREEN;


		} else if (btn.name.Equals ("btnRedLobby")) {

			return StaticVariables.COLOR_RED;

		} 

		return Color.white;
	}


}
