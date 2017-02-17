using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {

    private LineRenderer line;
    public bool isHit;

	// Use this for initialization
	void Start () {
        line = gameObject.GetComponent<LineRenderer>();
        line.enabled = false;
        line.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")){
            StopCoroutine("FireLaser");
            StartCoroutine("FireLaser");
        }
    }

    IEnumerator FireLaser(){
        line.enabled = true;

        while (Input.GetButton("Fire1")) {
            transform.forward = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(transform.position, transform.forward);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.forward);

            line.SetPosition(0, ray.origin);

            if (hit.collider) {
                line.SetPosition(1, hit.point);
                isHit = true;

            } else {
                line.SetPosition(1, ray.GetPoint(100));
            }

            yield return null;
        }

        line.enabled = false;
    }
}
