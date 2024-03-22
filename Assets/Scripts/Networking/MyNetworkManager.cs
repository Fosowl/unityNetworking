using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine;

// network manager for lobby is a room that contain //GamePlayers
public class MyNetworkManager : NetworkManager
{
    [SerializeField] public int minPlayers = 2;

    [SerializeField] public GameObject gamePlayerPrefab = null;
    [SerializeField] public GameObject lobbyPlayerPrefab = null;
    [SerializeField] public GameObject playerSpawnSystem = null;

    [SerializeField] public static event Action OnClientConnected;
    [SerializeField] public static event Action OnClientDisconnected;
    [SerializeField] public static event Action<NetworkConnection> OnServerReadied;
    [SerializeField] public static event Action OnServerStopped;

    bool is_host;

    // either lobby_menu or 2 class : one for lobby player and one for actual game player
    // unity should find other script like lobby_menu by default without error but get error why ???????

    public List<lobby_menu> LobbyPlayers { get; } = new List<lobby_menu>();
    public List<lobby_menu> GamePlayers { get; } = new List<lobby_menu>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs) {
            Debug.Log("Registering prefab...");
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections) {
            Debug.Log("numPlayers >= maxConnection, disconnecting...");
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().path == "scene:Assets/Scenes/menu.unity") {
            if (lobbyPlayerPrefab == null) {
                Debug.Log("lobbyPlayerPrefab is null");
                return;
            }
            GameObject lobbyPlayerInstance = Instantiate(lobbyPlayerPrefab);
            Debug.Log("Adding player for connection...");
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<GameObject>();
            Debug.Log("Disconnecting player...");
            //LobbyPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();
        LobbyPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        Debug.Log("Notifying player of ready states...");
        foreach (var player in LobbyPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) {
            Debug.Log("Not enought players to start.");
            return false;
        }

        foreach (var player in LobbyPlayers) {
            if (!player.IsReady) {
                Debug.Log("A player is not ready.");
                return false;
            }
            Debug.Log("Player is ready:" + player);
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == "scene:Assets/Scenes/menu.unity") {
            if (!IsReadyToStart()) {
                Debug.Log("StartGame() Not ready to start.");
                return;
            }
            ServerChangeScene("game");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().path == "scene:Assets/Scenes/menu.unity" && newSceneName.StartsWith("game")) {
            Debug.Log("Instanciating players for game.");
            for (int i = LobbyPlayers.Count - 1; i >= 0; i--) {
                var conn = LobbyPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(LobbyPlayers[i].DisplayName);
                //NetworkServer.Destroy(conn.identity.GameObject); // keep ? see https://stackoverflow.com/questions/62458966/why-does-the-host-player-not-have-authority-to-send-server-command-unity-mi
                //NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("game")) {
            Debug.Log("OnServerSceneChanged() called");
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }
}