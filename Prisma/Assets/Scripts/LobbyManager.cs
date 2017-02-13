using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviour {

	public RectTransform mainMenuPanel;

	public RectTransform lobbySelectionPanel;

	public LobbyLoadingPanel loadingScreen;


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

	public void ShowLoadingScreen(Color color){

		loadingScreen.Display (color);

	}

	public void StartGameButtonPressed(){
	
		ChangePanel (lobbySelectionPanel);

	}




	
	// Update is called once per frame
	void Update () {
		
	}
}
