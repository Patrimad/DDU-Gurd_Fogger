using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    public Slider slider;
    public Text value;


    public void SetValue(int amount)
    {
        slider.value = amount;
    }

    public void SetMaxValue(int amount)
    {
        slider.maxValue = amount;
    }

    public void SetText(string text)
    {
        value.text = text;
    }

}
