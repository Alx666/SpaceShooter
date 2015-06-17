using UnityEngine;
using System.Collections;

public class AIStateAim : IAIState
{
    public IAIState Success { get; set; }
    public IAIState Fail    { get; set; }

    private float m_fMaxAimAngle;

    public AIStateAim(float fMaxAimAngle)
    {
        m_fMaxAimAngle = fMaxAimAngle;
    }

    public void OnFixedUpdate(IAIActor hShip)
    {
        if (hShip.Target == null)
            return;

        float sign = Mathf.Sign(Vector3.Angle(hShip.Transform.position, hShip.Target.Transform.position));

        Vector3 v1 = hShip.Transform.position;
        Vector3 v2 = hShip.Target.Transform.position;
        Vector3 v3 = v2 - v1;

        float fAngle = Mathf.Sign(Vector3.Dot(v3.normalized, hShip.Transform.right));

        hShip.Rigidbody.AddTorque(0f, fAngle * hShip.TurnForce, 0f, ForceMode.VelocityChange);
    }

    public IAIState Update(IAIActor hShip)
    {

        if (hShip.Target == null)
        {
            Fail.OnStateEnter(hShip, this);
            return Fail;
        }
        else
        {
            Vector3 v1 = hShip.Transform.forward;
            Vector3 v2 = (hShip.Target.Transform.position - hShip.Transform.position).normalized;

            float fAngle = Vector3.Angle(v1, v2);

            if (fAngle <= m_fMaxAimAngle)
            {
                Success.OnStateEnter(hShip, this);
                return Success;
            }
            else
            {
                return this;
            }
        }
    }

    public void OnStateEnter(IAIActor hActor, IAIState hPrevState)
    {
        
    }
}