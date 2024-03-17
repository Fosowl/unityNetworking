using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wintrigger : MonoBehaviour
{
    public GameObject winmenu;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object contact");
        if (other.tag != "PlayerMain") {
            return;
        }
        winmenu.active = true;
    }
}
