using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserScript : MonoBehaviour {

    private LineRenderer line;
    public bool isHit;
	private Camera playerCam;

	// Use this for initialization
	void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = true;
        line.useWorldSpace = true;
		playerCam = transform.parent.GetComponent<Camera>();
		Debug.Log("parent : " + transform.parent.name);
	}
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")){
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
        }
    }
		
    IEnumerator FireLaser()
    {
        line.enabled = true;

        while (Input.GetButton("Fire1"))
        {
			transform.forward = playerCam.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(transform.position, transform.forward);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward);
            line.numPositions = 2;

            // Debug.Log("bla");
            line.SetPosition(0, ray.origin);
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
                if (hit.collider)
                {
                    if (hit.collider.tag.Equals("Wall"))
                    {

                        line.SetPosition(ptNum, hit.point);
                        isHit = true;
                        break;
                    }
                    else if (hit.collider.tag.Equals("Mirror"))
                    {
                       // Debug.Log(bounceNum);
                        if (bounceNum == maxBounces)
                        {
                            line.SetPosition(ptNum, hit.point);
                            isHit = true;
                            break;
                        }
                        line.SetPosition(ptNum, hit.point);
                        isHit = true;

                        Vector3 origin = line.GetPosition(ptNum - 1);
                        Vector3 hitPoint = hit.point;
                        Vector3 incoming = hitPoint - origin;
                        Vector3 normal = new Vector3(hit.normal.x, hit.normal.y, 0);
                        //Debug.Log(incoming);-2*Vector3.Dot(incoming, normal)*normal - incoming; //
                        Vector3 reflected = Vector3.Reflect(incoming, hit.normal);

                        ray = new Ray2D(hitPoint, reflected);
                        hit = Physics2D.Raycast(hitPoint + offsetReflection * normal, reflected);
                        ptNum++;
                        line.numPositions++;
                        bounceNum++;
                        //line.SetPosition(ptNum, reflected + hitPoint);

                    }

                }
                else
                {
                    line.SetPosition(ptNum, ray.GetPoint(100));
                    break;
                }
            }
            yield return null;
        }

        line.enabled = false;
    }
}
