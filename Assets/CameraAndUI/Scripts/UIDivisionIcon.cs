using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDivisionIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image flag;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image orgBar;
    [SerializeField] private TextMeshProUGUI divisionCount;
    private List<int> divisions = new List<int>();


    public void OnButtonPress()
    {
        PlayerInteraction.instance.SelectDivisions(divisions);
    }

    public void SetIconHighlight(bool on)
    {
        image.color = on ? Color.yellow : Color.white;
    }

    public void SendDivisionData(List<int> divisions_)
    {
        divisions.Clear();
        divisions.AddRange(divisions_);

        divisionCount.text = divisions.Count.ToString();

        DivisionData firstDivision = VisualManager.GetDivisionData(divisions[0]);

        Texture2D flagTexture = CountryLoader.countries[firstDivision.country].GetFlag();
        Sprite sprite = Sprite.Create(flagTexture, new Rect(0, 0, flagTexture.width, flagTexture.height), new Vector2(0.5f, 0.5f));
        flag.sprite = sprite;

        // healh bars

        float averageHealth = 0;
        float averageOrg = 0;

        for (int i = 0; i < divisions.Count; i++)
        {
            DivisionData data = VisualManager.GetDivisionData(divisions[i]);

            averageHealth += data.healthPercent;
            averageOrg += data.orgPercent;
        }

        averageHealth = averageHealth / divisions.Count;
        averageOrg = averageOrg / divisions.Count;

        healthBar.fillAmount = averageHealth;
        orgBar.fillAmount = averageOrg;
    }
}
