using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIIconGroup : MonoBehaviour
{
    public int visualId;
    [SerializeField] private GameObject iconPrefab;
    public List<GameObject> shownIcons;
    private List<int> divisionIds;

    private List<CountryDivisionGroup> countryDivisionGroups = new List<CountryDivisionGroup>();

    void Start()
    {
        PlayerInteraction.OnSelectedDivisionChanged += PlayerInteraction_OnSelectedDivisionChanged;
    }
    private void OnDestroy()
    {
        PlayerInteraction.OnSelectedDivisionChanged -= PlayerInteraction_OnSelectedDivisionChanged;
    }

    private void PlayerInteraction_OnSelectedDivisionChanged(object sender, SelectedDivisionArgs e)
    {
        DrawAllIcons();
    }

    private void DrawAllIcons()
    {
        DestroyAllIcons();

        List<int> allSelectedDivisions = PlayerInteraction.instance.GetDivisionsSelected();
        countryDivisionGroups.Clear();

        //make lists of division ids per country

        for (int i = 0; i < divisionIds.Count; i++)
        {
            int country = VisualManager.GetDivisionData(divisionIds[i]).country;
            bool inList = false;

            // if country already in list
            for (int j = 0; j < countryDivisionGroups.Count; j++)
            {
                if (countryDivisionGroups[j].country == country)
                {
                    countryDivisionGroups[j].divisions.Add(divisionIds[i]);
                    inList = true;
                    break;
                }
            }

            // not in list
            if (!inList)
            {
                countryDivisionGroups.Add(new CountryDivisionGroup()
                {
                    divisions = new List<int>() {
                        divisionIds[i]
                    },
                    country = country
                });
            }
        }

        for (int i = 0; i < countryDivisionGroups.Count; i++)
        {
            List<int> unselectedDivisions = new List<int>();
            List<int> selectedDivisions = new List<int>();

            // add all divisions in this country to list
            for (int j = 0; j < countryDivisionGroups[i].divisions.Count; j++)
            {
                unselectedDivisions.Add(countryDivisionGroups[i].divisions[j]);
            }

            // remove division from unselected divions if selected and add to selected divisions
            for (int j = 0; j < allSelectedDivisions.Count; j++)
            {
                int divisionId = allSelectedDivisions[j];

                if (unselectedDivisions.Contains(divisionId))
                {
                    selectedDivisions.Add(divisionId);
                    unselectedDivisions.Remove(divisionId);
                }
            }

            if (unselectedDivisions.Count > 1) // draw unselected divisions first
            {
                //draw unselected divisions

                CreateIcon(unselectedDivisions, false);

                //draw selected divisions

                for (int j = 0; j < selectedDivisions.Count; j++)
                {
                    CreateIcon(new List<int>() { unselectedDivisions[j] }, true);
                }
            }
            else // dont draw unselected divisions first
            {
                //draw all divisions

                for (int j = 0; j < countryDivisionGroups[i].divisions.Count; j++)
                {
                    if (selectedDivisions.Contains(countryDivisionGroups[i].divisions[j]))
                    {
                        CreateIcon(new List<int>() { countryDivisionGroups[i].divisions[j] }, true);
                    }
                    else
                    {
                        CreateIcon(new List<int>() { countryDivisionGroups[i].divisions[j] }, false);
                    }
                }
            }
        }



    }

    [System.Serializable]
    private class CountryDivisionGroup
    {
        public int country;
        public List<int> divisions;
    }

    public void CreateIcon(List<int> newIconDivisions, bool highlight)
    {
        GameObject newIconObject = Instantiate(iconPrefab, transform);
        UIDivisionIcon newIcon = newIconObject.GetComponent<UIDivisionIcon>();
        newIcon.SendDivisionData(newIconDivisions);
        newIcon.SetIconHighlight(highlight);

        DivisionData data = VisualManager.GetDivisionData(newIconDivisions[0]);
        Vector3 divisionPosition = VisualManager.CalculateVisualPosition(data.currentTileId, data.targetTileId, out Quaternion rotation);

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(divisionPosition + Vector3.down * 0.4f + Vector3.back * 0.3f);
        transform.position = screenPosition;

        shownIcons.Add(newIconObject);
    }

    public void DeleteIcon(int iconIndex)
    {
        GameObject iconToDestroy = shownIcons[iconIndex];
        shownIcons.RemoveAt(iconIndex);
        Destroy(iconToDestroy);
    }

    public void SetDivisions(List<int> divisionIds_)
    {
        divisionIds = divisionIds_;
        DrawAllIcons();
    }

    public void DestroyAllIcons()
    {
        for (int i = 0; i < shownIcons.Count; i++)
        {
            Destroy(shownIcons[i]);
        }

        shownIcons.Clear();
    }
}
