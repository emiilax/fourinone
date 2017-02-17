using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyNetworkLobbyManager : NetworkLobbyManager {

	// Menu with startbutton
	public RectTransform mainMenuPanel;

	// Menu with the different lobbies
	public RectTransform lobbySelectionPanel;

	// Displays amount of players connected
	public LobbyLoadingPanel loadingScreen;

	// Displays when connecting to a server
	public RectTransform connectingScreen;

	public string lobbyName = ""; 

	//public NetworkDiscovery discovery;



	protected RectTransform currentPanel;

	// Use this for initialization
	void Start () {

		StartMatchMaker ();
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

	public override void OnStartHost(){
		base.OnStartHost();



	}

	public void ShowLoadingScreen(Color color){
		
		loadingScreen.Display (color);

		this.matchMaker.ListMatches(0, 6, "", true, 0, 0, JoinOrCreateMatch);

	}

	public void JoinOrCreateMatch(bool success, string extendedInfo, List<MatchInfoSnapshot> matches){
	
		if (success) {

			if(matches.Count > 0){
				
				foreach (MatchInfoSnapshot m in matches) {

					if(lobbyName.Equals(m.name)) {

						Debug.Log ("Joined Game. Server: " + m.name);
						matchMaker.JoinMatch (m.networkId, "", "", "", 0, 0, OnMatchJoined);
						return;
					}

				}



			}

			this.matchMaker.CreateMatch(
				lobbyName,
				(uint)maxPlayers,
				true,
				"", "", "", 0, 0,
				OnMatchCreate);
			Debug.Log ("Match created. Servername: " + lobbyName);
		
		} else {
			
			Debug.LogError("Couldn't connect to match maker");

		}


	}

	public override void OnMatchCreate (bool success, string extendedInfo, MatchInfo matchInfo)
	{
		base.OnMatchCreate (success, extendedInfo, matchInfo);
		if(!success){
			Debug.LogError("Couldn't connect to match maker");
		}
	}



	public override void OnLobbyClientConnect(NetworkConnection conn){
		base.OnLobbyClientConnect (conn);

		Debug.Log ("OnLobbyClientConnect");

	}


	public override void OnLobbyServerPlayersReady(){
	
		base.OnLobbyServerPlayersReady ();

		Debug.Log ("All ready");

		GetComponent<Canvas>().enabled = false;
	
	}

	public override void OnLobbyClientSceneChanged (NetworkConnection conn){
		
		base.OnLobbyClientSceneChanged (conn);
		GetComponent<Canvas>().enabled = false;

	}

	public void StartGameButtonPressed(){
	
		ChangePanel (lobbySelectionPanel);

	}

	public void startGame(){
		
		GetComponent<Canvas>().enabled = false;

	}


	// Update is called once per frame
	void Update () {
		
	}
}
