using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

public class MyNetworkLobbyManager : NetworkLobbyManager {

	static public MyNetworkLobbyManager singelton;

	// Menu with startbutton
	public RectTransform mainMenuPanel;

	// Menu with the different lobbies
	public RectTransform lobbySelectionPanel;

	// Displays amount of players connected
	public LobbyLoadingPanel waitingForPlayersScreen;

	// Displays when connecting to a server
	public ConnectingPanel connectingScreen;

	private ulong currentMatchID; 

	private bool isHost = false;

	public string lobbyName = ""; 

	//public NetworkDiscovery discovery;



	protected RectTransform currentPanel;

	// Use this for initialization
	void Start () {

		singelton = this;

		mainMenuPanel.gameObject.SetActive (true);
		lobbySelectionPanel.gameObject.SetActive (false);

		currentPanel = mainMenuPanel;


		GetComponent<Canvas>().enabled = true;


		DontDestroyOnLoad(gameObject);


	}

	public void ChangePanel(RectTransform newPanel){

		currentPanel.gameObject.SetActive (false);

		newPanel.gameObject.SetActive (true);

		currentPanel = newPanel;



	}




	public void ShowLoadingScreen(Color color){

		connectingScreen.gameObject.SetActive (true);
		waitingForPlayersScreen.SetColor (color);
		StartMatchMaker ();
		this.matchMaker.ListMatches(0, 6, "", true, 0, 0, JoinOrCreateMatch);

	}


	/* Function called when serching for games (ShowLoadingScreen). It first loops through all the active games, and 
	if the lobby you choosed is not active, then create lobby */ 
	public void JoinOrCreateMatch(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
	
		if (success) {

			if(matches.Count > 0){
				
				foreach (MatchInfoSnapshot m in matches) {

					if(lobbyName.Equals(m.name)) {

						Debug.Log ("Joined Game. Server: " + m.name);
						currentMatchID = (System.UInt64) m.networkId;
						isHost = false;
						matchMaker.JoinMatch (m.networkId, "", "", "", 0, 0, OnMatchJoined);
						return;

					}// end if equals

				}// end if count

			}// end if success

			matchMaker.CreateMatch(
				lobbyName ,
				(uint)maxPlayers,
				true,
				"", "", "", 0, 0,
				OnMatchCreate);

			isHost = true;
			Debug.Log ("Match created. Servername: " + lobbyName);

		
		} else {
			
			Debug.LogError("Couldn't connect to match maker");

		}


	}

	/* Fucktion used when you want to cancel the connection. Doing different actions
	depending on "host" och client */
	public void CancelConnection(){

		if (isHost) {
			Debug.Log ("Host: Destroy match");
			matchMaker.DestroyMatch((NetworkID)currentMatchID, 0, OnDestroyMatch);
		} else {
			Debug.Log ("Client: Stop matchmaker");
			StopClient();
			StopMatchMaker();
		}

		ChangePanel(lobbySelectionPanel);

	}

	/* Called when you want to destroy ther current matchmaker game is */
	public override void OnDestroyMatch(bool success, string extendedInfo)
	{
		base.OnDestroyMatch(success, extendedInfo);
		StopMatchMaker();
		StopHost();
		isHost = false;
	}


	/* Called when a match is created on mathMaker. Overrides this since we wan to save current match id.*/
	public override void OnMatchCreate (bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		currentMatchID = (System.UInt64)matchInfo.networkId;
		if(!success){
			Debug.LogError("Couldn't connect to match maker");
		}
	}


	/* When you connect to the lobby, show loading screen with the amount of players */ 
	public override void OnLobbyClientConnect(NetworkConnection conn){
		base.OnLobbyClientConnect (conn);
		connectingScreen.gameObject.SetActive (false);
		waitingForPlayersScreen.Display (minPlayers);

		Debug.Log ("Client connected to lobby");

	}

	/* called when the scene is changed (lobby -> game). Have to disable current screen */
	public override void OnLobbyClientSceneChanged (NetworkConnection conn){

		base.OnLobbyClientSceneChanged (conn);
		GetComponent<Canvas>().enabled = false;

	}

	/* When all players ready, start the game and disable current canvas */
	public override void OnLobbyServerPlayersReady(){

		Debug.Log ("All ready");
		base.OnLobbyServerPlayersReady ();

		GetComponent<Canvas>().enabled = false;

	}


	public override void OnLobbyServerDisconnect(NetworkConnection conn){
		base.OnLobbyServerDisconnect (conn);
		Debug.Log ("Server disconnect");

		ChangePanel (lobbySelectionPanel);
		CancelConnection ();
		DestroyObject (gameObject);
		//ServerReturnToLobby ();


	}



	/* When app is closing and you were "host", destroy game */
	void OnApplicationQuit() {
		if (isHost){
			if (matchMaker)
				Debug.Log ("Host: Destroy match");
				matchMaker.DestroyMatch((NetworkID)currentMatchID, 0, OnDestroyMatch);
		}
		Debug.Log("Application ending after " + Time.time + " seconds");
	}



	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);
		waitingForPlayersScreen.gameObject.SetActive (false);

		GetComponent<Canvas>().enabled = true;
	}


	/* Action handler for start screen */
	public void StartButtonPressed(){
	
		ChangePanel (lobbySelectionPanel);

	}

	/* Called when a player is added */ 

	public void NumberOfPlayersChanged(int i){
		waitingForPlayersScreen.NumberOfPlayersChanged (i);

	}
		
}
