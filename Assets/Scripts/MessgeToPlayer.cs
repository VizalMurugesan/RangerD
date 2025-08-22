using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessgeToPlayer : MonoBehaviour
{
    [SerializeField] private RectTransform SpawnPos;
    [SerializeField] private RectTransform EndPos;
    [SerializeField] private float InvSpeed;

    public TMP_Text text;
    Color textcolor;
    

    float DefaultAlpha;
    public void Awake()
    {
        text = GetComponent<TMP_Text>();
        DefaultAlpha = 220f; ;
        textcolor = text.color;
    }
    public IEnumerator MoveUp(Vector3 MessageColor, string message)
    {
        text.text = message;
        textcolor.r = MessageColor.x; textcolor.g = MessageColor.y; textcolor.b = MessageColor.z;
        textcolor.a = DefaultAlpha;
        text.color = textcolor;
        transform.position = SpawnPos.transform.position;
        float t = 0f;
        while(t< InvSpeed)
        {
            textcolor.a = (1 - (t / InvSpeed)) * DefaultAlpha;
            text.color = textcolor;
            transform.position = Vector3.Lerp((Vector3)SpawnPos.transform.position, (Vector3)EndPos.transform.position, t / InvSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        yield break;
    }
}
