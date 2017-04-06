using System;
using UnityEngine.Networking;
using UnityEngine;

public class LevelCompleteManager : NetworkBehaviour {

	public float restartDelay = 5f;         // Time to wait before restarting the level

	public string currentScene;
	public string nextScene;
	public GameObject game;


	Animator anim;                          // Reference to the animator component.
	float restartTimer;                     // Timer to count up to restarting the level


	void Awake ()
	{		
		// Set up the reference.
		anim = GetComponent <Animator> ();
	}
		
	void Update ()
	{

		if (!isServer)
			return;


		// If the player has run out of health...
		if(GameController.instance.GameFinished())
		{
			//Debug.Log ("in here");
			anim.SetTrigger ("LevelCompleteHost");

			RpcShowAnimation ();
			//game.SetActive (false);
		}
	}

	[ClientRpc]
	void RpcShowAnimation(){
		// ... tell the animator the game is over.
		if (isServer)
			return;
		
		anim.SetTrigger ("LevelCompleteClient");
		// .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
	}


	public void ButtonBackToLobby(){

		if (!isServer)
			return;


		RpcSendMessage("ButtonBackToLobbypressed");

		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.CancelConnection ();
	
	}

	public void ButtonNextLevel(){
		if (!isServer)
			return;

		RpcSendMessage("ButtonNextLevelPressed");


		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.ServerChangeScene (nextScene);
	}


	public void ButtonRestartLevel(){
		if (!isServer)
			return;
		
		RpcSendMessage("ButtonRestartLevelpressed");
		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.ServerChangeScene (currentScene);
	}


	[ClientRpc]
	void RpcSendMessage(string str){
	
		Debug.Log (str);

	}
}


