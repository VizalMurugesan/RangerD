using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 1f;
    public float currentHealth;

    public Image healthBar;
    void UpdateUI()
    {
        healthBar.fillAmount = currentHealth / MaxHealth;

    }
    public void AddHealth(float Value)
    {
        currentHealth = Mathf.Clamp(currentHealth + Value, 0f, MaxHealth);
        UpdateUI();
    }

    public void TakeDamage(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth - value, 0f, MaxHealth);

        if (currentHealth.Equals(0f))
        {
            Die();
        }
        
        UpdateUI();
    }

    void setHpToMax()
    {
        currentHealth = MaxHealth;
        UpdateUI();
    }

    void Die()
    {
        Game.Instance.levelManager.AddEXP(GetComponent<Enemy>().EXPtogive);
        gameObject.SetActive(false);
    }
}
