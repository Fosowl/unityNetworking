using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class join_menu_logic : MonoBehaviour
{
    // menu
    public GameObject main_menu;
    public GameObject lobby_menu;
    // field
    public GameObject input_field_ip;
    public GameObject input_field_port;
    public GameObject input_field_username;
    public GameObject input_field_password;
    // text
    public GameObject text_error;
    // private

    enum Error
    {
        NONE = 0,
        IP = 1,
        PORT = 2,
        USERNAME = 3,
        PASSWORD_FORMAT = 4,
        PASSWORD = 5
    }

    Error error_code = Error.NONE;

    void Start()
    {
        text_error.active = false;
    }

    public void back_button()
    {
        gameObject.active = false;
        main_menu.active = true;
    }

    void fill_error(string error)
    {
        text_error.active = true;
        text_error.GetComponent<Text>().text = error;
    }

    public static bool IsValidIPv4(string ipAddress)
    {
        string[] parts = ipAddress.Split('.');
        if (parts.Length != 4) {
            return false;
        }
        foreach (string part in parts) {
            if (!int.TryParse(part, out int parsedPart)) {
                return false;
            }
            if (parsedPart < 0 || parsedPart > 255) {
                return false;
            }
            if (part.Length > 1 && part.StartsWith("0")) {
                return false;
            }
        }
        return true;
    }

    void set_ip(string ip)
    {
        if (error_code != Error.NONE && error_code != Error.IP) {
            return;
        }
        if (ip.Length < 7 || ip.Length > 15) {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IP length");
            return;
        }
        if (ip[0] == '.' || ip[ip.Length - 1] == '.') {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IP format");
            return;
        }
        if (!IsValidIPv4(ip)) {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IPv4");
            return;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetString("ip_client", ip);
    }

    void set_port(string port_str)
    {
        int port;

        if (error_code != Error.NONE && error_code != Error.PORT) {
            return;
        }

        if (!int.TryParse(port_str, out port)) {
            error_code = Error.PORT;
            Debug.Log("ERROR: port is not a number");
            return;
        }
        if (port < 1024 || port > 65535) {
            error_code = Error.PORT;
            Debug.Log("ERROR: Invalid port range");
            return;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetInt("port_client", port);
    }

    void set_username(string username)
    {
        if (error_code != Error.NONE && error_code != Error.USERNAME) {
            return;
        }

        if (username.Length < 3 || username.Length > 15) {
            Debug.Log("ERROR: Invalid username length");
            error_code = Error.USERNAME;
            return;
        }
        if (username.Contains(" ")) {
            Debug.Log("ERROR: Username canoot contains space");
            error_code = Error.USERNAME;
            return;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetString("username", username);
    }

    void set_host(bool is_host)
    {
        if (is_host) {
            PlayerPrefs.SetString("is_host", "true");
        }
        else {
            PlayerPrefs.SetString("is_host", "false");
        }
    }

    void check_password(string password)
    {
        if (error_code != Error.NONE && error_code != Error.PASSWORD_FORMAT) {
            return;
        }

        // TODO: check password correct
        error_code = Error.NONE;
        // TODO: encrypt password
        PlayerPrefs.SetString("password_client", password);
    }

    public void join_button()
    {
        Debug.Log("Joining game...");
        set_host(false);
        set_ip(input_field_ip.GetComponent<InputField>().text);
        set_port(input_field_port.GetComponent<InputField>().text);
        set_username(input_field_username.GetComponent<InputField>().text);
        check_password(input_field_password.GetComponent<InputField>().text);
        switch (error_code)
        {
            case Error.IP:
                fill_error("Invalid IP");
                break;
            case Error.PORT:
                fill_error("Invalid PORT");
                break;
            case Error.USERNAME:
                fill_error("Invalid USERNAME");
                break;
            case Error.PASSWORD_FORMAT:
                fill_error("Invalid password length or format");
                break;
            case Error.PASSWORD:
                fill_error("Incorrect PASSWORD");
                break;
            default:
                text_error.active = false;
                gameObject.active = false;
                lobby_menu.active = true;
                error_code = Error.NONE;
                break;
        }
    }
}
