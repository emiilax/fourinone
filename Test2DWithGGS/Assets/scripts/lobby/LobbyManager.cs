using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;

namespace AssemblyCSharp
{
	public class LobbyManager : NetworkLobbyManager 
	{
		//static short MsgKicked = MsgType.Highest + 1;

		static public LobbyManager s_Singleton;

		public NetworkDiscovery discovery;

		public bool isDebug = true;
		public string lobbyName = "";
		private string externalIP = "";



		[Header("Unity UI Lobby")]
		[Tooltip("Time in second between all players ready & match start")]
		public float prematchCountdown = 5.0f;

		[Space]
		[Header("UI Reference")]
		public RectTransform mainMenuPanel;
		public RectTransform lobbyPanel;
		protected RectTransform currentPanel;


		public Text statusInfo;
		public Text hostInfo;

		//Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
		//of players, so that even client know how many player there is.
		[HideInInspector]
		public int _playerNumber = 0;


		//used to disconnect a client properly when exiting the matchmaker
		[HideInInspector]
		public bool _isMatchmaking = false;

		protected bool _disconnectServer = false;

		protected ulong _currentMatchID;

		void Start()
		{
			s_Singleton = this;

			currentPanel = mainMenuPanel;

			//GetComponent<Canvas>().enabled = true;
			GetComponent<Canvas>().enabled = true;

			DontDestroyOnLoad(gameObject);

			StartCoroutine(GetPublicIP ());

		}

		public override void OnStartHost()
		{
			base.OnStartHost();

			discovery.StopBroadcast ();
			string str = networkPort.ToString ();
			Debug.Log("Broadcast start.. " + str);
			discovery.broadcastData = networkPort.ToString();
			discovery.StartAsServer();

			ChangePanel (lobbyPanel);

			Debug.Log ("OnStartHost");
		}

		public void ChangePanel(RectTransform newPanel)
		{
			if (currentPanel != null)
			{
				currentPanel.gameObject.SetActive(false);
			}
				

			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(true);
			}

			currentPanel = newPanel;


		}

 
		public void LobbyButtonPressed () {
		
			GameObject btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

		    lobbyName = externalIP + GetButtonName(btn);
			if (IsLobbyEmpty()) {
				
				OnClickCreateMatchmakingGame ();

			} else {
				JoinGame ();
			}
			if (isDebug) 
				Debug.Log (lobbyName);
		}


		protected IEnumerator GetPublicIP(){
			WWW myExtIPWWW = new WWW ("http://checkip.dyndns.org");

			yield return myExtIPWWW;

			externalIP = myExtIPWWW.text;

			externalIP=externalIP.Substring(externalIP.IndexOf(":")+1);
			externalIP=externalIP.Substring(0,externalIP.IndexOf("<"));

			if (isDebug) 
				Debug.Log (externalIP);
		}
			




		protected string GetButtonName(GameObject btn){
			if (btn.name.Equals ("btnYellowLobby")) {

				return "yellow";

			} else if (btn.name.Equals ("btnBlueLobby")) {
				
				return "blue";

			} else if (btn.name.Equals ("btnGreenLobby")) {

				return "green";


			} else if (btn.name.Equals ("btnWhiteLobby")) {

				return "white";

			} 

			return "";
		}



		public void OnClickCreateMatchmakingGame()
		{

			StartMatchMaker();
			matchMaker.CreateMatch( 
				lobbyName,
				(uint) maxPlayers,
				true,
				"", "", "", 0, 0,
				OnMatchCreate);

			//lobbyManager.backDelegate = lobbyManager.StopHost;
			_isMatchmaking = true;

			//lobbyManager.DisplayIsConnecting();

			//lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
		}



		public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
		{
			base.OnMatchCreate(success, extendedInfo, matchInfo);
			_currentMatchID = (System.UInt64)matchInfo.networkId;
		}
		public void JoinGame(){
			//todo
		}
		public bool IsLobbyEmpty(){
			return true;
		}
	}
}

