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
    private bool inAir;

    void Start() {
        player = GetComponent<PlatformPlayer>();

        jumping = player.jumping;
        dashing = player.dashing;
        stoppingTime = player.stoppingTime;
        tricking = player.tricking;
        grinding = player.grinding;
        manualing = player.manualing;
        takingHit = player.invuln;
        dead = player.dead;
        inAir = player.canJump;
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
        inAir = player.canJump;

        if (player.moveInput.x != 0) {
            animator.SetBool("moving", true);
            Debug.Log("Moving now");
        }
        else {
            animator.SetBool("moving", false);
            Debug.Log("Stopped Moving");
        }

        animator.SetBool("jumping", jumping);
        animator.SetBool("dashing", dashing);
        animator.SetBool("stoppingTime", stoppingTime);
        animator.SetBool("tricking", tricking);
        animator.SetBool("grinding", grinding);
        animator.SetBool("manualing", manualing);
        animator.SetBool("takingHit", takingHit);
        animator.SetBool("dead", dead);
        animator.SetBool("inAir", !inAir);
    }
}
