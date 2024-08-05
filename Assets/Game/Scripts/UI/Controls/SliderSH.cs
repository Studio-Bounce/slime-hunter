using FMODUnity;
using UnityEngine.UIElements;

public class SliderSH : Slider
{
    private bool isInitialized = false;

    public new class UxmlFactory : UxmlFactory<SliderSH, UxmlTraits> { }

    public SliderSH()
    {
        this.RegisterCallback<ClickEvent>(
            ev => RuntimeManager.PlayOneShot(AudioManager.Config.buttonPressEvent)
        );
        this.RegisterCallback<ChangeEvent<float>>(
            ev => { if (isInitialized) RuntimeManager.PlayOneShot(AudioManager.Config.sliderChangeEvent); }
        );

        // Avoid changeEvent occuring on game start
        this.RegisterCallback<GeometryChangedEvent>(ev =>
        {
            if (!isInitialized)
            {
                isInitialized = true;
            }
        });
    }
}
