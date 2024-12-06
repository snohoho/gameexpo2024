using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public GameObject model;
    [SerializeField] public Animator animator;
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
    private bool inAir;
    public bool hasDied;

    void Start() {
        player = GetComponent<PlatformPlayer>();

        jumping = player.jumping;
        dashing = player.dashing;
        stoppingTime = player.StoppingTime;
        tricking = player.tricking;
        grinding = player.grinding;
        manualing = player.manualing;
        takingHit = player.invuln;
        dead = player.Dead;
        inAir = player.canJump;
        hasDied = false;
    }

    void Update()
    {
        jumping = player.jumping;
        dashing = player.dashing;
        stoppingTime = player.StoppingTime;
        tricking = player.tricking;
        grinding = player.grinding;
        manualing = player.manualing;
        takingHit = player.invuln;
        dead = player.Dead;
        inAir = player.canJump;

        if (player.moveInput.x != 0) {
            animator.SetBool("moving", true);
        }
        else {
            animator.SetBool("moving", false);
        }

        if(player.manualing) {
            animator.SetTrigger("manualing");
        }
        else if(!player.manualing) {
            animator.ResetTrigger("manualing");
        }

        animator.SetBool("jumping", jumping);
        animator.SetBool("dashing", dashing);
        animator.SetBool("tricking", tricking);
        animator.SetBool("grinding", grinding);
        animator.SetBool("takingHit", takingHit);
        animator.SetBool("inAir", !inAir);
    }

}
