using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 1f;
    public float currentHealth;
    public float droprate;

    public Image healthBar;
    Enemy enemy;

    public void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public void AddHealth(float Value)
    {
        currentHealth = Mathf.Clamp(currentHealth + Value, 0f, MaxHealth);
        
    }

    public void TakeDamage(float value)
    {
        currentHealth = Mathf.Clamp(currentHealth - value, 0f, MaxHealth);

        if (currentHealth.Equals(0f))
        {
            Die();
        }
        else
        {
            StartCoroutine(ChangeColorToRed());
        }
        
        
    }

    public void setHpToMax()
    {
        currentHealth = MaxHealth;
        
    }

    void Die()
    {
        gameObject.GetComponent<Character>().StopAllCoroutines();
        gameObject.GetComponent<Character>().SetCurrentpathReservedToFalse();
        enemy.StopAllCoroutines();
        enemy.Die();
        Game.Instance.levelManager.AddEXP(GetComponent<Enemy>().EXPtogive);
        if (Random.Range(0f, 1f) <= droprate)
        {
            Game.Instance.SpawnItem(transform.position);
        }
        
    }

    IEnumerator ChangeColorToRed()
    {
        List<SpriteRenderer> renderers = GetComponent<Character>().renderers;
        foreach (var rend in renderers)
        {
            rend.color = new Vector4(1f, 0f, 0f, 1f);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var rend in renderers)
        {
            rend.color = new Vector4(1f, 1f, 1f, 1f);
        }
        yield break;
    }
}
