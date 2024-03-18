using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class winscreenlogic : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void back_to_lobby_button()
    {
        SceneManager.LoadScene("menu");
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
