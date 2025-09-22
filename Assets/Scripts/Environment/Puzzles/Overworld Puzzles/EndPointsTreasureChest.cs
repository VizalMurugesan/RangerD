using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndPointsTreasureChest : MonoBehaviour
{
    public bool fullfilled = false;
    public Image image;
    [SerializeField] float duration = 1f;
    Coroutine coroutine;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TreasureChestStuff") && !fullfilled)
        {
            SetFullFillToTrue();
        }
    }

    IEnumerator FillImage()
    {
        while (image.fillAmount < 1f)
        {
            image.fillAmount += Time.deltaTime / duration;
            yield return null;
        }
    }
    IEnumerator UnfillImage()
    {
        while (image.fillAmount > 0f)
        {
            image.fillAmount -= Time.deltaTime / duration;
            yield return null;
        }
    }

    public void SetFullFillToFalse()
    {
        fullfilled = false;
        if(coroutine!= null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(UnfillImage());
    }

    public void SetFullFillToTrue()
    {
        fullfilled = true;
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(FillImage());
    }
}
