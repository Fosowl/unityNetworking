using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class spawnHandler : NetworkManager
{
    void Start()
    {
    }

    public List<GameObject> spawns;

    //public override void OnServerAddPlayer(NetworkConnection conn)
    //{
    //    int spawnIndex = Random.Range(0, spawns.Count);
    //    Transform spawnPoint = spawns[spawnIndex].transform;

    //    GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    //    NetworkServer.AddPlayerForConnection(conn, player);
    //}

}
