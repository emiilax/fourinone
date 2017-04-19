using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncScreenButton : MonoBehaviour {

	public int id;
	bool isEnabled;

	void Start() {
		isEnabled = false;
	}

	void OnMouseDown(){
		
		if (gameObject.tag == "SyncScreenButton") {

			if (!isEnabled) {
				gameObject.GetComponent<Renderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
				isEnabled = true;
			} else {
				gameObject.GetComponent<Renderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
				isEnabled = false;
			}

			var player = SyncScreenController.instance.players [id].GetComponent<AimShootingMultiTouch> ();
			player.CmdReadyBtnPressed (id);

		}
			
	}
}
