using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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

		gameMode = MyNetworkLobbyManager.singelton.gameMode;

		if (gameMode == "SinglePlayer") {

			singlePlayerPanel.SetActive (true);
			multiPlayerPanel.SetActive (false);

		} else if (gameMode == "MultiPlayer") {
			
			if (isServer) {
				multiPlayerPanel.SetActive (true);
			} else {
				clientPanel.SetActive (true);
				levelMenu.SetActive (false);
				multiPlayerPanel.SetActive (false);
			}

			singlePlayerPanel.SetActive (false);

			mpLevelList = getFirstChildren (mpLevels);
				
		} 

	}
		
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LevelSelectedSP(string sceneName){
		MyNetworkLobbyManager.singelton.ServerChangeScene (sceneName);
	}

	public void LevelSelected(GameObject level) {

		if (isServer) {
			RpcDeactivateLevels ();
			RpcToggleSelector ();
			if (!mpCommons.activeSelf) {
				RpcToggleMpCommons ();
			}
			Debug.Log (level.name);
			RpcChangeLevel(level.name);
			RpcTriggerChangeLevel ();
		}
	}
		
	//makes sure all levels are inactive before chosing a level
	//they have to be active prior due to not beince properly instanced otherwise
	[ClientRpc]
	private void RpcDeactivateLevels(){
		foreach (GameObject level in mpLevelList) {
			if (level.activeSelf) {
				level.SetActive (false);
			}
		}
	}


	//turns the levelselector view on or off
	[ClientRpc]
	public void RpcToggleSelector()
	{
		List<GameObject> l1 = getFirstChildren (gameObject);
		GameObject lvlselector = l1 [0];
		lvlselector.SetActive (!lvlselector.activeSelf);
	}

	//Toggles all common multiplayer objects
	[ClientRpc]
	private void RpcToggleMpCommons(){
		mpCommons.SetActive (!mpCommons.activeSelf);
	}

	//used because only some things can be sent ofer RPC calls.
	[ClientRpc]
	public void RpcChangeLevel(string nextLevel){
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
	[ClientRpc]
	public void RpcTriggerChangeLevel(){
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
