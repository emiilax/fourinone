﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using AssemblyCSharp;

public class MyNetworkLobbyManager : NetworkLobbyManager {

	static public MyNetworkLobbyManager singelton;

	public Transform background;

	// Menu with startbutton
	public RectTransform mainMenuPanel;

	// Menu with the different lobbies
	public RectTransform lobbySelectionPanel;

	// Displays amount of players connected
	public PromptLobbyLoadingPanel promptWaitingForPlayers;

	// Displays when connecting to a server
	public PromptConnectingToLobby promptConnectingToLobby;

	// Displays when something went wrong
	public PromptPlayerDisconnect promptPlayerDisconnect;

	public PromptLobbyFull promptLobbyFull;

	public string gameMode;

	private ulong currentMatchID; 

	private bool isHost = false;

	public string lobbyName = ""; 

	protected RectTransform currentPanel;

	private Vector3[] playerSpawnPositions;

	private MyNetworkLobbyManager(){}

	private int defaultMinPlayer;

	private int playerid;

	private NodeID nodeId;


	// Initialization of the singelton
	void Awake() {
		//If we don't currently have a game control...
		if (singelton == null)
			//...set this one to be it...
			singelton = this;
		//...otherwise...
		else if(singelton != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {

		playerSpawnPositions = new Vector3 [4];

		playerSpawnPositions[0] = (new Vector3 (-30.5f, 22f, -1f));
		playerSpawnPositions[1] = (new Vector3 (30.5f, 22f, -1f));
		playerSpawnPositions[2] = (new Vector3 (-30.5f, -22f, -1f));
		playerSpawnPositions[3] = (new Vector3 (30.5f, -22f, -1f));

		mainMenuPanel.gameObject.SetActive (true);
		lobbySelectionPanel.gameObject.SetActive (false);

		currentPanel = mainMenuPanel;

		gameMode = "MultiPlayer";

		defaultMinPlayer = minPlayers;

		DontDestroyOnLoad(gameObject);

	}

	public void ChangePanel(RectTransform newPanel){

		if (!background.gameObject.activeSelf) {
			background.gameObject.SetActive (true);
		}

		currentPanel.gameObject.SetActive (false);

		if (newPanel.Equals (lobbySelectionPanel)) {
			ButtonsInteractiable (true);
		}
			
		newPanel.gameObject.SetActive (true);
		currentPanel = newPanel;

	}
	public int GetPlayerId(){
		return (int)nodeId;
	}
	public void ShowPromptWindow(PromptWindow prompt, bool active){

		ButtonsInteractiable (!active);

		prompt.gameObject.SetActive (active);
	}


	public void ButtonsInteractiable(bool isInteractiable) {

		if(LobbySelectorManager.singleton)
			LobbySelectorManager.singleton.ButtonsInteractiable (isInteractiable);

	}


	public void ShowLoadingScreen(Color color){

		promptWaitingForPlayers.SetColor (color);
		promptConnectingToLobby.SetColor (color);

		ShowPromptWindow (promptConnectingToLobby, true);

		StartMatchMaker ();
		this.matchMaker.ListMatches(0, 6, "", true, 0, 0, JoinOrCreateMatch);

	}


	/* Function called when serching for games (ShowLoadingScreen). It first loops through all the active games, and 
	if the lobby you choosed is not active, then create lobby */ 
	public void JoinOrCreateMatch(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
	
		if (success) {

			if(matches.Count > 0){
				
				foreach (MatchInfoSnapshot m in matches) {

					if (m.currentSize == 0 || m.currentSize == minPlayers) {
						
						ShowPromptWindow (promptConnectingToLobby, false);
						ShowPromptWindow (promptLobbyFull, true);
						return;
					}

					if(lobbyName.Equals(m.name)) {

						Debug.Log ("Joined Game. Server: " + m.name);
						currentMatchID = (System.UInt64) m.networkId;
						isHost = false;
						matchMaker.JoinMatch (m.networkId, "", "", "", 0, 0, OnMatchJoin);
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
	public void OnDestroyMatch(bool success, string extendedInfo)
	{
		Debug.Log ("OnDestroyMatch");

		base.OnDestroyMatch(success, extendedInfo);


		Debug.Log ("Removing Matchmaeker");
		StopMatchMaker();
		Debug.Log ("Stop host");
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
		nodeId = matchInfo.nodeId;
	}

	public void OnMatchJoin (bool success, string extendedInfo, MatchInfo matchInfo)
	{
		OnMatchJoined (success, extendedInfo, matchInfo);
		nodeId = matchInfo.nodeId;
	}

	public override void OnServerSceneChanged (string sceneName)
	{
		base.OnServerSceneChanged (sceneName);
//		Debug.Log ("OnServerSceneChanged");
	}

	/* When you connect to the lobby, show loading screen with the amount of players */ 
	public override void OnLobbyClientConnect(NetworkConnection conn){

		base.OnLobbyClientConnect (conn);

		ShowPromptWindow (promptConnectingToLobby, false);

		promptWaitingForPlayers.SetUpPanel (minPlayers);

		ShowPromptWindow (promptWaitingForPlayers, true);


		//Debug.Log ("Client connected to lobby");

	}


	/* called when the scene is changed (lobby -> game). Have to disable current screen */
	public override void OnLobbyClientSceneChanged (NetworkConnection conn){

		base.OnLobbyClientSceneChanged (conn);
		HideAllPanels ();
		//gameObject.SetActive (false);

	}



	public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId){

		playerid = conn.connectionId;

		GameObject player = GameObject.Instantiate (gamePlayerPrefab, playerSpawnPositions[playerid], Quaternion.identity);

		return player;
	}
		

	/* When all players ready, start the game and disable current canvas */
	public override void OnLobbyServerPlayersReady(){
		// Debug.Log ("OnLobbyServerPlayerReady: Nmbr of startpos: " + this.startPositions.Count);
		Debug.Log ("All ready");



		base.OnLobbyServerPlayersReady ();
		//gameObject.SetActive (false);


	}
		

	/*
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer){
		Debug.Log ("Server loaded scene for player");

		nbr++;

		LevelSelectorController.instance.allLoaded = (nbr == minPlayers);
	

		return base.OnLobbyServerSceneLoadedForPlayer (lobbyPlayer, gamePlayer);
	}*/

	public override void OnLobbyServerDisconnect(NetworkConnection conn){
		base.OnLobbyServerDisconnect (conn);
		Debug.Log ("Server disconnect");

		//gameObject.SetActive (true);
		//Changed
		ChangePanel (lobbySelectionPanel);

		ShowPromptWindow (promptWaitingForPlayers, false);

		ShowPromptWindow (promptPlayerDisconnect, true);

		CancelConnection ();
	}

	public void QuitGame (){
		OnApplicationQuit ();
	}
		

	/* When app is closing and you were "host", destroy game */
	void OnApplicationQuit() {
		//gameObject.SetActive (true);
		if (isHost){
			if (matchMaker != null) {
				
				//StopHost();
				//isHost = false;
				matchMaker.DestroyMatch ((NetworkID)currentMatchID, 0, OnDestroyMatch);
				//StopMatchMaker();
			}
		}
		Debug.Log("Application ending after " + Time.time + " seconds");
	}


	/* When a client disconnects, get back to lobby */ 
	public override void OnClientDisconnect(NetworkConnection conn)
	{
		base.OnClientDisconnect(conn);

		ShowPromptWindow (promptWaitingForPlayers, false);

		ShowPromptWindow (promptPlayerDisconnect, true);

		//Changed
		ChangePanel (lobbySelectionPanel);
		//gameObject.SetActive (true);
	}


	/* Action handler for start screen */
	public void StartButtonPressed() {
	
		ChangePanel (lobbySelectionPanel);

	}

	/* Action handler for button to go to the level selector for single player */
	public void SingleGameButtonPressed() {

		minPlayers = 1;
		playerSpawnPositions[0] = (new Vector3 (-30.5f, 6.5f, 0f));
		gameMode = "SinglePlayer";

		//gameObject.SetActive (false);
		//Changed
		HideAllPanels();
		StartHost ();

	}


	/* Setup the values for 1-player game and starts it */
	public void StartSinglePlayer(string sceneName) {

		ServerChangeScene(sceneName);
		//SceneManager.LoadScene (sceneName);

	}

	public override void ServerChangeScene (string sceneName)
	{
		
		base.ServerChangeScene (sceneName);

		Debug.Log ("ServerChangeScene");
	}
		

	/* Set the value back to the original value for multiplayer */ 
	public void ResetFromSinglePlayer() {
		
		minPlayers = defaultMinPlayer;
		playerSpawnPositions[0] = (new Vector3 (-30.5f, 22f, 0f));
		gameMode = "MultiPlayer";


		//Changed
		//ChangePanel (mainMenuPanel);
		//gameObject.SetActive (true);
		StopHost();

		promptWaitingForPlayers.CancelButtonClicked ();
		ChangePanel (mainMenuPanel);

	}

	/* Action handler for back-button to previous panel*/
	/* The only panel to go back to is the mainMenuPanel at the moment */
	public void BackButtonPressed() {
		
		ChangePanel (mainMenuPanel);

	}

	/* Called when a player is added */ 

	public void NumberOfPlayersChanged(int i){
		promptWaitingForPlayers.NumberOfPlayersChanged (i);

	}

	private void HideAllPanels(){

		background.gameObject.SetActive (false);
		
		mainMenuPanel.gameObject.SetActive (false);

		lobbySelectionPanel.gameObject.SetActive (false);

		ShowPromptWindow (promptWaitingForPlayers, false);

		ShowPromptWindow (promptConnectingToLobby, false);

		ShowPromptWindow (promptPlayerDisconnect, false);

	}

		
}
