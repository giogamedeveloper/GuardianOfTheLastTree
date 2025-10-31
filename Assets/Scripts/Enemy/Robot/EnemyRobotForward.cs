using UnityEngine;

public class EnemyRobotForward : StateMachineBehaviour
{
    //Referencia al controller principal
    public Enemy enemy;
    public AudioSource clip;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy == null) enemy = animator.GetComponentInParent<Enemy>();
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

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
   
}
