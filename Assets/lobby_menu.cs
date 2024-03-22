using Mirror;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class lobby_menu : NetworkBehaviour
{
    // public
    //public GameObject lobbyPlayerPrefab;
    [Header("UI")]
    public TMP_Text[] playerNameTexts = new TMP_Text[4];
    public Button startGameButton = null;
    public GameObject join_menu;
    public GameObject host_menu;

    [SyncVar]
    public string DisplayName = "You name...";
    public bool IsReady = false;
    public bool is_host;

    // private
    private string username;
    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null) {
                return room;
            }
            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    void Start()
    {
        is_host = PlayerPrefs.GetString("is_host") == "true";
        username = PlayerPrefs.GetString("username");
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

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(DisplayName);
    }

    public override void OnStartClient()
    {
        Room.LobbyPlayers.Add(this);
        Debug.Log("Client start.. Room player added !");
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
        Debug.Log("Client stopped.. Room player removed !");
        UpdateDisplay();
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!isLocalPlayer) {
            foreach (var player in Room.GamePlayers) {
                if (player.isLocalPlayer) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player " + i;
        }

        for (int i = 0; i < Room.GamePlayers.Count; i++) {
            playerNameTexts[i].text = Room.GamePlayers[i].IsReady ?
                Room.GamePlayers[i].DisplayName + "<color=green>Ready</color>" :
                Room.GamePlayers[i].DisplayName + "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!is_host) {
            return;
        }
        Debug.Log("Start button interactable ?" + readyToStart);
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        Debug.Log("Setting name...");
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        //if (!isLocalPlayer) {
            //Debug.Log("CmdReadyUp not local, skip..");
            //return;
        //}
        IsReady = !IsReady;
        Debug.Log("Notifying of ready state.");
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        //if (!isLocalPlayer) {
            //Debug.Log("CmdStartGame not local, skip");
            //return;
        //}
        if (Room.GamePlayers[0].connectionToClient != connectionToClient) {
            Debug.Log("Wrong connection in CmdStartGame.");
            return;
        }
        Debug.Log("Starting game...");
        Room.StartGame(); // scene change handled byStartGame  in MyNetworkmanager
    }
}
