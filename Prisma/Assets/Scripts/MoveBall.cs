using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour {

	public Vector2 speed = new Vector2(30,50);

	private float direction = 0;
	// Update is called once per frame
	void Update () {




		foreach (Touch touch in Input.touches) {
			HandleTouch(touch.fingerId, Camera.main.ScreenToWorldPoint(touch.position), touch.phase);
		}

		// Simulate touch events from mouse events
		if (Input.touchCount == 0) {
			if (Input.GetMouseButton(0) ) {
				HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Stationary);
			} else {
				HandleTouch(10, Camera.main.ScreenToWorldPoint(Input.mousePosition), TouchPhase.Ended);
			}
		}

	}

	private void HandleTouch(int touchFingerId, Vector3 touchPosition, TouchPhase touchPhase) {
		switch (touchPhase) {
		case TouchPhase.Stationary:


			if (touchPosition.x < 0) {
				if (direction > -1)
					direction -= 0.1f;

			} else {
				if (direction < 1)
					direction += 0.1f;
			}
				
			moveBall ();

			break;


		case TouchPhase.Ended:
			if (direction > 0) {
				direction -= 0.1f;

				if (direction < 0)
					direction = 0;


			} else if (direction < 0) {
				direction += 0.1f;

				if (direction > 0)
					direction = 0;
			}

			moveBall ();

			break;
		}
	}

	private void moveBall(){
		Vector3 movement = new Vector3 ((float)direction * speed.x, 0, 0);

		movement *= Time.deltaTime;

		transform.Translate (movement);
	}
		

}
