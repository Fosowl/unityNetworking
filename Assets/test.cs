using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine;

public class test : NetworkManager
{
    public List<lobby_menu> LobbyPlayers { get; } = new List<lobby_menu>();
}