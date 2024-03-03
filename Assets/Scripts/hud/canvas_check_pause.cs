using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvas_check_pause : MonoBehaviour
{
    public GameObject pause_menu;
    void Start()
    {
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            pause_menu.active = true;
        }
    }
}
