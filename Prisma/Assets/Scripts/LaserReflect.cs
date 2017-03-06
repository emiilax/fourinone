using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReflect : MonoBehaviour {

    private LineRenderer line;

    // Use this for initialization
    void Start() {
        GameObject player = GameObject.Find("Player");
        line = gameObject.GetComponent<LineRenderer>();
//        LaserScript laserScript = player.gameObject.GetComponent<LaserScript>();
        line.enabled = false;
        line.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            StopCoroutine("Reflect");
            StartCoroutine("Reflect");
        }
    }

    IEnumerator Reflect() {
        line.enabled = true;
        GameObject player = GameObject.Find("Player");
        LaserScript laserScript = player.gameObject.GetComponent<LaserScript>();

        while (Input.GetButton("Fire1") || laserScript.isHit == true ) {
            Ray2D ray = new Ray2D(transform.position, transform.up);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up);

            line.SetPosition(0, ray.origin);

            if (hit.collider) {
                line.SetPosition(1, hit.point);
            } else {
                line.SetPosition(1, ray.GetPoint(100));
            }

            yield return null;
        }

        line.enabled = false;
    }
}