using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI value;


    public void SetValue(int amount)
    {
        slider.value = amount;
    }
    public void SetValue(float amount)
    {
        slider.value = amount;
    }

    public void SetMaxValue(int amount)
    {
        slider.maxValue = amount;
    }

    public void SetMaxValue(float amount)
    {
        slider.maxValue = amount;
    }

    public void SetText(string text)
    {
        value.text = text;
    }

}
