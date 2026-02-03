using UnityEngine;

public class PlayerResourceSystem : MonoBehaviour
{
    [Header("Bars")]
    public BarScript healthBar;
    public BarScript staminaBar;
    public BarScript manaBar;

    [Header("MaxStats")]
    public int maxHealth  = 100;
    public int maxStamina = 100;
    public int maxMana;
    
    [Header("Updated Stats")]
    public int currentHealth = 100;
    public int currentMana = 0;
    public int currentStamina = 100;
    public float staminaCooldown = 0.4f;

    private void Start()
    {
        SetupHealth();
        SetupMana();
        SetupStamina();
    }
    //Health
    void SetupHealth()
    {
        healthBar.SetMaxValue(maxHealth);
        healthBar.SetValue(currentHealth);
        healthBar.SetText($"{maxHealth.ToString()} / {currentHealth.ToString()}");
    }
    public void Takedamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetValue(currentHealth);
        healthBar.SetText($"{maxHealth.ToString()} / {currentHealth.ToString()}");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player is Dead");
        }   
    }
    public void GiveHealth(int health)
    {
        if (currentHealth + health >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += health;
        }
        healthBar.SetValue(currentHealth);
        healthBar.SetText($"{maxHealth.ToString()} / {currentHealth.ToString()}");
    }
    public void ChangeMaxHealth(int maxHealthMod)
    {
        int tempMaxH;
        if (currentHealth > maxHealth + maxHealthMod)
        { 
            tempMaxH = maxHealth + maxHealthMod;
        }
        else
        {
            tempMaxH = Mathf.Abs(maxHealth - maxHealthMod);
        }
        maxHealth += maxHealthMod;
        healthBar.SetValue(tempMaxH);
        healthBar.SetText($"{maxHealth.ToString()} / {currentHealth.ToString()}");

    }

    //Stamina
    void SetupStamina()
    {
        staminaBar.SetMaxValue(maxStamina);
        staminaBar.SetValue(currentStamina);
        staminaBar.SetText($"{maxStamina.ToString()} / {currentStamina.ToString()}");
    }
    public void TakeStamina(int amount)
    {
        currentStamina -= amount;
    }

    //Mana
    void SetupMana()
    {
        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(currentMana);
        manaBar.SetText($"{maxMana.ToString()} / {currentMana.ToString()}");
    }
    public void UseMana(int amount)
    {
        currentMana -= amount;
        manaBar.SetValue(currentMana);
        manaBar.SetText($"{maxMana.ToString()} / {currentMana.ToString()}");
        if(currentMana - amount <= 0)
        {
            Debug.Log("Mana Depleted");
        }
    }
    public void AddMana(int amount)
    {
        if (currentMana + amount >= maxMana)
        {
            currentMana = maxMana;
        }
        else
        {
            currentMana += amount;
        }
        manaBar.SetValue(currentMana);
        manaBar.SetText($"{maxMana.ToString()} / {currentMana.ToString()}");
    }
    public void ChangeMaxMana(int maxManaMod)
    {
        int tempMaxM;
        if (currentMana > maxMana + maxManaMod)
        {
            tempMaxM = maxMana + maxManaMod;
        }
        else
        {
            tempMaxM = Mathf.Abs(maxMana - maxManaMod);
        }
        maxMana += maxManaMod;
        manaBar.SetValue(tempMaxM);
        manaBar.SetText($"{maxMana.ToString()} / {currentMana.ToString()}");
    }
}
