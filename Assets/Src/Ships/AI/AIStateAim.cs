using UnityEngine;
using System.Collections;

public class AIStateAim : StateMachineBehaviour
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
        float fAngleToEnemy;
        float fSign = Turn(m_hActor, m_hActor.Target, out fAngleToEnemy);

        animator.SetFloat("AngleToEnemy", fAngleToEnemy);
        m_hActor.Rigidbody.AddTorque(0f, fSign * m_hActor.TurnForce, 0f, ForceMode.VelocityChange);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public static float Turn(IAIActor hShip, IAIActor hTarget, out float fAngle)
    {
        Vector3 v1 = hShip.Transform.position;
        Vector3 v2 = hTarget.Transform.position;
        Vector3 v3 = v2 - v1;

        fAngle = Vector3.Angle((v2 - v1).normalized, hShip.Transform.forward);
        return Mathf.Sign(Vector3.Dot(v3.normalized, hShip.Transform.right));
    }
}