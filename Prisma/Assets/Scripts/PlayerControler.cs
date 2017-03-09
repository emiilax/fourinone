using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControler : NetworkBehaviour {

	[SerializeField] float fireRate = 0.5f;
	LineRenderer laser;
	float nextFireTime;
	private Camera playerCam;

	public override void OnStartLocalPlayer ()
	{
		enabled = true;
	}

	public override void OnStartClient ()
	{
		laser = transform.Find ("LaserTip").GetComponent<LineRenderer> ();
		laser.enabled = false;
		enabled = false;
	}
		


	// Use this for initialization
	void Start () {
		playerCam = gameObject.GetComponent<Camera> ();
		Debug.Log (playerCam.transform.position.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Fire1") != 0 && canFire) {
			nextFireTime = Time.time + fireRate;
			Fire ();
		}
	}
	void Fire(){
		StartCoroutine (ShowLaser ());
		CmdShowLaser ();
	}

	[Command]
	void CmdShowLaser()
	{
		RpcShowLaser ();
	}

	[ClientRpc]
	void RpcShowLaser()
	{
		if (isLocalPlayer) {
			return;
		}
		Debug.Log ("Client RPC");
		StartCoroutine (ShowLaser ());

	}
	IEnumerator ShowLaser(){
		Debug.Log ("EnterShowLaser");
		{
			laser.enabled = true;

			//while (Input.GetButton("Fire1"))
			//{
				Debug.Log ("Enter while");
			Debug.Log (playerCam.transform.position.ToString ());
				Vector3 mousePosition = playerCam.ScreenToWorldPoint(Input.mousePosition);
				//transform.forward = playerCam.ScreenToWorldPoint(Input.mousePosition);
				Ray2D ray = new Ray2D(transform.position, mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePosition);
				laser.numPositions = 2;

				// Debug.Log("bla");
				laser.SetPosition(0, ray.origin);
				int ptNum = 1;
				int maxBounces = 16;
				int bounceNum = 0;
				//Verkar som att reflektionen kan fastna på samma spegel igen
				//om man inte flyttar ut den lite från spegeln bör kanske 
				//lösa detta på nåt bättre sätt men verkar fungera iallafall
				float offsetReflection = 0.01f;
				bool continueBouncing = true;
				while (continueBouncing)
				{
					Debug.Log ("Continue Bouncing");
					if (hit.collider)
					{
						if (hit.collider.tag.Equals("Wall"))
						{

							laser.SetPosition(ptNum, hit.point);
							//isHit = true;
							break;
						}
						else if (hit.collider.tag.Equals("Mirror"))
						{
							 Debug.Log(bounceNum + "Mirror");
							if (bounceNum == maxBounces)
							{
								laser.SetPosition(ptNum, hit.point);
								//isHit = true;
								break;
							}
							laser.SetPosition(ptNum, hit.point);
							//isHit = true;

							Vector3 origin = laser.GetPosition(ptNum - 1);
							Vector3 hitPoint = hit.point;
							Vector3 incoming = hitPoint - origin;
							Vector3 normal = new Vector3(hit.normal.x, hit.normal.y, 0);
							//Debug.Log(incoming);-2*Vector3.Dot(incoming, normal)*normal - incoming; //
							Vector3 reflected = Vector3.Reflect(incoming, hit.normal);

							ray = new Ray2D(hitPoint, reflected);
							hit = Physics2D.Raycast(hitPoint + offsetReflection * normal, reflected);
							ptNum++;
							laser.numPositions++;
							bounceNum++;
							//line.SetPosition(ptNum, reflected + hitPoint);

						}

					}
					else
					{
						laser.SetPosition(ptNum, ray.GetPoint(100));
						break;
					}
				//}
				yield return null;
			}

			laser.enabled = false;
		}
		/*laser.enabled = true;
		yield return new WaitForSeconds (0.1f);
		laser.enabled = false;*/
	}

	public bool canFire {
		get { return Time.time >= nextFireTime; }
	}
}
