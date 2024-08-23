using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class IntroMenu : Menu
{
    private Label intro1;
    private Label intro2;

    private void Start()
    {
        intro1 = root.Q<Label>("Intro1");
        intro2 = root.Q<Label>("Intro2");
        intro1.AddToClassList("hidden");
        intro2.AddToClassList("hidden");
        intro2.style.display = DisplayStyle.None;
    }

    public override void Show()
    {
        base.Show();
        StartCoroutine(RunIntroSequence());
    }

    IEnumerator RunIntroSequence()
    {
        yield return new WaitForSecondsRealtime(2.0f);
        intro1.RemoveFromClassList("hidden");
        yield return new WaitForSecondsRealtime(5.0f);
        intro1.AddToClassList("hidden");
        yield return new WaitForSecondsRealtime(1.0f);
        intro1.style.display = DisplayStyle.None;
        intro2.style.display = DisplayStyle.Flex;
        yield return new WaitForSecondsRealtime(1.0f);

        intro2.RemoveFromClassList("hidden");
        yield return new WaitForSecondsRealtime(5.0f);
        intro2.AddToClassList("hidden");
        yield return new WaitForSecondsRealtime(1.0f);
        intro2.style.display = DisplayStyle.None;


        CameraManager.Instance.SmoothSetVignette(1.0f, 0.0f, 5.0f);
        CameraManager.Instance.SmoothSetBlur(15.0f, 0.0f, 5.0f);
        CameraManager.Instance.SmoothSetSaturation(-100, 8, 5.0f);
        yield return FadeOut(1.0f);
    }
}
