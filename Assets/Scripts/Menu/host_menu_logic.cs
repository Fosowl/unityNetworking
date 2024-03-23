using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class host_menu_logic : MonoBehaviour
{
    // menu
    public GameObject main_menu;
    // field
    public GameObject input_field_password;
    public GameObject input_field_port;
    public GameObject input_field_username;
    public GameObject input_field_max_players;
    // text
    public GameObject text_error;

    // CHANGE
    [SerializeField] private MyNetworkManager networkManager = null;

    // private
    enum Error
    {
        NONE = 0,
        PORT = 1,
        USERNAME = 2,
        PASSWORD_FORMAT = 3,
        PASSWORD = 4,
        MAX_PLAYERS = 5
    }

    Error error_code = Error.NONE;

    void Start()
    {
        text_error.SetActive(false);
    }

    public void back_button()
    {
        gameObject.SetActive(false);
        main_menu.SetActive(true);
    }


    void fill_error(string error)
    {
        text_error.SetActive(true);
        text_error.GetComponent<Text>().text = error;
    }

    int set_port(string port_str)
    {
        int port;

        if (error_code != Error.NONE && error_code != Error.PORT) {
            return -1;
        }

        if (!int.TryParse(port_str, out port)) {
            error_code = Error.PORT;
            Debug.Log("ERROR: port is not a number");
            return -1;
        }
        if (port < 1024 || port > 65535) {
            error_code = Error.PORT;
            Debug.Log("ERROR: Invalid port range");
            return -1;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetInt("port_client", port);
        return port;
    }

    int set_max_players(string max_players_str)
    {
        int max_players;

        if (error_code != Error.NONE && error_code != Error.MAX_PLAYERS) {
            return -1;
        }

        if (!int.TryParse(max_players_str, out max_players)) {
            error_code = Error.MAX_PLAYERS;
            Debug.Log("ERROR: max players is not a number");
            return -1;
        }
        if (max_players < 2 || max_players > 10) {
            error_code = Error.MAX_PLAYERS;
            Debug.Log("ERROR: Invalid max players range");
            return -1;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetInt("max_players", max_players);
        return max_players;
    }

    string set_username(string username)
    {
        if (error_code != Error.NONE && error_code != Error.USERNAME) {
            return "";
        }

        if (username.Length < 3 || username.Length > 15) {
            Debug.Log("ERROR: Invalid username length");
            error_code = Error.USERNAME;
            return "";
        }
        if (username.Contains(" ")) {
            Debug.Log("ERROR: Username canoot contains space");
            error_code = Error.USERNAME;
            return "";
        }
        error_code = Error.NONE;
        PlayerPrefs.SetString("username", username);
        return username;
    }

    bool set_host(bool is_host)
    {
        if (is_host) {
            PlayerPrefs.SetString("is_host", "true");
        }
        else {
            PlayerPrefs.SetString("is_host", "false");
        }
        return is_host;
    }

    string set_password(string password)
    {
        if (error_code != Error.NONE && error_code != Error.PASSWORD_FORMAT) {
            return "";
        }

        // TODO: check password correct
        error_code = Error.NONE;
        // TODO: encrypt password
        string hash_password = password;
        PlayerPrefs.SetString("password_client", hash_password);
        return hash_password;
    }

    public void host_game_button()
    {
        bool host = set_host(true);
        int port = set_port(input_field_port.GetComponent<InputField>().text);
        string username = set_username(input_field_username.GetComponent<InputField>().text);
        string password = set_password(input_field_password.GetComponent<InputField>().text);
        int max_players = set_max_players(input_field_max_players.GetComponent<InputField>().text);
        switch (error_code)
        {
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
            case Error.MAX_PLAYERS:
                fill_error("Invalid MAX PLAYERS");
                break;
            default:
                text_error.SetActive(false);
                gameObject.SetActive(false);
                error_code = Error.NONE;
                host_lobby(max_players, port, username, password);
                break;
        }
    }

    private void host_lobby(int max_players, int port, string username, string password)
    {
        Debug.Log("Hosting game...");
        // that's it ? check working
        // Attention telepathy transport sur le MyNetworkMananger pas kcp ?

        //NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        //KcpTransport kcpTransport = networkManager.GetComponent<KcpTransport>();
        //if (kcpTransport != null) {
        //    kcpTransport.Port = port;
        //    Debug.Log("KCP Transport port changed to: " + kcpTransport.Port);
        //}
        //else {
        //    Debug.LogWarning("KCP Transport component not found on NetworkManager.");
        //}
        networkManager.StartHost();
    }
}
