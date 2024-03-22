using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main_menu_logic : MonoBehaviour
{
    // public
    public GameObject host_menu;
    public GameObject join_menu;
    public GameObject settings_menu;

    void Start()
    {
        gameObject.SetActive(true);        
        host_menu.active = false; 
        join_menu.active = false;
        settings_menu.active = false;
    }

    public void join_button()
    {
        gameObject.SetActive(false);
        join_menu.active = true;
    }

    public void host_button()
    {
        gameObject.SetActive(false);
        host_menu.active = true;
    }

    public void settings_button()
    {
        gameObject.active = false;
        settings_menu.active = true;
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
