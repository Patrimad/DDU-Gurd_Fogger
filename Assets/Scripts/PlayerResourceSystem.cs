using UnityEngine;
using static FPSControllerWithStates;

public class PlayerResourceSystem : MonoBehaviour
{
    [Header("Bars")]
    public BarScript healthBar;
    public BarScript staminaBar;
    public BarScript manaBar;

    [Header("MaxStats")]
    public int maxHealth  = 100;
    public float maxStamina = 100f;
    public int maxMana = 0;
    
    [Header("Updated Stats")]
    public int currentHealth = 100;
    public float currentStamina = 100f;
    public int currentMana = 0;
    
    public float staminaCooldown = 0.4f;
    public float staminaDrainPerSecond = 15;
    public float staminaRegenPerSecond = 10;
    public float staminaRegenTimer;

    

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
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        healthBar.SetValue(currentHealth);
        healthBar.SetText($"{currentHealth} / {maxHealth}");

        if (currentHealth == 0)
        {
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
        maxHealth += maxHealthMod;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        healthBar.SetMaxValue(maxHealth);
        healthBar.SetValue(currentHealth);
        healthBar.SetText($"{currentHealth} / {maxHealth}");

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
        currentStamina = Mathf.Max(currentStamina - amount, 0);
        staminaBar.SetValue(currentStamina);
        staminaBar.SetText($"{currentStamina} / {maxStamina}");
    }
    public void DrainStamina(float deltaTime)
    {
        currentStamina -= staminaDrainPerSecond * deltaTime;
        currentStamina = Mathf.Max(currentStamina, 0f);

        staminaRegenTimer = staminaCooldown;

        UpdateStaminaUI();
    }

    public void RegenerateStamina(float deltaTime)
    {
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= deltaTime;
            return;
        }

        currentStamina += staminaRegenPerSecond * deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        UpdateStaminaUI();
    }

    public bool HasStamina()
    {
        return currentStamina > 0f;
    }

    private void UpdateStaminaUI()
    {
        staminaBar.SetValue(currentStamina);
        staminaBar.SetText($"{Mathf.CeilToInt(currentStamina)} / {Mathf.CeilToInt(maxStamina)}");
    }

    //Mana
    void SetupMana()
    {
        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(currentMana);
        manaBar.SetText($"{currentMana} / {maxMana}");
    }
    public bool UseMana(int amount)
    {
        if (maxMana <= 0 || currentMana < amount)
            return false;

        currentMana -= amount;

        manaBar.SetValue(currentMana);
        manaBar.SetText($"{currentMana} / {maxMana}");

        if (currentMana == 0)
            Debug.Log("Mana Depleted");

        return true;
    }
    public void AddMana(int amount)
    {
        if (maxMana <= 0)
            return;

        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);

        manaBar.SetValue(currentMana);
        manaBar.SetText($"{currentMana} / {maxMana}");
    }
    public void ChangeMaxMana(int amount)
    {
        maxMana += amount;
        maxMana = Mathf.Max(maxMana, 0);

        currentMana = Mathf.Clamp(currentMana, 0, maxMana);

        manaBar.SetMaxValue(maxMana);
        manaBar.SetValue(currentMana);
        manaBar.SetText($"{currentMana} / {maxMana}");
    }
}
