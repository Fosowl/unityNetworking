using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause_menu_logic : MonoBehaviour
{
    void Start()
    {
        gameObject.active = false;
    }

    public void resume_button()
    {
        gameObject.active = false;
        Time.timeScale = 1;
    }

    public void back_to_menu_button()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("menu");
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
