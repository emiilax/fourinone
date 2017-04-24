using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncScreenButton : MonoBehaviour {

	// Variable if button is pressed or not
	bool isEnabled;

	void Start() {
		isEnabled = false;
	}

	// Button opacity: True = 50%. False = 100%. 
	void OnMouseDown(){
		
		if (gameObject.tag == "SyncScreenButton") {

			//
			if (!isEnabled) {
				gameObject.GetComponent<Renderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				isEnabled = true;
			} else {
				gameObject.GetComponent<Renderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				isEnabled = false;
			}

		}
			
	}
}
