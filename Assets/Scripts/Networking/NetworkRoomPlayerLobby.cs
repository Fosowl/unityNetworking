using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject lobbyUI = null;
    [SerializeField] public TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] public Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "You name...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        //CmdSetDisplayName(PlayerNameInput.DisplayName);

        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPlayers.Add(this);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        Room.RoomPlayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        //if (!hasAuthority) {
        //    foreach (var player in Room.RoomPlayers) {
        //        if (player.hasAuthority) {
        //            player.UpdateDisplay();
        //            break;
        //        }
        //    }
        //    return;
        //}

        for (int i = 0; i < playerNameTexts.Length; i++) {
            playerNameTexts[i].text = "Waiting For Player...";
        }

        for (int i = 0; i < Room.RoomPlayers.Count; i++) {
            playerNameTexts[i].text = Room.RoomPlayers[i].IsReady ?
                Room.RoomPlayers[i].DisplayName + "<color=green>Ready</color>" :
                Room.RoomPlayers[i].DisplayName + "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }
        startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) { return; }

        Room.StartGame();
    }
}