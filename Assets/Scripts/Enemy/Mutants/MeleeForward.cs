using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeForward : StateMachineBehaviour
{
    //Referencia al controller principal
    public AttackEnemyMelee enemy;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null) enemy = animator.GetComponentInParent<AttackEnemyMelee>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Si el navMesh agent está activo y existe un objeto
        if (enemy.nav.enabled && enemy.target != null)
            //Indicamos el destino al agent que es la posicion donde se encuentra el objetivo 
            enemy.nav.SetDestination(enemy.target.position);

        //Verificamos si el enemigo se encuentra a distancia de ataque
        if (!enemy.nav.pathPending && enemy.nav.remainingDistance <= enemy.attackDistance)
        {
            animator.SetTrigger("Attack");
        }
    }

    IEnumerator Duration(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
