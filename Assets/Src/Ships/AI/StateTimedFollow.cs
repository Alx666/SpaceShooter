//using UnityEngine;
//using System.Collections;


//internal class StateFollow : StateTimed
//{
//    public StateFollow(float fTime)
//        : base(fTime)
//    {

//    }

//    public override void OnFixedUpdate(Enemy hShip)
//    {
//        if (PlayerController.Instance == null)
//            return;
//        float res = TurnDirection(hShip.transform.position, hShip.transform.right, PlayerController.Instance.transform.position);

//        //hShip.m_hRigidbody.AddTorque(0f, res * hShip.SidethrusterPower, 0f);
//        //hShip.m_hRigidbody.AddForce(hShip.transform.forward * hShip.MainthrusterPower);
//    }
//}
