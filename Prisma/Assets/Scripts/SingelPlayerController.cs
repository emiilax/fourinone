using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SingelPlayerController : MonoBehaviour {

	// A reference to our game control script so we can access it statically
	public static SingelPlayerController instance;

	// The gamepanel that contains all the game objects
	public RectTransform gamePanel;

	// Pop-up menu when the player wins, also takes the player to the next level
	public RectTransform winPanel;

	// Integer for the maximum keys that has to be collected
	public int keyMaxCount;

	// Integer to store the number of keys collected
	private int keyCount;

	void Awake()
	{
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

		winPanel.gameObject.SetActive (false);

		keyCount = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void keyHit(Collider2D key) {
		
		key.gameObject.SetActive (false);
		keyCount++;

		if (keyCount == keyMaxCount) {
			gamePanel.gameObject.SetActive (false);
			winPanel.gameObject.SetActive (true);
		}

	}

	public void NextButtonPressed() {

		switch (SceneManager.GetActiveScene().name) {
			case "SingelPlayerLevel1":
				SceneManager.LoadScene ("SingelPlayerLevel2");
				break;
			case "SingelPlayerLevel2":
				SceneManager.LoadScene ("SingelPlayerLevel3");
				break;
			case "SingelPlayerLevel3":
				SceneManager.LoadScene ("Lobby");
				break;
			}
	}
		
	// Action handler for back-button to last scene
	public void BackButtonPressed() {

		SceneManager.LoadScene ("Lobby");
		MyNetworkLobbyManager.singelton.gameObject.SetActive (true);

	}
}
