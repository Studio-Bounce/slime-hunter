using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFill : MonoBehaviour
{
    public Image fill;

    public void ResetFill()
    {
        fill.fillAmount = 1;
    }

    public void ClearFill()
    {
        fill.fillAmount = 0;
    }

    public void SetFill(float amount)
    {
        fill.fillAmount = amount;
    }
}
