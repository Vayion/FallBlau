using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Connection;
using Data.Scripts.GameElements.Units;
using System.Linq;

public class TheGameManager : NetworkBehaviour
{
    public static TheGameManager instance;

    [SerializeField] private GameTime gameTime;
    public Image flag;

    private void Awake()
    {
        instance = this;
    }

    private Dictionary<int, Division> divisionObjects = new Dictionary<int, Division>();
    private List<Division> divisionList = new List<Division>();
    private List<Tile> tileObjects = new List<Tile>();
    private List<int> changedTile = new List<int>();

    private Dictionary<int, LandBattle> landBattles = new Dictionary<int, LandBattle>();

    public static float hoursPerSecond;
    private float hourTimer;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsServerInitialized)
        {
            SendAllTileData();
        }
        else
        {
            RequestUpdateAllTiles(LocalConnection);
            RequestAllDivisionData(LocalConnection);
            RequestUpdateAllBattles(LocalConnection);
        }
    }

    private void NewHourTick()
    {
        gameTime.NextHour(out GameTime.TimeData time);
        SendTime(time);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameSaves.SaveGame();
            Debug.Log("Game Saved!");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameSaves.LoadGame();
            Debug.Log("Game Loaded!");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearDivisions();
            SendAllDivisionData();
        }

        if(hoursPerSecond > 0)
        {
            hourTimer -= Time.deltaTime;
            if (hourTimer < 0)
            {
                NewHourTick();
                hourTimer = 1 / hoursPerSecond;
            }
        }


    }

    // rpc functions

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveDivisions(List<int> divisionId, int tileId)
    {
        for (int i = 0; i < divisionId.Count; i++)
        {
            GetDivision(divisionId[i]).setTarget(GetTile(tileId));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnDivision(int tileId, int country)
    {
        SpawnDivision(tileId, country);
    }

    // whatever the server does

    public static void SpawnDivision(int tileId, int country)
    {
        int newKey = FindUnusedKey(instance.divisionObjects);

        Division division = new Division()
        {
            id = newKey,
        };

        Tile current = GetTile(tileId);
        division.SetCountry(CountryLoader.countries[country]);
        division.MoveToTile(current);

        instance.divisionObjects.Add(newKey, division);
        instance.divisionList.Add(division);

        SendDivisionUpdate(division);
    }

    public static void DespawnDivision(Division division)
    {
        instance.divisionObjects.Remove(division.id);
        instance.divisionList.Remove(division);

        SendDivisionUpdate(division);
    }

    public static void CreateLandBattle(LandBattle battle)
    {
        instance.landBattles.Add(battle.GetTile().id, battle);

        LandBattleData battleData = new LandBattleData()
        {
            tileId = battle.GetTile().id,
            battleProgress = battle.getBattleProgress()
        };


        instance.SendLandBattleUpdate(battleData);
    }

    public static void RemoveLandBattle(LandBattle battle)
    {
        instance.landBattles.Remove(battle.GetTile().id);

        LandBattleData battleData = new LandBattleData()
        {
            tileId = battle.GetTile().id,
            battleProgress = battle.getBattleProgress()
        };

        instance.RemoveLandBattleUpdate(battleData);
    }

    public static void LandBattleUpdate(LandBattle battle)
    {
        LandBattleData battleData = new LandBattleData()
        {
            tileId = battle.GetTile().id,
            battleProgress = battle.getBattleProgress()
        };

        instance.SendLandBattleUpdate(battleData);
    }

    /// <summary>
    /// Updates clients on division data
    /// </summary>
    /// <param name="division"></param>
    /// 

    // server updates time to clients

    [ServerRpc(RequireOwnership = false)]
    public void SendTime(GameTime.TimeData timeData)
    {
        RecieveTime(timeData);
    }

    [ObserversRpc(BufferLast = true)]
    public void RecieveTime(GameTime.TimeData timeData)
    {
        instance.gameTime.ReceiveTimeUpdate(timeData);
    }

    // client requested for tile data

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateAllTiles(NetworkConnection conn)
    {
        List<TileData> tileDatas = new List<TileData>();

        for (int i = 0; i < instance.tileObjects.Count; i++)
        {
            Tile[] tileNeighbors = instance.tileObjects[i].getNeighbours();
            int[] neighbors_ = new int[tileNeighbors.Length];

            for (int j = 0; j < tileNeighbors.Length; j++)
            {
                neighbors_[j] = tileNeighbors[j].id;
            }

            tileDatas.Add(new TileData()
            {
                id = i,
                neighbors = neighbors_,
                country = instance.tileObjects[i].GetCountry().id,
            });
        }

        ReceiveAllTileData(conn, tileDatas);
    }

    [TargetRpc]
    public void ReceiveAllTileData(NetworkConnection conn, List<TileData> datas)
    {
        VisualManager.UpdateTiles(datas);
    }

    // server pushes tile updates to all clients

    [ServerRpc(RequireOwnership = false)]
    public void SendAllTileData()
    {
        List<TileData> tileDatas = new List<TileData>();

        for (int i = 0; i < instance.tileObjects.Count; i++)
        {
            Tile[] tileNeighbors = instance.tileObjects[i].getNeighbours();
            int[] neighbors_ = new int[tileNeighbors.Length];

            for (int j = 0; j < tileNeighbors.Length; j++)
            {
                neighbors_[j] = tileNeighbors[j].id;
            }

            tileDatas.Add(new TileData()
            {
                id = i,
                neighbors = neighbors_,
                country = instance.tileObjects[i].GetCountry().id,
            });
        }

        ObserversRecieveAllTileData(tileDatas);
    }

    [ObserversRpc]
    public void ObserversRecieveAllTileData(List<TileData> datas)
    {
        VisualManager.UpdateTiles(datas);
    }

    //server pushes one tile update to all clients

    public static void SendTileData(Tile tile)
    {
        TileData tileData = new TileData();

        int[] neighbors = new int[tile.getNeighbours().Length];
        Tile[] tileNeighbors = tile.getNeighbours();
       
        for (int i = 0; i < tileNeighbors.Length; i++)
        {
            neighbors[i] = tileNeighbors[i].id;
        }

        tileData = new TileData()
        {
            id = tile.id,
            neighbors = neighbors,
            country = tile.GetCountry().id,
        };

        instance.ObserversRecieveTileData(tileData);
    }

    [ObserversRpc]
    public void ObserversRecieveTileData(TileData datas)
    {
        VisualManager.UpdateTile(datas);
    }

    // client requested for division data

    [ServerRpc(RequireOwnership = false)]
    public void RequestAllDivisionData(NetworkConnection conn)
    {
        List<DivisionData> datas = new List<DivisionData>();

        for (int i = 0; i < instance.divisionList.Count; i++)
        {
            datas.Add(CreateDivisionData(instance.divisionList[i]));
        }

        RecieveAllDivisionData(conn, datas);
    }

    [TargetRpc]
    public void RecieveAllDivisionData(NetworkConnection conn, List<DivisionData> datas)
    {
        VisualManager.ClearVisuals();

        for (int i = 0; i < datas.Count; i++)
        {
            VisualManager.UpdateDivision(datas[i]);
        }
    }

    // server pushes all division updates to all clients

    [ServerRpc(RequireOwnership = false)]
    public void SendAllDivisionData()
    {
        List<DivisionData> datas = new List<DivisionData>();

        for (int i = 0; i < instance.divisionList.Count; i++)
        {
            datas.Add(CreateDivisionData(instance.divisionList[i]));
        }

        ObserversRecieveAllDivisionData(datas);
    }

    [ObserversRpc]
    public void ObserversRecieveAllDivisionData(List<DivisionData> datas)
    {
        VisualManager.ClearVisuals();

        for (int i = 0; i < datas.Count; i++)
        {
            VisualManager.UpdateDivision(datas[i]);
        }
    }

    // server pushes a single division update for all clients

    public static void SendDivisionUpdate(Division division)
    {
        DivisionData updatedDivisionData = CreateDivisionData(division);
        instance.UpdateDivisionObservers(updatedDivisionData);
    }

    [ObserversRpc]
    public void UpdateDivisionObservers(DivisionData updatedDivisionData)
    {
        VisualManager.UpdateDivision(updatedDivisionData);
    }

    // server pushes all battle updates for a client

    [ServerRpc(RequireOwnership = false)]
    public void RequestUpdateAllBattles(NetworkConnection conn)
    {
        List<LandBattleData> battleDatas = new List<LandBattleData>();

        ICollection<LandBattle> battles = instance.landBattles.Values;

        foreach (LandBattle battle in battles)
        {
            battleDatas.Add(new LandBattleData()
            {
                tileId = battle.GetTile().id,
                battleProgress = battle.getBattleProgress(),
            });
        }

        instance.RecieveAllBattleUpdates(conn, battleDatas);
    }

    [TargetRpc]
    private void RecieveAllBattleUpdates(NetworkConnection conn, List<LandBattleData> landBattleDatas)
    {
        for (int i = 0; i < landBattleDatas.Count; i++)
        {
            VisualManager.UpdateBattle(landBattleDatas[i]);
        }
    }


    // server pushes a single battle update for all clients

    [ServerRpc(RequireOwnership = false)]
    public void SendLandBattleUpdate(LandBattleData battleData)
    {
        instance.UpdateLandBattleObservers(battleData);
    }

    [ObserversRpc]
    private void UpdateLandBattleObservers(LandBattleData updatedBattleData)
    {
        VisualManager.UpdateBattle(updatedBattleData);
    }

    // server removes a battle for all clients

    [ServerRpc(RequireOwnership = false)]
    public void RemoveLandBattleUpdate(LandBattleData battleData)
    {
        instance.RemoveLandBattleObservers(battleData);
    }

    [ObserversRpc]
    private void RemoveLandBattleObservers(LandBattleData updatedBattleData)
    {
        VisualManager.RemoveBattle(updatedBattleData);
    }

    // recieves tile data from tile loader
    public static void CollectTileData(List<Tile> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            instance.tileObjects.Add(tiles[i]);
        }
    }

    static int currentKey = 0;

    // Function to find an unused key
    static int FindUnusedKey(Dictionary<int, Division> dict)
    {
        //int key = 0;

        //// Loop to find an available key
        //while (dict.ContainsKey(key))
        //{
        //    key++;  // Increment key until we find an unused one

        //    if (key > 2000000)
        //    {
        //        Debug.LogError(("Max number of entities spawned"));
        //        return -1;
        //    }
        //}

        currentKey++;
        return currentKey;
    }

    public static DivisionData CreateDivisionData(Division division)
    {
        DivisionData newDivisionData = new DivisionData()
        {
            id = division.id,
            currentTileId = division.GetCurrentTile().id,
            targetTileId = division.GetTargetTile().id,
            progressPercent = division.GetProgressPercent(),
            country = division.GetCountry().id,

            orgPercent = (float)division.getOrgPercent(),
            healthPercent = (float)division.getHealthPercent(),
        };

        return newDivisionData;
    }

    public static void LoadDivisionButDontSendUpdate(DivisionData data)
    {
        int newKey = FindUnusedKey(instance.divisionObjects);

        Tile current = GetTile(data.currentTileId);
        Tile target = GetTile(data.targetTileId);

        float progressPercent = data.progressPercent;

        Country country = CountryLoader.countries[data.country];

        Division newDivision = new Division();
        newDivision.LoadData(newKey, target, current, progressPercent, country);

        instance.divisionObjects.Add(newKey, newDivision);
        instance.divisionList.Add(newDivision);
    }

    public static void ClearDivisions()
    {
        for (int i = 0; i < instance.divisionList.Count; i++)
        {
            instance.divisionObjects.Remove(instance.divisionList[i].id);
        }

        instance.divisionList.Clear();
    }

    public static List<Division> GetDivisions()
    {
        return instance.divisionList;
    }

    public static Division GetDivision(int divisionId)
    {
        return instance.divisionObjects[divisionId];
    }

    public static Tile GetTile(int tileId)
    {
        return instance.tileObjects[tileId];
    }
}

// this is a class for now maybe make a struct later
[System.Serializable]
public class DivisionData
{
    public int id;
    public int currentTileId;
    public int targetTileId;
    public float progressPercent;
    public int country;

    public float orgPercent;
    public float healthPercent;
}

[System.Serializable]
public class TileData
{
    public int id;
    public int[] neighbors;
    public int country;
}

[System.Serializable]
public class LandBattleData
{
    public int tileId;
    public int battleProgress;
}


