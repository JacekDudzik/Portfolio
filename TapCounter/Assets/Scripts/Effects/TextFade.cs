using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextFade : MonoBehaviour
{
    public float fadeTime;
    public bool deleteOnFinish;

    Text text;

    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(Fade());
    }
    IEnumerator Fade()
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;

        while (color.a != 1)
        {
            color.a = Mathf.MoveTowards(color.a, 1, 2 / fadeTime * Time.deltaTime);
            text.color = color;
            yield return new WaitForEndOfFrame();
        }
        while (color.a != 0)
        {
            color.a = Mathf.MoveTowards(color.a, 0, 2 / fadeTime * Time.deltaTime);
            text.color = color;
            yield return new WaitForEndOfFrame();
        }
        if (deleteOnFinish)
        {
            Destroy(gameObject);
        }
    }
}
