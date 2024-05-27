using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    private Animator anim;
    public float cooldowntime = 2f;
    private float nextAttackTime = 1f;
    public static int noOfClick = 3;
    float lastClickedTime = 1;
    float maxComboDelay = 1;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit-1"))
        {
            anim.SetBool("Hit-1", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit-2"))
        {
            anim.SetBool("Hit-2", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit-3-combo"))
        {
            anim.SetBool("Hit-3-combo", false);
            noOfClick = 0;
        }

        if(Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClick = 0;
        }
        if(Time.time > nextAttackTime) 
        {
            if(Input.GetMouseButton(0))
            {
                OnClick();
            }
        }
    }

    void OnClick()
    {
        lastClickedTime = Time.time;
        noOfClick++;
        if(noOfClick == 1)
        {
            anim.SetBool("Hit-1", true);
        }
        noOfClick = Mathf.Clamp(noOfClick, 0, 3);

        if(noOfClick >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit-1"))
        {
            anim.SetBool("Hit-1", false);
            anim.SetBool("Hit-2", true);
        }

        if (noOfClick >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit-2"))
        {
            anim.SetBool("Hit-2", false);
            anim.SetBool("Hit-3-combo", true);
        }
    }
}
