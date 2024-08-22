using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DummyEnemy : DynamicDamageTaker
{
    Vector3 start;

    Coroutine _shake;
    Coroutine _disableCanvas;

    protected override void Start()
    {
        base.Start();
        start = transform.position;
        StartCoroutine(Regenerate());
    }

    public override bool TakeDamage(Damage damage, bool detectDeath = false)
    {
        if (_shake != null) StopCoroutine(_shake);
        _shake = StartCoroutine(ShakeDummy(damage));
        return base.TakeDamage(damage, detectDeath);
    }

    IEnumerator Regenerate()
    {
        while (true)
        {
            if (health < maxHealth)
            {
                health += 10;
                StartCoroutine(UpdateHealth(0.1f));

                if (health >= maxHealth)
                {
                    if (_disableCanvas != null) { StopCoroutine(_disableCanvas); }
                    _disableCanvas = StartCoroutine(DisableCanvasAfterTimeout());
                }
            }
            health = Mathf.Min(health, maxHealth);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ShakeDummy(Damage damage)
    {
        float elapsed = 0.0f;
        float duration = 0.3f;
        while (elapsed < duration)
        {
            Vector3 randomOffset = 0.02f * damage.knockback * Random.insideUnitSphere;
            transform.position = start + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = start;
    }
}
