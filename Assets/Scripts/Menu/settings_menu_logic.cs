using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settings_menu_logic : MonoBehaviour
{
    public GameObject main_menu;

    public void back_button()
    {
        gameObject.SetActive(false);
        main_menu.SetActive(true);
    }

}
