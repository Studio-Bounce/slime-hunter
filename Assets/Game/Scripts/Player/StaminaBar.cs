using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Notification
{
    public Player player;
    public Image fill;

    public float visibleDuration;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canvasGroup.alpha = 0;
        if (player != null)
        {
            CanvasManager.Instance.AddAnchoredElement(player.transform, rectTransform.GetComponent<RectTransform>(), new Vector2(-70, 100));
        }
        GameManager.Instance.OnPlayerStaminaChange += value => SetFill((float)value / GameManager.Instance.PlayerMaxStamina);
    }

    public void SetFill(float amount)
    {
        fill.fillAmount = amount;
        PlayInOut(visibleDuration);
    }

}
