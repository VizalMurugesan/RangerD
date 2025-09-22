using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 1f;
    public float currentHealth;
    public float droprate;

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
        gameObject.GetComponent<Character>().StopAllCoroutines();
        gameObject.GetComponent<Character>().SetCurrentpathReservedToFalse();
        gameObject.GetComponent<Enemy>().StopAllCoroutines();
        Game.Instance.levelManager.AddEXP(GetComponent<Enemy>().EXPtogive);
        if (Random.Range(0f, 1f) <= droprate)
        {
            Game.Instance.SpawnItem(transform.position);
        }
        gameObject.SetActive(false);
    }
}
