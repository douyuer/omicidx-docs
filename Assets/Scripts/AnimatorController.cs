using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Animator animator;
    MovimentControler mov;
    Rigidbody2D rig;
    Vector2 lastV;
    


    // Start is called before the first frame update
    void Start()
    {
        
        animator = GetComponent<Animator>();
        mov = GetComponent<MovimentControler>();
        rig = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        bool movex = Mathf.Abs(rig.velocity.x) > 0.01;
        bool gnd = mov.getGrounded();
        animator.SetBool("Grounded", gnd);
        animator.SetBool("Walk", gnd && movex);
         if (mov.stage == 2)
        {
            animator.SetTrigger("Jump");
        }
        if (rig.velocity.y < 0 && lastV.y >= 0)
        {
            animator.SetBool("Fall", true);
        }
        if (rig.velocity.y == 0)
        {
            animator.SetBool("Fall",false);
        }
        lastV = rig.velocity;
    }
}
