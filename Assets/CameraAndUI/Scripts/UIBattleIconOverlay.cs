using System.Collections.Generic;
using UnityEngine;

public class UIBattleIconOverlay : MonoBehaviour
{
    [SerializeField] private GameObject battleIconPrefab;
    [SerializeField] private Transform battleIconParent;
    private Dictionary<int, UIBattleIcon> icons = new Dictionary<int, UIBattleIcon>();

    public static UIBattleIconOverlay instance;

    private void Awake()
    {
        instance = this;
    }

    public void AddBattleIcon(int battleProgress, int tileId)
    {
        GameObject newBattleIcon = Instantiate(battleIconPrefab, battleIconParent);
        Vector3 worldPosition = VisualManager.GetTileVisualPosition(tileId);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        newBattleIcon.transform.position = screenPosition;
        icons.Add(tileId, newBattleIcon.GetComponent<UIBattleIcon>());

        icons[tileId].SendBattleData(battleProgress, tileId);
    }

    public void UpdateBattleIcon(int battleProgress, int tileId)
    {
        if (icons.ContainsKey(tileId))
        {
            icons[tileId].SendBattleData(battleProgress, tileId);
        }
        else
        {
            AddBattleIcon(battleProgress, tileId);
        }
    }

    public void RemoveBattleIcon(int battleProgress, int tileId)
    {
        GameObject battleIcon = icons[tileId].gameObject;
        icons.Remove(tileId);
        print(battleIcon + " " + tileId);
        GameObject.Destroy(battleIcon);
        
    }
}
