using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FishNet.Connection;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Player> playerList = new List<Player>();

    public void AddPlayer(Player player)
    {
        playerList.Add(player);
    }


}
