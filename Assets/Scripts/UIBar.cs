using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public float MaximumValue;
    public float CurrentValue;
    public float regenSpeed;

    public Image BarImage;

    public TMP_Text value;
    Coroutine Regen;

    public void AddValue(float Value)
    {
        CurrentValue = Mathf.Clamp(CurrentValue + Value, 0f, MaximumValue);
        UpdateUI();
    }

    public void ReduceValue(float Value)
    {
        CurrentValue = Mathf.Clamp(CurrentValue - Value, 0f, MaximumValue);
        UpdateUI();
        if(Regen != null)
        {
            return;
        }
        Regen = StartCoroutine(SelfFilling());
    }

    void UpdateUI()
    {
        float value = CurrentValue / MaximumValue;
        BarImage.fillAmount = value;
        this.value.text = ""+ (int)CurrentValue;
    }

    public IEnumerator SelfFilling()
    {
        while (CurrentValue < MaximumValue)
        {
            AddValue(regenSpeed);
            
            yield return null;
        }

        StopCoroutine(Regen);
        Regen = null;
        yield break;
    }
}
