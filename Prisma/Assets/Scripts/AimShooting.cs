using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimShooting : MonoBehaviour {

	private LineRenderer line;
	public bool isHit;
	private Camera playerCam;

	private Vector3 offset; 

	// Use this for initialization
	void Start () {



		line = gameObject.GetComponentInChildren<LineRenderer>();
		line.enabled = false;
		line.useWorldSpace = true;

		RectTransform rt = (RectTransform)gameObject.transform;

		offset = new Vector3 (-Camera.main.pixelWidth / 128, Camera.main.pixelHeight / 128, 0);

		Vector3 uppercorner = new Vector3 (gameObject.transform.position.x - (rt.rect.width / 2), gameObject.transform.position.y + (rt.rect.height / 2), 0);
		Vector3 lowercorner = new Vector3 (gameObject.transform.position.x + (rt.rect.width / 2), gameObject.transform.position.y - (rt.rect.height / 2), 0);

		Debug.Log ("Camera width: " + Camera.main.pixelWidth/64);

		Debug.Log ("upper Corner (x,y): " + uppercorner);
		Debug.Log ("lower Corner (x,y): " + lowercorner);
		Vector3 direction = new Vector3 (lowercorner.x - uppercorner.x, lowercorner.y - uppercorner.y, 0);
		Ray2D ray = new Ray2D (rt.rect.center, direction);

		line.SetPosition (0, gameObject.transform.position   );
		line.SetPosition (1, ray.GetPoint (100));

		line.enabled = false;

	}

	
	// Update is called once per frame
	void Update () {


		if (Input.GetButtonDown("Fire1"))
			Debug.Log("MousePos: " + (Camera.main.ScreenToWorldPoint (Input.mousePosition)-offset));
			StopCoroutine("FireLaser");
			StartCoroutine("FireLaser");
		
		

	}




	IEnumerator FireLaser()
	{
		if(Input.GetButton("Fire1"))
		{
			line.enabled = false;
			//if (isLocalPlayer) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition) - offset;
				Vector3 playerPosition = gameObject.transform.position-offset;
				Vector3 direction = new Vector3 (mousePosition.x-playerPosition.x, mousePosition.y-playerPosition.y, 0);
				Ray2D ray = new Ray2D (playerPosition, direction);
				RaycastHit2D hit = Physics2D.Raycast (playerPosition, direction);
				line.numPositions = 2;
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

							line.SetPosition (ptNum, hit.point);
							//isHit = true;
							break;
						} else if (hit.collider.tag.Equals ("Mirror")) {
							// Debug.Log(bounceNum);
							if (bounceNum == maxBounces) {
								line.SetPosition (ptNum, hit.point);
								//isHit = true;
								break;
							}
							line.SetPosition (ptNum, hit.point);
							//isHit = true;

							Vector3 origin = line.GetPosition (ptNum - 1);
							Vector3 hitPoint = hit.point;
							Vector3 incoming = hitPoint - origin;
							Vector3 normal = new Vector3 (hit.normal.x, hit.normal.y, 0);
							Vector3 reflected = Vector3.Reflect (incoming, hit.normal);

							ray = new Ray2D (hitPoint, reflected);
							hit = Physics2D.Raycast (hitPoint + offsetReflection * normal, reflected);
							ptNum++;
							line.numPositions++;
							bounceNum++;
			

						}

					} else {
						line.SetPosition (ptNum, ray.GetPoint (100));
						break;
					}
				}


				//gets all the points of the laser
				Vector3[] laserPositions = new Vector3[line.numPositions];
				line.GetPositions (laserPositions);


			line.enabled = true;
				
			yield return null;

			//}



			//ShutOffTimer = Time.time + shutDownDelay;
			//Fire();
		}
	}

}
