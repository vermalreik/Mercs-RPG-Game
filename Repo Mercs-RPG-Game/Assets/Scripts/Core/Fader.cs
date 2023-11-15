using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader i { get; private set; }
    Image image;

    private void Awake()
    {
        i = this;
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time) // time in which we should complete the fading
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time) // time in which we should complete the fading
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
}
