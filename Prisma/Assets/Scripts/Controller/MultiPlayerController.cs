using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerController : MonoBehaviour {

	// A reference to our game control script so we can access it statically
	public static MultiPlayerController instance;

	// The gamepanel that contains all the game objects
	public RectTransform gamePanel;

	// Pop-up menu when the player wins, also takes the player to the next level
	public RectTransform winPanel;

	// Integer for the maximum keys that has to be collected
	public int keyMaxCount;

	// Integer to store the number of keys collected
	private int keyCount;

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

		//winPanel.gameObject.SetActive (false);

		keyCount = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
