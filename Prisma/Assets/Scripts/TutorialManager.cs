using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public Text tutorialText;

	public GameObject tutorialPanel;

	// Use this for initialization
	IEnumerator Start () {

		tutorialText.canvasRenderer.SetAlpha (0.0f);

		FadeIn ();
		yield return new WaitForSeconds (2.5f);
		FadeOut ();
		yield return new WaitForSeconds (2.5f);

		tutorialPanel.SetActive (false);
	}

	void FadeIn () {
		
		tutorialText.CrossFadeAlpha (1.0f, 1.5f, false);

	}

	void FadeOut () {

		tutorialText.CrossFadeAlpha (0f, 2.5f, false);
		
	}

	public void SkipButtonPressed () {

		tutorialPanel.SetActive (false);
		
	}
}
