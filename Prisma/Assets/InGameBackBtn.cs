using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameBackBtn : MonoBehaviour {
	// Variable if button is pressed or not
	bool isEnabled;

	public Sprite ready;
	public Sprite notReady;
	public GameObject inGameMenu;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnChangeLevel(){
		gameObject.GetComponent<SpriteRenderer> ().sprite = notReady;
		isEnabled = false;
	}

	void OnMouseDown()
	{
		GUILog.Log ("pressed button");
		if (!isEnabled) {
			gameObject.GetComponent<SpriteRenderer> ().sprite = ready;
			inGameMenu.GetComponent<InGameManu>().SelectedExitBtn ();
			isEnabled = true;
		} else {
			gameObject.GetComponent<SpriteRenderer> ().sprite = notReady;
			inGameMenu.GetComponent<InGameManu>().DeselectedExitBtn ();
			isEnabled = false;
		}
	}
}
