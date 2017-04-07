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
		currentScene = "2Plvl1";
		nextScene = "2Plvl2";
	}
		
	void Update ()
	{

		if (!isServer)
			return;


		// If the player has run out of health...
		if(GameController.instance.GameFinished())
		{

			if (isServer) {
			//	MyNetworkLobbyManager.singelton.ServerChangeScene (nextScene);
			}
			//Debug.Log ("in here");
			anim.SetTrigger ("LevelCompleteHost");

			RpcShowAnimation ();

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
		
		//anim.gameObject.SetActive (false);
		RpcSendMessage("ButtonBackToLobbypressed");

		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		MyNetworkLobbyManager.singelton.CancelConnection ();
	
	}

	public void ButtonNextLevel(){
		if (!isServer)
			return;

		RpcSendMessage("ButtonNextLevelPressed");

		RpcSendMessage (MyNetworkLobbyManager.singelton.ToString());
		//MyNetworkLobbyManager.singelton.gameObject.SetActive (true);
		anim.gameObject.SetActive (false);
		MyNetworkLobbyManager.singelton.ServerChangeScene (nextScene);
		//MyNetworkLobbyManager.singelton.gameObject.SetActive (false);
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


