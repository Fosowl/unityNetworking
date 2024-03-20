using Mirror;
using TMPro;
using UnityEngine;

public partial class PlayerNetwork : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerLifeChanged))]
    private int _playerLife;

    [SyncVar(hook = nameof(OnPlayerPseudoChanged))]
    private string _playerPseudo;

    [SerializeField] private TMP_Text lifeUI;
    [SerializeField] private TMP_Text pseudoUI;
}
