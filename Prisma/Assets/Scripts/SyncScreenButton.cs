using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncScreenButton : MonoBehaviour {

	void OnMouseDown(){
		
		if (gameObject.tag == "SyncScreenButton") {
			
			SyncScreenController.instance.ReadyBtnPressed(gameObject);

		}
			
	}
}
