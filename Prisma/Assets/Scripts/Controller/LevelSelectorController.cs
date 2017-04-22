using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LevelSelectorController : NetworkBehaviour, IVoteListener {

	public static LevelSelectorController instance;

	// The standard UI
	public GameObject levelMenu;


	//List of all the multiplayer levels
	private List<GameObject> mpLevelList;
	private List<GameObject> spLevelList;

	//the current active level
	public GameObject currentLevel;

	//common multiplayer objects
	public GameObject mpCommons;

	// The different panels of the different game modes.
	public GameObject singlePlayerPanel;
	public GameObject multiPlayerPanel;

	//The text objects for displaying text at the top of the level selector view
	public GameObject text;

	// The current gamemode. SinglePlayer or MultiPlayer.
	private string gameMode;

	// connection ids and names of levels selected by players. Theses are only used by the server.
	private VotingSystem vote;


	public Texture2D defaultLevelThumbnail;
	// State for the SelectionGrid
	private bool initialized = false;
	public int padding;
	private int lastId = -1;
	private GUIStyle style;
	private List<GUIContent> contents;
	private int selGridInt = -1;
	private List<GameObject> guiLevels;
	private bool showLevels;

	void Awake() {
		if (instance == null)
			instance = this;
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		showLevels = true;
		vote = new VotingSystem (
			StaticVariables.LevelVoteMsg, 
			StaticVariables.LevelVoteCompletedMsg,
			StaticVariables.LevelVoteFailMsg,
			MyNetworkLobbyManager.singelton.GetPlayerId(),
			MyNetworkLobbyManager.singleton.client,
			this
		);

		if (isServer) {
			vote.setupServer (MyNetworkLobbyManager.singleton.numPlayers);
		}

		GUILog.Log ("player id " + MyNetworkLobbyManager.singelton.GetPlayerId());
		gameMode = MyNetworkLobbyManager.singelton.gameMode;



		mpLevelList = FindLevels ("MP");
		spLevelList = FindLevels ("SP");

		style = new GUIStyle ("button");
		if (gameMode == "SinglePlayer") {
			guiLevels = spLevelList;
			contents = GetContents (spLevelList);
			singlePlayerPanel.SetActive (true);
			multiPlayerPanel.SetActive (false);

		} else if (gameMode == "MultiPlayer") {
			guiLevels = mpLevelList;
			contents = GetContents (mpLevelList);
			multiPlayerPanel.SetActive (true);
			singlePlayerPanel.SetActive (false);

		} 



	}

	void OnGUI() {
		if (showLevels) {
			if (!initialized) {
				style = new GUIStyle (GUI.skin.button);
				style.margin = new RectOffset (48, 48, 48, 48);
				style.imagePosition = ImagePosition.ImageAbove;
				style.fontSize = 32;
			}
			selGridInt = GUI.SelectionGrid (new Rect (padding, padding, 1024 - padding * 2, 768 - padding * 2), selGridInt, contents.ToArray (), guiLevels.Count / 2, style);
			if (lastId != selGridInt) {
				LevelSelected (guiLevels [selGridInt]);
				lastId = selGridInt;

			}
		}
	}

	public void ShowLevelGrid(){
		showLevels = true;
	}

	List<GUIContent> GetContents(List<GameObject> levels){
		List<GUIContent> contents = new List<GUIContent> ();
		foreach(GameObject level in levels){
			
			contents.Add (new GUIContent(level.name, defaultLevelThumbnail));
		}
		return contents;
	}

	List<GameObject> FindLevels(string type){
		GameObject levels = GameObject.Find (type + "Levels");
		List<GameObject> levelObjs = new List<GameObject>();
		foreach(Transform trans in levels.transform)
		{
			GameObject level = trans.gameObject;
			levelObjs.Add (level);
		}
		return levelObjs;

	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public void ServerVoteComplete (string winner){
		//do nothing
	}
	public void OnVoteFail(){
		
	}

	public void OnVoteComplete(string level){
		selGridInt = -1;
		lastId = selGridInt;
		showLevels = false;
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
		vote.CastVote (level.name);
	}


	//makes sure all levels are inactive before chosing a level
	//they have to be active prior due to not beince properly instanced otherwise

	public void DeactivateLevels(){
		foreach (GameObject level in mpLevelList) {
			if (level.activeSelf) {
				level.SetActive (false);
			}
		}
		foreach (GameObject level in spLevelList) {
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
		ChangeLevel(guiLevels.Find(obj => obj.name == nextLevel));
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
