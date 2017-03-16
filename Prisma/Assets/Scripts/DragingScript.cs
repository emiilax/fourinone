using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragingScript : MonoBehaviour {

	public GameObject gameObjectToDrag;

	public Vector3 GOcenter; // The game object center

	public Vector3 touchPosition; // Where on the object the mouse click

	public Vector3 offset; // vector between GOcenter and touchPosition

	public Vector3 newGOcenter;

	public bool draggingMode;

	RaycastHit hit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Clicked!");

			//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

			if(hit.collider != null){

				Debug.Log ("Raycast hit");

				gameObjectToDrag = hit.collider.gameObject;

				GOcenter = gameObjectToDrag.transform.position;

				touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				offset = touchPosition - GOcenter;

				draggingMode = true;

			}
		}

		if (Input.GetMouseButton (0)) {
		
			if (draggingMode) {

				touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				newGOcenter = touchPosition - offset;

				gameObjectToDrag.transform.position = newGOcenter;

			}

		}

		if (Input.GetMouseButtonUp (0)) {
			draggingMode = false;
		}

	}
}
