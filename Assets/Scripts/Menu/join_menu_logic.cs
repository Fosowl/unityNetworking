using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class join_menu_logic : MonoBehaviour
{
    public GameObject main_menu;
    public GameObject input_field_ip;
    public GameObject input_field_port;

    void Start()
    {
    }

    public void back_button()
    {
        gameObject.active = false;
        main_menu.active = true;
    }

    public void join_button()
    {
        Debug.Log("Joining game...");
        PlayerPrefs.SetString("is_host", "false");
        PlayerPrefs.SetString("ip_client", input_field_ip.GetComponent<InputField>().text);
        PlayerPrefs.SetInt("port_client", int.Parse(input_field_port.GetComponent<InputField>().text));
        SceneManager.LoadScene("game");
    }
}
