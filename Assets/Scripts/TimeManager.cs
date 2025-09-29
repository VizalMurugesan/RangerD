using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public enum Timeoftheday { Morning,Evening,Night}
    Timeoftheday time = Timeoftheday.Morning;

    public enum ManagerState { Basking, Transitioning}
    ManagerState State = ManagerState.Basking;

    [SerializeField]float MorningDuration;
    [SerializeField]float EveningDuration;
    [SerializeField]float NightDuration;
    [SerializeField] float Transitionduration;

    [SerializeField] Sprite MorningSprite;
    [SerializeField] Sprite EveningSprite;
    [SerializeField] Sprite NightSprite;
    public TimeOfTheDay currentTimeOfTheDay;

    public Image image;
    [SerializeField] Image SpriteSpace;
    [SerializeField] TextMeshProUGUI text;
    Color imageColor;

    //TimeOfTheDay nextTime;
    float TotalTime = 0f;
    float currentTimeCurrentduration = 0f;
    float TransitionTimeCurrentduration = 0f;


    private void Awake()
    {
        TimeOfTheDay Morning = new TimeOfTheDay(MorningDuration, new Vector4(0f, 0f, 0f, 0f), Timeoftheday.Morning, MorningSprite);
        TimeOfTheDay Evening = new TimeOfTheDay(EveningDuration, new Vector4(100f/255f, 0f, 50f/255f, 150f / 255f), Timeoftheday.Evening, EveningSprite);
        TimeOfTheDay Night = new TimeOfTheDay(NightDuration, new Vector4(0f, 0f, 50f / 255f, 200f/255f), Timeoftheday.Night, NightSprite);
        Morning.SetNextTime(Evening);
        Evening.SetNextTime(Night);
        Night.SetNextTime(Morning);
        imageColor = image.color;
        currentTimeOfTheDay = Morning;

    }
    public void FixedUpdate()
    {
        if (State.Equals(ManagerState.Basking))
        {
            
            currentTimeCurrentduration += Time.fixedDeltaTime;
            if (currentTimeCurrentduration >= currentTimeOfTheDay.GetTimeDuration())
            {
                ChangeStateToTransitioning();
            }
        }
        else if (State.Equals(ManagerState.Transitioning))
        {
            
            Vector4 ColorVector = Vector4.Lerp(currentTimeOfTheDay.GetRGBval(),
            currentTimeOfTheDay.GetNextTimeOfTheDay().GetRGBval(), TransitionTimeCurrentduration / Transitionduration);

            imageColor.r = ColorVector.x; imageColor.g = ColorVector.y;
            imageColor.b = ColorVector.z; imageColor.a = ColorVector.w;
            image.color = imageColor;

            TransitionTimeCurrentduration += Time.fixedDeltaTime;

            if(TransitionTimeCurrentduration >= Transitionduration / 2)
            {
                ChangeSpriteAndTextToCurrent();
            }

            if(TransitionTimeCurrentduration>= Transitionduration)
            {
                ChangeStateToBasking();
            }
        }

        TotalTime += Time.fixedDeltaTime;
        
    }

    void ChangeToNextTime()
    {
        currentTimeOfTheDay = currentTimeOfTheDay.GetNextTimeOfTheDay();
        //currentTimeCurrentduration = 0f;
       
    }

    void ChangeStateToBasking()
    {
        State = ManagerState.Basking;
        ChangeToNextTime();
        SpriteSpace.sprite = currentTimeOfTheDay.GetSprite();
        text.text = time.ToString();
        TransitionTimeCurrentduration = 0f;
        currentTimeCurrentduration = 0f;
    }

    void ChangeSpriteAndTextToCurrent()
    {
        time = currentTimeOfTheDay.GetNextTimeOfTheDay().GetTime();
        SpriteSpace.sprite = currentTimeOfTheDay.GetNextTimeOfTheDay().GetSprite();
        text.text = time.ToString();
    }

    void ChangeStateToTransitioning()
    {
        
        State = ManagerState.Transitioning;
        TransitionTimeCurrentduration = 0f;
        currentTimeCurrentduration = 0f;
    }
}

public class TimeOfTheDay
{
    float duration;
    Vector4 RGBval;
    TimeManager.Timeoftheday time;
    TimeOfTheDay nextTime;
    Sprite sprite;

    public TimeOfTheDay(float duration, Vector4 RGBval, TimeManager.Timeoftheday time, Sprite sprite)
    {
        this.duration = duration;
        this.RGBval = RGBval;
        this.time = time;
        this.sprite = sprite;
    }

    public void SetNextTime(TimeOfTheDay nextTime)
    {
        this.nextTime = nextTime;
    }
    
    public TimeOfTheDay GetNextTimeOfTheDay()
    {
        return this.nextTime;
    }

    public TimeManager.Timeoftheday GetTime()
    {
        return this.time;
    }

    public float GetTimeDuration()
    {
        return this.duration;
    }

    public Vector4 GetRGBval()
    {
        return RGBval;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}

