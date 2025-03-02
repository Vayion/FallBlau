using TMPro;
using UnityEngine;

public class UICountryInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countryNameText;
    public GameObject parentObject;

    private void Start()
    {
        parentObject.SetActive(false);
    }

    public void UpdateCountryInfo(Country country)
    {
        countryNameText.text = country.GetName();
    }

    public void Hide()
    {
        parentObject.SetActive(false);
    }
}
