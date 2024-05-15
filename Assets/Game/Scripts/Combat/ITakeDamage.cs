using UnityEngine;
using UnityEngine.Events;

public interface ITakeDamage
{
    void TakeDamage(Damage damage);
    void Death();
}
