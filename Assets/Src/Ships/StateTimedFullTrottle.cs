using UnityEngine;
using System.Collections;


internal class StateFullThrottle : StateTimed
{
    public StateFullThrottle(float fTime)
        : base(fTime)
    {
    }

    public override void OnFixedUpdate(Enemy hShip)
    {
        //hShip.m_hRigidbody.AddForce(hShip.transform.forward * hShip.MainthrusterPower);
    }
}

