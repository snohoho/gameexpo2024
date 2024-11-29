using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public GameObject model;
    [SerializeField] private Animator animator;
    private PlatformPlayer player;

    //anim bools
    private bool jumping;
    private bool dashing;
    private bool stoppingTime;
    private bool tricking;
    private bool grinding;
    private bool manualing;
    private bool takingHit;
    private bool dead;

    void Start() {
        player = transform.parent.GetComponent<PlatformPlayer>();

        jumping = player.jumping;
        dashing = player.dashing;
        stoppingTime = player.stoppingTime;
        tricking = player.tricking;
        grinding = player.grinding;
        manualing = player.manualing;
        takingHit = player.invuln;
        dead = player.dead;
    }

    void Update()
    {
        jumping = player.jumping;
        dashing = player.dashing;
        stoppingTime = player.stoppingTime;
        tricking = player.tricking;
        grinding = player.grinding;
        manualing = player.manualing;
        takingHit = player.invuln;
        dead = player.dead;

        if(player.moveInput.x != 0) {
            animator.SetBool("moving", true);
        }
        else {
            animator.SetBool("moving", false);
        }

        animator.SetBool("jumping", jumping);
        animator.SetBool("dashing", dashing);
        animator.SetBool("stoppingTime", stoppingTime);
        animator.SetBool("tricking", tricking);
        animator.SetBool("grinding", grinding);
        animator.SetBool("manualing", manualing);
        animator.SetBool("takingHit", takingHit);
        animator.SetBool("dead", dead);
    }
}
