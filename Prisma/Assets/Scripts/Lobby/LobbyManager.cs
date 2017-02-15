using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyManager : NetworkLobbyManager {

	public RectTransform mainMenuPanel;

	public RectTransform lobbySelectionPanel;

	public LobbyLoadingPanel loadingScreen;

	public NetworkDiscovery discovery;


	protected RectTransform currentPanel;

	// Use this for initialization
	void Start () {

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

		discovery.StopBroadcast();
		Debug.Log("Start Host Broadcast....");
		discovery.broadcastData = networkPort.ToString();
		Debug.Log (networkPort.ToString ());
		discovery.StartAsServer();
		StartHost ();

	}

	public void StartGameButtonPressed(){
	
		ChangePanel (lobbySelectionPanel);

	}

	public void startGame(){
		Destroy (gameObject); 

		ServerChangeScene(playScene);
	}





	
	// Update is called once per frame
	void Update () {
		
	}
}
