using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

    // CHANGE
    [SerializeField] private NetworkManagerLobby networkManager = null;

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

    string set_ip(string ip)
    {
        if (error_code != Error.NONE && error_code != Error.IP) {
            return null;
        }
        if (ip.Length < 7 || ip.Length > 15) {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IP length");
            return null;
        }
        if (ip[0] == '.' || ip[ip.Length - 1] == '.') {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IP format");
            return null;
        }
        if (!IsValidIPv4(ip)) {
            error_code = Error.IP;
            Debug.Log("ERROR: Invalid IPv4");
            return null;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetString("ip_client", ip);
        return ip;
    }

    string set_port(string port_str)
    {
        int port;

        if (error_code != Error.NONE && error_code != Error.PORT) {
            return null;
        }

        if (!int.TryParse(port_str, out port)) {
            error_code = Error.PORT;
            Debug.Log("ERROR: port is not a number");
            return null;
        }
        if (port < 1024 || port > 65535) {
            error_code = Error.PORT;
            Debug.Log("ERROR: Invalid port range");
            return null;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetInt("port_client", port);
        return port
    }

    string set_username(string username)
    {
        if (error_code != Error.NONE && error_code != Error.USERNAME) {
            return null;
        }

        if (username.Length < 3 || username.Length > 15) {
            Debug.Log("ERROR: Invalid username length");
            error_code = Error.USERNAME;
            return null;
        }
        if (username.Contains(" ")) {
            Debug.Log("ERROR: Username canoot contains space");
            error_code = Error.USERNAME;
            return null;
        }
        error_code = Error.NONE;
        PlayerPrefs.SetString("username", username);
        return username
    }

    bool set_host(bool is_host)
    {
        if (is_host) {
            PlayerPrefs.SetString("is_host", "true");
        }
        else {
            PlayerPrefs.SetString("is_host", "false");
        }
        return is_host
    }

    string check_password(string password)
    {
        if (error_code != Error.NONE && error_code != Error.PASSWORD_FORMAT) {
            return;
        }

        // TODO: check password correct
        error_code = Error.NONE;
        // TODO: encrypt password
        PlayerPrefs.SetString("password_client", password);
    }

    string check_password(string password)
    {
        if (error_code != Error.NONE && error_code != Error.PASSWORD_FORMAT) {
            return;
        }
        string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$";
        if (!Regex.IsMatch(password, passwordPattern)) {
            error_code = Error.PASSWORD_FORMAT;
            return null;
        }
        error_code = Error.NONE;
        Debug.Log("Hashing password...");
        string salt = GenerateSalt();
        string hashedPassword = HashPassword(password, salt);
        PlayerPrefs.SetString("password_client_hash", hashedPassword);
        PlayerPrefs.SetString("password_client_s")
        return hashedPassword;
    }

    private string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        { 
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    private string HashPassword(string password, string salt)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = Convert.FromBase64String(salt);

        using (var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 10000))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public void join_button()
    {
        Debug.Log("Joining game...");
        set_host(false);
        string ip = set_ip(input_field_ip.GetComponent<InputField>().text);
        string port = set_port(input_field_port.GetComponent<InputField>().text);
        string username = set_username(input_field_username.GetComponent<InputField>().text);
        string passwordHashed = check_password(input_field_password.GetComponent<InputField>().text);

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
                JoinLobby(ip, port);
                lobby_menu.active = true;
                error_code = Error.NONE;
                break;
        }
    }

    public void JoinLobby(string ip, string port)
    {
        networkManager.networkAddress = ip;
        networkManager.networkPort = port;
        networkManager.StartClient();
    }

    private void OnEnable()
    {
        // subcribe event
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        // unsubcribe event
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }
}
