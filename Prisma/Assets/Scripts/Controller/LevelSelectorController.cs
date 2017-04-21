using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LevelSelectorController : NetworkBehaviour {

	public static LevelSelectorController instance;

	// The standard UI
	public GameObject levelMenu;

	//The gameobject containing multiplayer levels,
	//to be used to populate mpLevelList
	public GameObject mpLevels;

	//List of all the multiplayer levels
	public List<GameObject> mpLevelList;

	//the current active level
	public GameObject currentLevel;

	//common multiplayer objects
	public GameObject mpCommons;

	// The different panels of the different game modes.
	public GameObject singlePlayerPanel;
	public GameObject multiPlayerPanel;

	// The panel for the clients that are not the server
	public GameObject clientPanel;

	// The current gamemode. SinglePlayer or MultiPlayer.
	public string gameMode;

	// number of players in the current game
	private int numPlayers;

	// Our connectionId should not change during game
	private int connId;

	NetworkClient client;

	// id of the level vote network message
	const short IdMsg = 2000;

	const short LevelVoteMsg = 1000;
	const short LevelVoteCompleteMsg = 1001;
	// connection ids and names of levels selected by players. Theses are only used by the server.
	private Dictionary<int, string> votes;
	//private List<int> ids;
	//private List<string> votedLevels;

	void Awake() {
		//If we don't currently have a game control...
		if (instance == null)
			//...set this one to be it...
			instance = this;
		//...otherwise...
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {

		if (isServer) {
			//NetworkServer.RegisterHandler(MyBeginMsg, OnServerReadyToBeginMessage);
			if (gameMode == "MultiPlayer") {
				votes = new Dictionary<int, string> ();
				numPlayers = MyNetworkLobbyManager.singleton.numPlayers;


			}
		}
		gameMode = MyNetworkLobbyManager.singelton.gameMode;

		if (gameMode == "SinglePlayer") {

			singlePlayerPanel.SetActive (true);
			multiPlayerPanel.SetActive (false);

		} else if (gameMode == "MultiPlayer") {
			multiPlayerPanel.SetActive (true);

			singlePlayerPanel.SetActive (false);





			//if (!isServer) {
			NetworkServer.RegisterHandler(LevelVoteMsg, OnLevelVoteCast);
			NetworkServer.RegisterHandler(IdMsg, OnRequestId);
			client = MyNetworkLobbyManager.singelton.client;
			//connId = client.connection.connectionId;
			client.RegisterHandler (LevelVoteCompleteMsg, OnVoteComplete);
			client.RegisterHandler (IdMsg, OnRecieveId);
			votes = new Dictionary<int, string> ();
			numPlayers = MyNetworkLobbyManager.singleton.numPlayers;
			client.Send(IdMsg, new IntegerMessage(0));

		} 

		mpLevelList = getFirstChildren (mpLevels);


	}

	public void OnRequestId(NetworkMessage netMsg){
		
		NetworkServer.SendToClient(netMsg.conn.connectionId, IdMsg, new IntegerMessage(netMsg.conn.connectionId));
	}

	public void OnRecieveId(NetworkMessage netMsg){
		int id = netMsg.ReadMessage<IntegerMessage> ().value;
		GUILog.Log ("recieved id " + id.ToString());
		connId = id;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void SendVote(string level){
		//client.Send ();
		client.Send(LevelVoteMsg, new StringMessage(connId.ToString() + " " + level));
	}

	public void OnLevelVoteCast(NetworkMessage netMsg){
		
		string vote = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log ("recieved vote " + vote);
		var idAndLevel = vote.Split ();
		int id = int.Parse(idAndLevel[0]);
		string level = idAndLevel [1];
		votes [id] = level;
		if (votes.Count == numPlayers) {
			string firstVote = null;
			bool unanimous = true;
			foreach(KeyValuePair<int, string> entry in votes)
			{
				if (firstVote == null) {
					firstVote = entry.Value;
				} else {
					if (firstVote != entry.Value) {
						unanimous = false;
						break;
					} 
				}
				// do something with entry.Value or entry.Key
			}
			if (unanimous) {
				NetworkServer.SendToAll (LevelVoteCompleteMsg, new StringMessage(firstVote));

			}
		}

	}

	public void OnVoteComplete(NetworkMessage netMsg){
		string level = netMsg.ReadMessage<StringMessage>().value;
		GUILog.Log ("launch level " + level);
		DeactivateLevels ();
		ToggleSelector ();
		if (!mpCommons.activeSelf) {
			ToggleMpCommons ();
		}
		Debug.Log (level);
		ChangeLevel (level);
		TriggerChangeLevel ();

	}

	public void LevelSelected(GameObject level) {
		if (gameMode == "SinglePlayer") {
			if (isServer) {
				DeactivateLevels ();
				ToggleSelector ();
				if (!mpCommons.activeSelf) {
					ToggleMpCommons ();
				}
				Debug.Log (level.name);
				ChangeLevel (level.name);
				TriggerChangeLevel ();
			}
		} else if (gameMode == "MultiPlayer") {
			GUILog.Log ("selected level");
			SendVote (level.name);
			//CmdCastVote (MyNetworkLobbyManager.singelton.client.connection.connectionId, level.name);

		}

	}


	//makes sure all levels are inactive before chosing a level
	//they have to be active prior due to not beince properly instanced otherwise

	public void DeactivateLevels(){
		foreach (GameObject level in mpLevelList) {
			if (level.activeSelf) {
				level.SetActive (false);
			}
		}
	}


	//turns the levelselector view on or off

	public void ToggleSelector()
	{
		List<GameObject> l1 = getFirstChildren (gameObject);
		GameObject lvlselector = l1 [0];
		lvlselector.SetActive (!lvlselector.activeSelf);
	}

	//Toggles all common multiplayer objects

	private void ToggleMpCommons(){
		mpCommons.SetActive (!mpCommons.activeSelf);
	}

	//used because only some things can be sent ofer RPC calls.

	public void ChangeLevel(string nextLevel){
		ChangeLevel(mpLevels.transform.Find(nextLevel).gameObject);
	}
		
	//changes the current level
	public void ChangeLevel(GameObject nextLevel){
		if(currentLevel != null){
			currentLevel.gameObject.SetActive (false);
		}
		nextLevel.gameObject.SetActive (true);
		currentLevel = nextLevel;
	}
		

	//triggers OnChangeLevel message, required for messages to be able to reach inactive components

	public void TriggerChangeLevel(){
		Transform[] allGameObjects = transform.parent.GetComponentsInChildren<Transform> (true);
		foreach (Transform tf in allGameObjects) {
			tf.gameObject.SendMessage ("OnChangeLevel",null,SendMessageOptions.DontRequireReceiver);
		}
	}

	//returns a list of all the first level children in a gameobject
	private List<GameObject> getFirstChildren(GameObject gameObj){
		List<GameObject> list = new List<GameObject> ();
		//gets all components
		Transform[] allcomponents = gameObj.GetComponentsInChildren<Transform> (true);
		foreach (Transform tf in allcomponents) {
			if (tf.gameObject.Equals (gameObj)) { //top level object do nothing

			} else if (tf.parent.gameObject.Equals (gameObj)) {
				list.Add (tf.gameObject);
				Debug.Log (tf.gameObject.name);
			}
		}
		return list;
	}


				
	// Action handler for back-button for singlePlayer to last scene.
	public void SinglePlayerBackButtonPressed() {
		// Resets the default values for multiplayer and change back to lobby scene.
		MyNetworkLobbyManager.singelton.ResetFromSinglePlayer ();


	}

	// Action handler for back-button for multiPlayer to last scene.
	public void MultiPLayerBackButtonPressed() {

	}


}
