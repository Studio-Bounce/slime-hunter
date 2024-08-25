using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class IntroMenu : Menu
{
    private Label intro1;
    private Label intro2;
    InputAction inputAction;

    private void Start()
    {
        intro1 = root.Q<Label>("Intro1");
        intro2 = root.Q<Label>("Intro2");
        intro1.AddToClassList("hidden");
        intro2.AddToClassList("hidden");
        inputAction = InputManager.Instance.StringToAction("Select");
    }

    private void Skip(InputAction.CallbackContext ctx)
    {
        StopAllCoroutines();
        StartCoroutine(EndIntroSequence());
    }

    public override void Show()
    {
        base.Show();
        inputAction.performed += Skip;
        intro1.AddToClassList("hidden");
        intro2.AddToClassList("hidden");
        intro1.style.display = DisplayStyle.Flex;
        intro2.style.display = DisplayStyle.None;
        StartCoroutine(RunIntroSequence());
    }

    IEnumerator RunIntroSequence()
    {
        InputManager.Instance.TogglePlayerControls(false);

        yield return new WaitForSecondsRealtime(2.0f);
        intro1.RemoveFromClassList("hidden");
        yield return new WaitForSecondsRealtime(8.0f);
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

        yield return EndIntroSequence();
    }

    IEnumerator EndIntroSequence()
    {
        inputAction.performed -= Skip;
        InputManager.Instance.TogglePlayerControls(true);
        CameraManager.Instance.SmoothSetVignette(1.0f, 0.0f, 5.0f);
        CameraManager.Instance.SmoothSetBlur(15.0f, 0.0f, 4.0f);
        CameraManager.Instance.SmoothSetSaturation(-100, 8, 5.0f);
        yield return FadeOut(1.0f);
        AudioManager.Instance.ExplorationInstance.setParameterByName("Intro", 0);
    }
}
