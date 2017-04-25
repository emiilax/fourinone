using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncScreenButton : MonoBehaviour {

	// Variable if button is pressed or not
	bool isEnabled;

	public Sprite ready;
	public Sprite notReady;

	void Start() {
		isEnabled = false;
	}

	// Button opacity: True = 50%. False = 100%. 
	void OnMouseDown(){

		if (gameObject.tag == "SyncScreenButton") {

			//
			if (!isEnabled) {
				gameObject.GetComponent<SpriteRenderer> ().sprite = ready;
				isEnabled = true;
			} else {
				gameObject.GetComponent<SpriteRenderer> ().sprite = notReady;
				isEnabled = false;
			}

		}

	}
}
