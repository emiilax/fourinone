using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingDoorParticles : MonoBehaviour {
    public ParticleSystem particles;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnDisable() {
        particles.Play();
		//TODO enable
        Debug.Log("Particles active");
    }
}
