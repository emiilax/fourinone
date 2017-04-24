using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public GameObject tutorialButton;
	public GameObject tutorialPanel;

	public Sprite hideTutorialButton;
	public Sprite showTutorialButton;

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
