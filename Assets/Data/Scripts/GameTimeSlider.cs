using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeSlider : MonoBehaviour
{

    // 0 hours per tick
    
    private float minSpeed = 0f; 
    private float maxSpeed = 24f;

    private Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        if (slider != null)
        {
            slider.minValue = minSpeed;
            slider.maxValue = maxSpeed;

            slider.value = 0;
            
            slider.onValueChanged.AddListener(input =>
            {
                TheGameManager.hoursPerSecond = input;
            });
        }
    }

}
