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
        
        for (float t = 0; t < InvSpeed; t += Time.deltaTime)
        {
            float progress = t / InvSpeed;

            textcolor.a = (1 - progress) * DefaultAlpha;
            text.color = textcolor;

            transform.position = Vector3.Lerp(SpawnPos.position, EndPos.position, progress);

            yield return null;
        }
        textcolor.a = 0f;
        text.color = textcolor;
        transform.position = EndPos.position;
        gameObject.SetActive(false);
        yield break;
    }
}
