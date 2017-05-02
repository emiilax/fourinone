using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LevelSelectorController : MonoBehaviour, IVoteListener {

	public static LevelSelectorController instance;

	bool selectedBackBtn = false;
	public Sprite unselectedBack, selectedBack;

	public GameObject singlePlayerBack;

	public GameObject multiPlayerBack;

	// The standard UI
	public GameObject levelMenu;

	// The SyncController
	public GameObject syncScreen;

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
	private string defaultText = "Välj bana:";
	private string voteFailText = "Alla måste välja samma bana!";
	public GameObject textObject;
	private Text text;
	// The current gamemode. SinglePlayer or MultiPlayer.
	private string gameMode;

	// connection ids and names of levels selected by players. Theses are only used by the server.
	private VotingSystem vote;


	public Texture2D defaultLevelThumbnail;

	public Texture2D[] levelThumbnail = new Texture2D[5];

	//currently playable levels in the chosen game mode
	private List<GameObject> currentLevels;


	//index in currentLevels of the level active now
	private int currentLevelIdx = -1;

	// State for the SelectionGrid
	private bool initialized = false;
	public int padding;
	private int lastId = -1;
	private GUIStyle style;
	private List<GUIContent> contents;
	private int selGridInt = -1;
	private bool showLevels = false;

	public LevelSelectorController singelton;



	void Awake() {
		if (instance == null)
			instance = this;
		else if(instance != this)
			//...destroy this one because it is a duplicate.
			Destroy (gameObject);
	}




	void Start(){
		instance = this;
		GUILog.Log ("onstartlocalplayer");

		text = textObject.GetComponent<Text> ();
		text.text = defaultText;
		vote = new VotingSystem (
			StaticVariables.LevelVoteMsg, 
			StaticVariables.LevelVoteCompletedMsg,
			StaticVariables.LevelVoteFailMsg,
			MyNetworkLobbyManager.singelton.GetPlayerId(),
			MyNetworkLobbyManager.singelton.minPlayers,
			MyNetworkLobbyManager.singleton.client,
			this
		);

		Debug.Log ("onstartclient");
		gameMode = MyNetworkLobbyManager.singelton.gameMode;

		spLevelList = FindLevels ("SP");
		mpLevelList = FindLevels ("MP");
		if (gameMode == "MultiPlayer") {

			Debug.Log ("Multiplayer");
			currentLevels = mpLevelList;
			contents = GetContents (mpLevelList);

			DeactivateLevels ();
			//gameObject.SetActive (false);
			GUILog.Log ("gameobject active: " + gameObject.activeSelf);

		}

		if(gameMode == "SinglePlayer"){
			currentLevels = spLevelList;
			contents = GetContents (spLevelList);
			//gameObject.SetActive (false);
		}

	}

	public void StartLevelSelector(){
		
		GUILog.Log ("player id " + MyNetworkLobbyManager.singelton.GetPlayerId());

		style = new GUIStyle ("button");
		if (gameMode == "SinglePlayer") {
			
			currentLevels = spLevelList;

			contents = GetContents (spLevelList);

			Debug.Log ("contents: " + contents.Count);
			//singlePlayerPanel.SetActive (true);
			//multiPlayerPanel.SetActive (false);
			syncScreen.SetActive (false);

		} else if (gameMode == "MultiPlayer") {
			currentLevels = mpLevelList;
			contents = GetContents (mpLevelList);

		} 
	}



	IEnumerator Delay(){
		yield return new WaitForSeconds (3.5f);
	}


	void OnGUI() {


		if (showLevels) {
			if (!initialized) {
				style = new GUIStyle (GUI.skin.button);
				style.margin = new RectOffset (48, 48, 48, 48);
				style.imagePosition = ImagePosition.ImageAbove;
				style.fontSize = 32;
			}

			int paddingleft = 200;
			int paddingtop = 300;


			int rows = (contents.ToArray ().Length / 3) + 1;

			selGridInt = GUI.SelectionGrid (new Rect (paddingleft, paddingtop, 2048 - paddingleft*2, 390*rows), selGridInt, contents.ToArray (), 3, style);
			if (lastId != selGridInt) {
				LevelSelected (selGridInt);
				lastId = selGridInt;

			}
		}
	}

	public void ShowLevelGrid(){
		showLevels = true;
	}

	List<GUIContent> GetContents(List<GameObject> levels){
		List<GUIContent> contents = new List<GUIContent> ();
		int i = 0;
		foreach(GameObject level in levels){
			
			//contents.Add (new GUIContent(level.name, defaultLevelThumbnail));
			contents.Add (new GUIContent(levelThumbnail[i]));
			i++;
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
		text.text = voteFailText;
	}

	public GameObject SetNextLevel(){
		if (currentLevels == null || currentLevels [currentLevelIdx + 1] == null) {
			return null;
		} else {
			currentLevelIdx++;
			//currentLevel = currentLevels [currentLevelIdx];
			ChangeLevel (currentLevels[currentLevelIdx]);
			TriggerChangeLevel ();
			return currentLevels [currentLevelIdx];
		}

	}

	public bool HasNextLevel(){
		return currentLevelIdx + 1 < currentLevels.Count;
	}

	public void OnVoteComplete(string levelIdx){
		GUILog.Log ("levelIdx=" + levelIdx);
		currentLevelIdx = int.Parse( levelIdx );
		selGridInt = -1;
		lastId = selGridInt;
		showLevels = false;
		GUILog.Log ("launch level " + levelIdx);
		DeactivateLevels ();
		ToggleSelector ();
		if (!mpCommons.activeSelf) {
			ToggleMpCommons ();
		}
		//Debug.Log (level);
		ChangeLevel (currentLevels[currentLevelIdx]);
		TriggerChangeLevel ();
		text.text = defaultText;

		if (gameMode == "SinglePlayer") {
			singlePlayerBack.SetActive (true);
			multiPlayerBack.SetActive (false);
		} else {
			multiPlayerBack.SetActive (true);
			singlePlayerBack.SetActive (false);
		}


	}


	public void LevelSelected(int levelIdx) {
		vote.CastVote (levelIdx.ToString());
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
		//List<GameObject> l1 = getFirstChildren (gameObject);
		//GameObject lvlselector = l1 [0];
		//lvlselector.SetActive (!lvlselector.activeSelf);

	//	GUILog.Log ("Toggle selector, set: " + !gameObject.activeSelf);
		gameObject.SetActive (!gameObject.activeSelf);

	//	GUILog.Log ("Toggle selector(after), set: " + gameObject.activeSelf);
	}

	//Toggles all common multiplayer objects

	private void ToggleMpCommons(){
		mpCommons.SetActive (!mpCommons.activeSelf);
	}

	//used because only some things can be sent ofer RPC calls.

	public void ChangeLevel(string nextLevel){
		ChangeLevel(currentLevels.Find(obj => obj.name == nextLevel));
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
	public void SinglePlayerBackButtonPressed(GameObject btn) {
		// Resets the default values for multiplayer and change back to lobby scene.
		MyNetworkLobbyManager.singelton.ResetFromSinglePlayer ();


	}

	// Action handler for back-button for multiPlayer to last scene.
	public void MultiPLayerBackButtonPressed(GameObject btn) {
		GUILog.Log ("back button 1");
		MyNetworkLobbyManager.singelton.ResetFromMultiPlayer ();
		GUILog.Log ("back button 2");
		/*
		if (!selectedBack) {
			vote.CastVote ("back");
			btn.GetComponent<Image> ().sprite = selectedBack;
		} else {
			vote.CastVote ("none");
			btn.GetComponent<Image> ().sprite = selectedBack;
		}
		*/
		//MyNetworkLobbyManager.singelton.ResetFromMultiPlayer ();
	}

	public void BackButtonPressed(GameObject btn) {
		GUILog.Log ("back button 0");
		if (gameMode.Equals("SinglePlayer")) {
			SinglePlayerBackButtonPressed (btn);
		} else {
			MultiPLayerBackButtonPressed (btn);
		}
	}

}
