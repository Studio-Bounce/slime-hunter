using FMODUnity;
using System.Collections;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [Tooltip("Layermask used for hit detection")]
    public LayerMask comboMask;
    [Tooltip("Combo breaks after this timeout")]
    public float comboBreakTimeout = 3.0f;
    [Tooltip("No. of attacks after which combo starts")]
    public int startIndex = 3;
    [Tooltip("Amount of fill added to special attack bar on each combo")]
    [Range(0f, 1f)]
    public float specialBarFillRate = 0.1f;
    [Tooltip("Amount of decay per second on special attack bar")]
    [Range(0f, 0.5f)]
    public float specialBarDecayRate = 0.1f;
    [Tooltip("Special attack bar does not decay below this point")]
    [Range(0f, 1f)]
    public float specialBarReserve = 0.3f;

    bool isInCombo = false;
    int attackCount = 0;
    float comboTimer = 0.0f;

    WeaponController weaponController;

    private void Start()
    {
        isInCombo = false;
        attackCount = -startIndex;
        GameManager.Instance.PlayerSpecialAttack = 0.0f;
        weaponController = GetComponent<WeaponController>();
    }

    public void OnPlayerHit(int targetLayer)
    {
        if ((comboMask.value & (1 << targetLayer)) > 0 && !weaponController.isPerformingSpecialAttack)
        {
            ++attackCount;
            comboTimer = 0.0f;
            if (attackCount > 0)
            {
                if (!isInCombo)
                {
                    StartCoroutine(ComboSequence());
                } else
                {
                    AudioManager.Instance.ComboHitSound(attackCount * 0.01f);
                }
                UIManager.Instance.UpdateCombo(attackCount);
                GameManager.Instance.PlayerSpecialAttack += specialBarFillRate;
            }
        }
    }

    public void OnPlayerMiss()
    {
        if (!weaponController.isPerformingSpecialAttack)
        {
            // Destroy combo
            if (isInCombo)
            {
                RuntimeManager.PlayOneShot(AudioManager.Config.comboMiss);
            }
            comboTimer = comboBreakTimeout;
        }
    }

    IEnumerator ComboSequence()
    {
        isInCombo = true;
        while (true)
        {
            comboTimer += Time.unscaledDeltaTime;
            float progress = comboTimer / comboBreakTimeout;
            UIManager.Instance.SetHUDFade(1.0f - progress);

            if (comboTimer > comboBreakTimeout)
            {
                break;
            }

            yield return null;
        }
        isInCombo = false;
        StartCoroutine(DecaySequence());
        attackCount = -startIndex;
        UIManager.Instance.ClearCombo();
    }

    IEnumerator DecaySequence()
    {
        if (GameManager.Instance.PlayerSpecialAttack < 1f)
        {
            // Decay stops if combo starts building up
            while (!isInCombo && GameManager.Instance.PlayerSpecialAttack > specialBarReserve)
            {
                float deltaDecay = (specialBarDecayRate * Time.unscaledDeltaTime);
                GameManager.Instance.PlayerSpecialAttack -= deltaDecay;

                yield return null;
            }
            if (!isInCombo)
            {
                GameManager.Instance.PlayerSpecialAttack = specialBarReserve;
            }
        }
    }
}
