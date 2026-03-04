using UnityEngine;

public class Menu : MonoBehaviour
{

    public string menuName;
    public bool open; 
    public void Open()
    {
        open = true; // Hvis menu open kaldes, så er open = true
        gameObject.SetActive(true); // Og så er menuen synliggjort
    }

    public void Close ()
    {
        open = false; // Menuen sættes til open = false
        gameObject.SetActive(false); // Og den bliver usynliggjort
    }
}
