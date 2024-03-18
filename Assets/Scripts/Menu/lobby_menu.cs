using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class lobby_menu : MonoBehaviour
{
    // public
    public GameObject join_menu;
    public GameObject host_menu;
    // private
    bool is_host;

    void Start()
    {
        is_host = PlayerPrefs.GetString("is_host") == "true";
        Debug.Log("Is host?: " + is_host);
    }

    public void back_button()
    {
        gameObject.SetActive(false);
        if (is_host) {
            Debug.Log("Host is leaving lobby...");
            host_menu.active = true;
        }
        else {
            Debug.Log("Client is leaving lobby...");
            join_menu.active = true;
        }
    }

    public void start_button()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("game");
    }
}
