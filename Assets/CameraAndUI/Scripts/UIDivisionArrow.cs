using UnityEngine;
using UnityEngine.UI;

public class UIDivisionArrow : MonoBehaviour
{
    public int divisionId;
    [SerializeField] private Image bar;

    public void SetFill(float value)
    {
        bar.fillAmount = value;
    }
}
