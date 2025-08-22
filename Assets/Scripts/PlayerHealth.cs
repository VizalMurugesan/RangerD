using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 1f;
    public float currentHealth;

    public Image healthBar;

    public float CooldownTime;

    public float healingSpeed;

    Coroutine SelfHeal;

    public TMP_Text Value;

    public void AddHealth(float Value)
    {
        currentHealth = Mathf.Clamp(currentHealth+ Value, 0f, MaxHealth);
        UpdateUI();
    }

    public void TakeDamage(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth-value, 0f, MaxHealth);
        UpdateUI();
        if(SelfHeal != null)
        {
            StopCoroutine(SelfHeal); 
        }
        SelfHeal = StartCoroutine(SelfHealingWait());

    }

    void UpdateUI()
    {
        healthBar.fillAmount = currentHealth / MaxHealth;
        Value.text = "" + (int)currentHealth;
    }

    public IEnumerator SelfHealingWait()
    {
        float CurrTime = 0f;
        while (CurrTime < CooldownTime)
        {
            CurrTime+= Time.deltaTime;
            yield return null;
        }
        SelfHeal = StartCoroutine(SelfHealing());
    }

    public IEnumerator SelfHealing()
    {
        while(currentHealth< MaxHealth)
        {
            AddHealth(healingSpeed);
            yield return null;
        }
    }

}
