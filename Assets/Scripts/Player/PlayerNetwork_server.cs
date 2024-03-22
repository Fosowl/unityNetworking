using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerNetwork
{
    [Server]
    public void SetPseudo(string newPseudo) => this._playerPseudo = newPseudo;

    [Server]
    public void SetLife(int newLife) => this._playerLife = newLife;

    [ServerCallback]

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("damage"))
        {
            SetLife(this._playerLife - 1);
        }
    }
}
