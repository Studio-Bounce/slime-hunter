using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int> OnDamaged;

    public void ApplyDamage(int amount) => OnDamaged?.Invoke(amount);
}
