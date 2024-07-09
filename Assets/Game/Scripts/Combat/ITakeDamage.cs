using UnityEngine;
using UnityEngine.Events;

public interface ITakeDamage
{
    /// <summary>
    /// Returns if damage should be registered.
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="detectDeath">Should it call Destroy() if health falls to 0?</param>
    /// <returns></returns>
    bool TakeDamage(Damage damage, bool detectDeath);

    /// <summary>
    /// Called when the object's health falls below 0.
    /// </summary>
    /// <param name="killObject">Should the game object be destroyed? Can be used if child classes do not want parent's Destory() to destroy the object.</param>
    void Death(bool killObject);
}
