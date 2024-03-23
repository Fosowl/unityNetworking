using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] public int minPlayers = 2;
    [Scene] public string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] public lobby_menu roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] public gamePlayer gamePlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<lobby_menu> RoomPlayers { get; } = new List<lobby_menu>();
    public List<gamePlayer> GamePlayers { get; } = new List<gamePlayer>();

    private Vector3 spawnPosition = new Vector3(58f, 11f, 60f);

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

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
        float x = spawnPosition.x + 2;
        float y = spawnPosition.y;
        float z = spawnPosition.z - 1;
        spawnPosition = new Vector3(x, y, z);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().path == menuScene) {
            if (roomPlayerPrefab == null) {
                Debug.Log("roomPlayerPrefab is null");
                return;
            }
            lobby_menu lobbyPlayerInstance = Instantiate(roomPlayerPrefab);
            Debug.Log("Adding player for connection...");
            NetworkServer.AddPlayerForConnection(conn, lobbyPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<GameObject>();
            Debug.Log("Disconnecting player...");
            //RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        Debug.Log("Notifying player of ready states...");
        foreach (var player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) {
            Debug.Log("Not enought players to start.");
            return false;
        }

        foreach (var player in RoomPlayers) {
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
        if (SceneManager.GetActiveScene().path == menuScene) {
            if (!IsReadyToStart()) {
                Debug.Log("StartGame() Not ready to start.");
                return;
            }
            Debug.Log("Starting game scene...");
            ServerChangeScene("game");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        Debug.Log("Transitionning from menu to game...");
        if (SceneManager.GetActiveScene().path == menuScene && newSceneName.StartsWith("game")) {
            Debug.Log("Instanciating players for game.");
            var roomPlayersCopy = new List<lobby_menu>(RoomPlayers);
        
            for (int i = roomPlayersCopy.Count - 1; i >= 0; i--)
            {
                if (i >= roomPlayersCopy.Count) {
                    Debug.LogError($"Index out of range: {i}");
                    continue;
                }
                var conn = roomPlayersCopy[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab, spawnPosition, Quaternion.identity);
                gameplayerInstance.SetDisplayName(roomPlayersCopy[i].DisplayName);
                //NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }
}