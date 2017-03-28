using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimShooting : MonoBehaviour {

	private LineRenderer laser;
	public bool isHit;
	private Camera playerCam;

	private Vector3 offset; 

	private Vector3 oldDirection;

	private float offsetAngle;


	public GameObject gameObjectToDrag;

	public Vector3 GOcenter; // The game object center

	public Vector3 touchPosition; // Where on the object the mouse click

	public Vector3 dragOffset; // vector between GOcenter and touchPosition

	public Vector3 newGOcenter;

	public bool draggingMode;

	public bool inSinglePlayerMode = false;

	// Use this for initialization
	void Start () {



		laser = gameObject.GetComponentInChildren<LineRenderer>();
		laser.enabled = false;
		laser.useWorldSpace = true;

		RectTransform rt = (RectTransform)gameObject.transform;

		offset = new Vector3 (-Camera.main.pixelWidth / 128, Camera.main.pixelHeight / 128, 0);

		Vector3 uppercorner = new Vector3 (gameObject.transform.position.x - (rt.rect.width / 2), gameObject.transform.position.y + (rt.rect.height / 2), 0);
		Vector3 lowercorner = new Vector3 (gameObject.transform.position.x + (rt.rect.width / 2), gameObject.transform.position.y - (rt.rect.height / 2), 0);

		Debug.Log ("Camera width: " + Camera.main.pixelWidth/64);

		Debug.Log ("upper Corner (x,y): " + uppercorner);
		Debug.Log ("lower Corner (x,y): " + lowercorner);
		//Vector3 direction = new Vector3 (lowercorner.x - uppercorner.x, lowercorner.y - uppercorner.y, 0);


		float x = gameObject.transform.position.x - (rt.rect.width / 2);

		float y = gameObject.transform.position.y + (rt.rect.height / 2);



		Vector3 GOpos = gameObject.transform.position;

		Vector3 direction = new Vector3 (x - GOpos.x, y - GOpos.y, 0);

		offsetAngle = Vector2.Angle (Vector2.right, direction);

		float sign = (uppercorner.y < lowercorner.y) ? -1.0f : 1.0f;
		offsetAngle = Vector2.Angle (Vector2.right, direction) * sign + 180;
		//oldDirection = direction;
		Debug.Log ("Angle: " + offsetAngle);

	
		Ray2D ray = new Ray2D (rt.rect.center, direction);
		laser.SetPosition (0, gameObject.transform.position   );
		laser.SetPosition (1, ray.GetPoint (100));

		laser.enabled = false;

	}

	private void FireLaser(){
		
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 playerPosition = gameObject.transform.position;
		Vector3 direction = new Vector3 (mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y, 0);
		Ray2D ray = new Ray2D (playerPosition, direction);
		RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);

		//RotateSprite (direction);


		laser.numPositions = 2;
		int ptNum = 1;
		int maxBounces = 16;
		int bounceNum = 0;

		//Verkar som att reflektionen kan fastna på samma spegel igen
		//om man inte flyttar ut den lite från spegeln bör kanske 
		//lösa detta på nåt bättre sätt men verkar fungera iallafall
		float offsetReflection = 0.01f;
		bool continueBouncing = true;
		while (continueBouncing) {
			if (hit.collider) {
				if (hit.collider.tag.Equals ("Wall")) {

					laser.SetPosition (ptNum, hit.point);
					//isHit = true;
					break;
				} else if (hit.collider.tag.Equals ("Key")) {

					laser.SetPosition (ptNum, hit.point);
					//isHit = true;

					if (inSinglePlayerMode) {
						SingelPlayerController.instance.keyHit (hit.collider);
					}

					break;
				} else if (hit.collider.tag.Equals ("Mirror")) {
					// Debug.Log(bounceNum);
					if (bounceNum == maxBounces) {
						laser.SetPosition (ptNum, hit.point);
						//isHit = true;
						break;
					}
					laser.SetPosition (ptNum, hit.point);
					//isHit = true;

					Vector3 origin = laser.GetPosition (ptNum - 1);
					Vector3 hitPoint = hit.point;
					Vector3 incoming = hitPoint - origin;
					Vector3 normal = new Vector3 (hit.normal.x, hit.normal.y, 0);
					Vector3 reflected = Vector3.Reflect (incoming, hit.normal);

					ray = new Ray2D (hitPoint, reflected);
					hit = Physics2D.Raycast (hitPoint + offsetReflection * normal, reflected);
					ptNum++;
					laser.numPositions++;
					bounceNum++;


				}

			} else {
				laser.SetPosition (ptNum, ray.GetPoint (100));
				break;
			}
		}


		//gets all the points of the laser
		Vector3[] laserPositions = new Vector3[laser.numPositions];
		laser.GetPositions (laserPositions);
		laser.enabled = true;
	
	}

	
	// Update is called once per frame
	void Update () {

		laser.enabled = false;

		if (Input.GetButton ("Fire1")) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 playerPosition = gameObject.transform.position;
			Vector3 direction = new Vector3 (mousePosition.x - playerPosition.x, mousePosition.y - playerPosition.y, 0);

			if (direction.magnitude < 6) {
				FireLaser ();
			} else {
			
				// Now in the move object zone
				MoveObject ();

			}


		} else {
			draggingMode = false;
		}
		

	}

	// Function to move objects 
	private void MoveObject(){
		

		Debug.Log ("Move!");

		//Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

		if (hit.collider != null) {

			Debug.Log ("Raycast hit");

			gameObjectToDrag = hit.collider.gameObject;

			GOcenter = gameObjectToDrag.transform.position;

			touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			offset = touchPosition - GOcenter;

			draggingMode = true;

		}
			

		if (draggingMode) {

			touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			newGOcenter = touchPosition - offset;

			gameObjectToDrag.transform.position = newGOcenter;

		}
			
	}





}
