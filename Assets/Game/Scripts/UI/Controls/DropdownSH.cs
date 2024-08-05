using FMODUnity;
using UnityEngine.UIElements;

public class DropdownSH : DropdownField
{
    public new class UxmlFactory : UxmlFactory<DropdownSH, UxmlTraits> { }

    public DropdownSH()
    {
        this.RegisterCallback<ClickEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent)
            );
        this.RegisterCallback<MouseOverEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.hoverEvent)
            );
        this.RegisterCallback<ChangeEvent<int>>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.sliderChangeEvent)
            );
    }
}