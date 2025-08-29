using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour
{
    public int CurrentLevel = 1;
    public int MaxLevel = 50;
    public float EXPToNextLevel = 100f;
    public float CurrentExp = 0f;
    public float ExpNeededMultiplier = 1.2f;
    public Image BarImage;

    public TMP_Text value;
    
    public void AddEXP(float value)
    {
        if(!(CurrentLevel<MaxLevel)) return;
        CurrentExp += value;
        if(CurrentExp >= EXPToNextLevel)
        {
            Promote();
            return;
        }

        UpdateUI();

    }
    public void Promote()
    {
        CurrentLevel= Mathf.Clamp(CurrentLevel+1, 0, MaxLevel);
        CurrentExp %= EXPToNextLevel;
        EXPToNextLevel *= ExpNeededMultiplier;

        UpdateUI();

        Game.Instance.EnableNoticePanel("LEVEL UP!");
    }

    void UpdateUI()
    {
        BarImage.fillAmount = CurrentExp / EXPToNextLevel;
        value.text = CurrentLevel.ToString();
    }
}
