using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleIcon : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI amount;
    int tile;

    public void SendBattleData(int battleProgress, int tile_)
    {
        amount.text = battleProgress.ToString();
        tile = tile_;
    }

    public void Update()
    {
        Vector3 worldPosition = VisualManager.GetTileVisualPosition(tile);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition;

    }
}
