using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class host_menu_logic : MonoBehaviour
{
    // menu
    public GameObject lobby_menu;
    public GameObject main_menu;
    // field
    public GameObject input_field_password;
    public GameObject input_field_port;

    void Start()
    {
    }

    public void back_button()
    {
        gameObject.active = false;
        main_menu.active = true;
    }

    public void host_game_button()
    {
        Debug.Log("Creating game...");
        Debug.Log("NOT IMPLEMENTED YET!");
        PlayerPrefs.SetString("is_host", "true");
        PlayerPrefs.SetString("password_host", input_field_password.GetComponent<InputField>().text);
        PlayerPrefs.SetInt("port_host", int.Parse(input_field_port.GetComponent<InputField>().text));
        gameObject.active = false;
        lobby_menu.active = true;
    }
}
