using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Set", menuName = "Rounds/Map Set")]
public class MapSet : ScriptableObject
{
    [Scene]
    [SerializeField] private List<string> maps = new List<string>();

    public IReadOnlyCollection<string> Maps => maps.AsReadOnly();
}

// move to other file ?
public class MapHandler
{
    private readonly IReadOnlyCollection<string> maps;
    private readonly int numberOfRounds;

    private int currentRound;
    private List<string> remainingMaps;

    public MapHandler(MapSet mapSet, int numberOfRounds)
    {
        maps = mapSet.Maps;
        this.numberOfRounds = numberOfRounds;
        ResetMaps();
    }

    public bool IsComplete => currentRound == numberOfRounds;

    public string NextMap
    {
        get
        {
            if (IsComplete) { return null; }
            currentRound++;
            if (remainingMaps.Count == 0) { ResetMaps(); }
            string map = remainingMaps[UnityEngine.Random.Range(0, remainingMaps.Count)];
            remainingMaps.Remove(map);
            return map;
        }
    }
    private void ResetMaps() => remainingMaps = maps.ToList();
}

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Maps")]
    [SerializeField] private int numberOfRounds = 1;
    [SerializeField] private MapSet mapSet = null;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    //[SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;

    private MapHandler mapHandler;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs) {
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
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene) {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene) {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();
        RoomPlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers) {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers) {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene) {
            if (!IsReadyToStart()) { return; }
            mapHandler = new MapHandler(mapSet, numberOfRounds);
            ServerChangeScene(mapHandler.NextMap);
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("game"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--) {
                var conn = RoomPlayers[i].connectionToClient;
                //var gameplayerInstance = Instantiate(gamePlayerPrefab);
                //gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                //NetworkServer.Destroy(conn.identity.gameObject);
                //NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("game")) {
            //GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            //NetworkServer.Spawn(playerSpawnSystemInstance);

            //GameObject roundSystemInstance = Instantiate(roundSystem);
            //NetworkServer.Spawn(roundSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }
}