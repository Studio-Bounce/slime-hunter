using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;

public enum EnemyEye
{
    NORMAL,
    ATTACK,
    DEATH
};

[RequireComponent(typeof(SphereCollider))]
public class Enemy : DamageTaker
{
    [Header("Slime Eyes")]
    [SerializeField] SkinnedMeshRenderer normalEye;
    [SerializeField] SkinnedMeshRenderer attackEye;
    [SerializeField] SkinnedMeshRenderer deathEye;
    EnemyEye eye;

    [SerializeField] float damageEyeTimer = 1.0f;
    bool freezeEyeChange = false;

    [Header("Hit Feedback")]
    [SerializeField] VisualEffect hitVFX;
    [SerializeField] float hitVFXDuration = 1.0f;
    [SerializeField] SkinnedMeshRenderer slimeOuterBody;
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField][ColorUsage(true, true)] Color flashColor;

    [Header("Death")]
    [SerializeField] GameObject slimeModel;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] float deathDelay = 3.5f;


    [Header("Canvas")]
    [SerializeField] protected Slider healthSlider;
    [SerializeField] float canvasTimeout = 10.0f;
    Canvas enemyCanvas;
    CanvasGroup enemyCanvasGroup;
    bool canvasTriggered = false;  // Used as a flag to detect if enemy got hit, i.e. canvas should be shown

    BasicSlime_FSM fsm;

    protected override void Start()
    {
        base.Start();

        normalEye.enabled = true;
        attackEye.enabled = false;
        deathEye.enabled = false;
        isAlive = true;

        fsm = GetComponent<BasicSlime_FSM>();
        //CanvasManager.Instance.AddAnchoredElement(transform, healthSlider.GetComponent<RectTransform>(), new Vector2(0, 80));

        // Hide canvas unless required, for efficiency
        enemyCanvas = GetComponentInChildren<Canvas>();
        if (enemyCanvas != null)
        {
            enemyCanvasGroup = enemyCanvas.gameObject.GetComponent<CanvasGroup>();
            enemyCanvas.enabled = false;
        }
    }

    // Used in child classes to call the original TakeDamage method
    protected void BaseEnemyTakeDamage(Damage damage)
    {
        if (!isAlive)
        {
            return;
        }
        base.TakeDamage(damage);
        if (!isInvincible)
        {
            StartCoroutine(ShowHitParticles());
            StartCoroutine(FlashSlime());
        }
    }

    public override void TakeDamage(Damage damage)
    {
        BaseEnemyTakeDamage(damage);

        if (healthSlider != null && enemyCanvas != null)
        {
            if (enemyCanvas.enabled)
            {
                canvasTriggered = true;
                healthSlider.value = ((float)health / maxHealth);
            }
            else
            {
                enemyCanvasGroup.alpha = 1;
                enemyCanvas.enabled = true;
                // Update health with a small delay
                StartCoroutine(UpdateHealth(0.1f));
                StartCoroutine(DisableCanvasAfterTimeout());
            }
        }

        if (!isInvincible)
        {
            StartCoroutine(ChangeEyeToDamage());
        }
    }

    IEnumerator UpdateHealth(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthSlider.value = ((float)health / maxHealth);
    }

    IEnumerator DisableCanvasAfterTimeout()
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < canvasTimeout)
        {
            yield return null;

            timeElapsed += Time.deltaTime;
            if (canvasTriggered)
            {
                // Reset time as enemy got hit again
                timeElapsed = 0.0f;
                canvasTriggered = false;
            }
        }

        // Fade
        float t = 0.5f;
        while (t >= 0.0f)
        {
            enemyCanvasGroup.alpha = t * 2;
            yield return null;
            t -= Time.deltaTime;
        }
        enemyCanvas.enabled = false;
    }

    public override void Death()
    {
        // Trigger events
        onDeathEvent.Invoke();

        isAlive = false;
        fsm.ChangeState(fsm.DeadStateName);

        // Hide visible meshes / UI
        slimeModel.SetActive(false);
        healthSlider.gameObject.SetActive(false);

        // Ensure the enemy doesn't give damage after dying
        if (TryGetComponent<SphereCollider>(out var sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        // Stop movement
        GetComponent<SlimeSteeringAgent>().enabled = false;
        deathParticles.Play();
        Destroy(gameObject, deathDelay);
    }

    public void SetEye(EnemyEye enemyEye)
    {
        StartCoroutine(ChangeEye(enemyEye));
    }

    public EnemyEye GetEnemyEye()
    {
        return eye;
    }

    IEnumerator ShowHitParticles()
    {
        if (hitVFX != null)
        {
            hitVFX.Play();
            yield return new WaitForSeconds(hitVFXDuration);
            hitVFX.Stop();
        }
    }

    IEnumerator FlashSlime()
    {
        if (slimeOuterBody != null)
        {
            Color slimeColor = slimeOuterBody.materials[0].color;

            // Change color to white for a short duration
            slimeOuterBody.materials[0].color = flashColor;

            yield return new WaitForSeconds(flashDuration);

            // Revert color
            slimeOuterBody.materials[0].color = slimeColor;
        }
    }

    IEnumerator ChangeEyeToDamage()
    {
        SetEye(EnemyEye.DEATH);
        freezeEyeChange = true;
        yield return new WaitForSeconds(damageEyeTimer);
        freezeEyeChange = false;
        SetEye(EnemyEye.NORMAL);
    }

    IEnumerator ChangeEye(EnemyEye enemyEye)
    {
        while (freezeEyeChange)
        {
            yield return null;
        }
        eye = enemyEye;
        normalEye.enabled = (enemyEye == EnemyEye.NORMAL);
        attackEye.enabled = (enemyEye == EnemyEye.ATTACK);
        deathEye.enabled = (enemyEye == EnemyEye.DEATH);
    }
}
