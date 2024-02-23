using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settings_menu_logic : MonoBehaviour
{
    public GameObject main_menu;

    public void back_button()
    {
        gameObject.active = false;
        main_menu.active = true;
    }

}
