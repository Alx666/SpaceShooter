using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class AIStateFire : StateMachineBehaviour
{
    private IAIActor m_hActor;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_hActor = animator.GetComponent<IAIActor>();
        m_hActor.Transform.forward = (m_hActor.Target.Transform.position - m_hActor.Transform.position).normalized;
        m_hActor.Rigidbody.angularVelocity = Vector3.zero;
        m_hActor.Weapons.ForEach(hW => hW.OnbuttonPressed());        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float fAngleToEnemy;
        float fSign = AIStateAim.Turn(m_hActor, m_hActor.Target, out fAngleToEnemy);

        animator.SetFloat("AngleToEnemy", fAngleToEnemy);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_hActor.Weapons.ForEach(hW => hW.OnbuttonReleased());
    }
}