using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public static TutorialManager instance;

	public GameObject tutorialButton;
	public GameObject tutorialPanel;

	public Sprite hideTutorialButton;
	public Sprite showTutorialButton;

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
		

	}

	public void HideTutorialButtonPressed () {

		if (tutorialPanel.activeSelf) {
			tutorialPanel.SetActive (false);
			tutorialButton.GetComponent<SpriteRenderer> ().sprite = showTutorialButton;
		} else {
			tutorialPanel.SetActive (true);
			tutorialButton.GetComponent<SpriteRenderer> ().sprite = hideTutorialButton;
		}
		
	}
}
