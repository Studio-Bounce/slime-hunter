using FMODUnity;
using UnityEngine.UIElements;

public class ButtonSH : Button
{
    public new class UxmlFactory : UxmlFactory<ButtonSH, UxmlTraits> { }

    public ButtonSH()
    {
        this.RegisterCallback<ClickEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent)
            );
        this.RegisterCallback<MouseOverEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.hoverEvent)
            );
        this.RegisterCallback<FocusEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.hoverEvent)
        );
    }
}