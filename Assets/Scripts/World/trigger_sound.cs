using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger_sound : MonoBehaviour
{
    public AudioClip source;

    void Start()
    {
		GetComponent<AudioSource>().playOnAwake = false;
		GetComponent<AudioSource>().clip = source; 
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "PlayerMain") {
            return;
        }
		GetComponent<AudioSource>().Play();
        Debug.Log("Sound played");
	}
}
