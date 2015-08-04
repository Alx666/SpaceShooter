using UnityEngine;
using System.Collections;

public class AIStateGoNear : StateMachineBehaviour 
{

    private IAIActor m_hActor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_hActor = animator.GetComponent<IAIActor>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("DistanceToTarget", Vector3.Distance(m_hActor.Transform.position, m_hActor.Target.Transform.position));
        m_hActor.Rigidbody.AddForce((m_hActor.Target.Transform.position - m_hActor.Transform.position).normalized * m_hActor.EngineForce);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
