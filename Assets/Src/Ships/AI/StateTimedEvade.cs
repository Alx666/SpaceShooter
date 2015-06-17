//using UnityEngine;
//using System.Collections;

//internal class StateEvade : StateTimed
//{
//    public StateEvade(float fTime)
//        : base(fTime)
//    {

//    }

//    public override void OnFixedUpdate(Enemy hShip)
//    {
//        PlayerController hToFollow = PlayerController.Instance;
//        if (hToFollow == null)
//            return;

//        Vector3 vEvadePoint = hToFollow.transform.position;
//        vEvadePoint.x = -vEvadePoint.x;
//        vEvadePoint.z = -vEvadePoint.z;

//        float res = TurnDirection(hShip.transform.position, hShip.transform.right, vEvadePoint);



//        //hShip.m_hRigidbody.AddTorque(0f, -res * hShip.SidethrusterPower, 0f);
//    }
//}