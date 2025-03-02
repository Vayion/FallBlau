using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIDivisionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField countryInput;
    [SerializeField] private TextMeshProUGUI selectedTileText;
    private Country countrySelected;
    private int tileSelected;
    public GameObject parentObject;

    bool selectTileMode;

    void Start()
    {
        countryInput.onValueChanged.AddListener(OnTextChanged);
        countryInput.contentType = TMP_InputField.ContentType.IntegerNumber;
        PlayerInteraction.OnTileSelect += PlayerInteraction_OnTileSelect;
    }

    private void PlayerInteraction_OnTileSelect(object sender, SelectedTileArgs e)
    {
        print("e");
        if (selectTileMode)
        {
            SelectTile(e.selectedTileId);
        }

    }

    private void OnTextChanged(string newText)
    {
        int countryId = int.Parse(newText);

        if(countryId < CountryLoader.countries.Count)
        {
            countrySelected = CountryLoader.countries[countryId];
        }
    }

    public void UpdateInfo(Country country)
    {
        countrySelected = country;
        countryInput.text = country.id.ToString();
    }

    public void StartSelectTileMode()
    {
        PlayerInteraction.cantInteract = true;
        selectTileMode = true;
    }

    public void SpawnDivision()
    {
        TheGameManager.instance.RequestSpawnDivision(tileSelected, countrySelected.id);
    }

    private void StopSelectTileMode()
    {
        PlayerInteraction.cantInteract = false;
        selectTileMode = false;
    }

    private void SelectTile(int tile)
    {
        tileSelected = tile;
        selectedTileText.text = tile.ToString();
        StopSelectTileMode();
    }

    public void Hide()
    {
        SelectTile(0);
        parentObject.SetActive(false);

    }
}
