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
        host_menu.SetActive(false); 
        join_menu.SetActive(false);
        settings_menu.SetActive(false);
    }

    public void join_button()
    {
        gameObject.SetActive(false);
        join_menu.SetActive(true);
    }

    public void host_button()
    {
        gameObject.SetActive(false);
        host_menu.SetActive(true);
    }

    public void settings_button()
    {
        gameObject.SetActive(false);
        settings_menu.SetActive(true);
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
