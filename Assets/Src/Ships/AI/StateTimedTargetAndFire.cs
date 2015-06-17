//using UnityEngine;
//using System.Collections;


//internal class StateTargetAndFire : StateTimed
//{
//    public StateTargetAndFire(float fTime)
//        : base(fTime)
//    {

//    }
//    public override void OnFixedUpdate(Enemy hShip)
//    {
//        if (PlayerController.Instance == null)
//            return;
//        float res = TurnDirection(hShip.transform.position, hShip.transform.right, PlayerController.Instance.transform.position);

//        //hShip.m_hRigidbody.AddTorque(0f, res * hShip.SidethrusterPower, 0f);


//        Vector3 vPlayerPos = PlayerController.Instance.transform.position;
//        Vector3 vPosition = hShip.transform.position;
//        Vector3 vDiff = (vPlayerPos - vPosition).normalized;

//        if (Vector3.Angle(hShip.transform.forward, vDiff) <= 15f)
//        {
//            //for (int i = 0; i < hShip.m_hCannons.Count; i++)
//            //{
//            //    Cannon hCurrent = hShip.m_hCannons[i];
//            //    hCurrent.Fire();
//            //}
//        }
//        else
//        {
//            //for (int i = 0; i < hShip.m_hCannons.Count; i++)
//            //{
//            //    Cannon hCurrent = hShip.m_hCannons[i];
//            //    hCurrent.StopFire();
//            //}
//        }
//    }
//}
