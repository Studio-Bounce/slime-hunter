using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(menuName = "Game Material/Healing")]
public class HealingMaterialSO : MaterialSO
{
    [Header("Healing")]
    public int healAmount = 10;

    public override bool Use()
    {
        if (!GameManager.Instance.IsFullHealth)
        {
            GameManager.Instance.PlayerHealth += healAmount;
            return true;
        }
        return false;
    }
}