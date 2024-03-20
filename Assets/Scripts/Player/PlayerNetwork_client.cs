using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerNetwork
{
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SetLife(3);
    }

    #region SyncVar Hooks

    [Client]
    private void OnPlayerLifeChanged(int oldLife, int newLife)
    {
        this.lifeUI.text = newLife.ToString();
    }

    [Client]
    private void OnPlayerPseudoChanged(string oldPseudo, string newPseudo)
    {
        this.pseudoUI.text = newPseudo;
    }

    #endregion

}
