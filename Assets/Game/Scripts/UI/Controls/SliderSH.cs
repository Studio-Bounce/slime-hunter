using FMODUnity;
using UnityEngine.UIElements;

public class SliderSH : Slider
{
    public new class UxmlFactory : UxmlFactory<SliderSH, UxmlTraits> { }

    public SliderSH()
    {
        this.RegisterCallback<ClickEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent)
        );
        this.RegisterCallback<MouseOverEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.hoverEvent)
        );
        this.RegisterCallback<ChangeEvent<float>>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.sliderChangeEvent)
        );
    }
}
