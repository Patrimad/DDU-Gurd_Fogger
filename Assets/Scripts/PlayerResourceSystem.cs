using UnityEngine;

public class PlayerResourceSystem : MonoBehaviour
{
    [Header("MaxStats")]
    public int maxHealth  = 100;
    public int maxStamina = 100;
    public int maxMana;
    
    [Header("Updated Stats")]
    public int health = 100;
    public int mana = 0;
    public int stamina = 100;
    
    
    public void Takedamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }   
    }

    public void TakeStamina(int amount)
    {
        stamina -= amount;
    }

    public void UseMana(int amount)
    {
        mana -= amount;
    }
}
