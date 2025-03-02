using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CountryLoader : MonoBehaviour
{
    [SerializeField] private TileLoader tileLoader;
    public static List<Country> countries;

    public static Country nan = new Country(null, Color.black, "none", "NAN", -1);

    private String COUNTRY_JSON_PATH;
    private string FLAGS_PATH = "Assets/Resources/Countries/flags";

    private TextAsset COUNTRY_JSON;

    private double GetColorDiff(Color original, Color test)
    {
        return Math.Pow(original.r - test.r, 2) + Math.Pow(original.g - test.g, 2) + Math.Pow(original.b - test.b, 2);
    }

    public Country GetCountryByColor(Color color)
    {
        double minDiff = Double.MaxValue;
        Country bestFit = nan;

        if(color == Color.black) { return bestFit; }

        foreach (Country country in countries)
        {
            Debug.Log("Loaded Country "+country.GetName());
            double colorDiff = GetColorDiff(country.GetColor(), color);
            if (colorDiff < minDiff)
            {
                minDiff = colorDiff;
                bestFit = country;
            }
        }
        return bestFit;
    }

    public void LoadCountries()
    {
        COUNTRY_JSON_PATH = "Countries" +
                    Path.DirectorySeparatorChar + "countries";
        COUNTRY_JSON = Resources.Load<TextAsset>(COUNTRY_JSON_PATH);

        if (COUNTRY_JSON == null)
        {
            Debug.Log(COUNTRY_JSON_PATH+ " doesnt exist!");
            countries = new List<Country>();
            return;
        }
        countries = new List<Country>();
        try
        {
            CountryListWrapper wrapper = JsonUtility.FromJson<CountryListWrapper>(COUNTRY_JSON.text);
            countries = new List<Country>();
            foreach (CountryJson country in wrapper.countries)
            {
                Color color;
                ColorUtility.TryParseHtmlString(country.color, out color);
                Texture2D flagTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(FLAGS_PATH + "/" + country.flagPath);
                countries.Add(new Country(flagTexture, color, country.name, country.tag, countries.Count));
                print(country.name);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        //countries.Add(nan);
    }
    
    [Serializable]
    private class CountryListWrapper
    {
        public List<CountryJson> countries;
    }
    [Serializable]
    private class CountryJson
    {
        public string name;
        public string tag;
        public string color;
        public string flagPath;
    }
}