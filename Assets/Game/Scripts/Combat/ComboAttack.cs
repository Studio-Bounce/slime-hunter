using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [Tooltip("Layermask used for hit detection")]
    public LayerMask comboMask;
    [Tooltip("Combo breaks after this timeout")]
    public float comboBreakTimeout = 3.0f;
    [Tooltip("Amount of fill added to special attack bar on each combo")]
    [Range(0f, 1f)]
    public float specialBarRate = 0.1f;
    [Tooltip("No. of attacks after which combo starts")]
    public int startIndex = 3;

    // TODO: Move to GameManager
    public float specialAttack = 0.0f;

    bool isInCombo = false;
    int attackCount = 0;
    float comboTimer = 0.0f;

    private void Start()
    {
        isInCombo = false;
        attackCount = -startIndex;
        specialAttack = 0.0f;
    }

    public void OnPlayerHit(int targetLayer)
    {
        if ((comboMask.value & (1 << targetLayer)) > 0)
        {
            ++attackCount;
            comboTimer = 0.0f;
            if (attackCount > 0)
            {
                Debug.Log(attackCount + "X");
                specialAttack = Mathf.Clamp01(specialAttack + specialBarRate);
                if (!isInCombo)
                {
                    StartCoroutine(ComboSequence());
                }
            }
        }
    }

    IEnumerator ComboSequence()
    {
        isInCombo = true;
        while (true)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer > comboBreakTimeout)
            {
                break;
            }

            yield return null;
        }
        Debug.Log("COMBO BROKE!");
        attackCount = -startIndex;
        isInCombo = false;
    }
}
