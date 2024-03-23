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
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    public TMP_Text[] playerNameTexts = new TMP_Text[4];
    public Button startGameButton = null;
    private GameObject join_menu;
    private GameObject host_menu;
    private GameObject parent;

    public string DisplayName = "martin";
    public bool IsReady = false;
    public bool is_host;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }
    // private
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

    private RectTransform rectTransform;
    void Start()
    {
        GameObject parent = GameObject.FindWithTag("ParentCanvas");
        this.transform.SetParent(parent.transform);

        is_host = PlayerPrefs.GetString("is_host") == "true";
        Debug.Log("Is host?: " + is_host);

        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(150f, 45f);
        string username = PlayerPrefs.GetString("username");
        Debug.Log("Setting username:" + username);
        CmdSetDisplayName(username);
    }

    public void back_button()
    {
        Debug.Log("Back button pressed.");
        gameObject.SetActive(false);
        if (is_host) {
            Debug.Log("Host is leaving lobby...");
            host_menu.SetActive(true);
        }
        else {
            Debug.Log("Client is leaving lobby...");
            join_menu.SetActive(true);
        }
    }

    public override void OnStartAuthority()
    {
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);
        Debug.Log("Client start.. Room player added !");
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);
        Debug.Log("Client stopped.. Room player removed !");
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!isOwned) {
            foreach (var player in Room.RoomPlayers) {
                if (player.isOwned) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player " + i + "...";
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++) {
            playerNameTexts[i].text = Room.RoomPlayers[i].IsReady ?
                Room.RoomPlayers[i].DisplayName + "    <color=green>Ready</color>" :
                Room.RoomPlayers[i].DisplayName + "    <color=red>Not Ready</color>";
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
        UpdateDisplay();
        RpcSetDisplayNameOnClient(displayName);
    }

    [ClientRpc]
    private void RpcSetDisplayNameOnClient(string displayName)
    {
        if (!isServer) {
            Debug.Log("Setting name on client...");
            DisplayName = displayName;
            UpdateDisplay();
        }
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Debug.Log("Notifying of ready state.");
        Room.NotifyPlayersOfReadyState();
        UpdateDisplay();
        RpcReadyUp();
    }

    [ClientRpc]
    private void RpcReadyUp()
    {
        if (!isServer) {
            IsReady = !IsReady;

            Debug.Log("Notifying of ready state on client...");
            UpdateDisplay();
        }
    }

    [Command]
    public void CmdStartGame()
    {
        // this cause error ? 
        //if (Room.GamePlayers[0].connectionToClient != connectionToClient) {
            //Debug.Log("Wrong connection in CmdStartGame.");
            //return;
        //}

        Debug.Log("Starting game...");
        RpcStartGame();
        if (!isServer) {
            return;
        }
        Room.StartGame(); // scene change handled byStartGame  in MyNetworkmanager        }
    }

        [ClientRpc]
        private void RpcStartGame()
        {
            if (!isServer) {
                return;
            }
        }
}
