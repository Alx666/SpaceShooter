using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIController : MonoBehaviour, IDamageable
{
    public float        MainthrusterPower;
    public float        SidethrusterPower;
    public float        Health              = 100;
    public float        FollowTime;
    public float        TargetAndFireTime;
    public float        EvadeTime;
    public float        FullThrottleTime;
    public MeshFilter   DamagedDiffuse;
    public MeshFilter   DamagedColor;
    public int          ScoreIncrement      = 100;
    public string       DEBUG_STATE;


    public float currentHealth { get; private set; }

    public Rigidbody Rigidbody { get; private set; }

    private static  Object  m_hResourceExplosion;

    private GameObject      m_hExplosion;
    private Animator        m_hAnimatorController;
    private IEnemyState     m_hCurrentState;
    private IEnemyState     m_hFollowState;
    private WorldController m_hWorldController;
    private Renderer        m_hRenderer;       


    void Awake()
    {
        Rigidbody                   = this.GetComponent<Rigidbody>();

        m_hWorldController          = this.gameObject.GetComponent<WorldController>();
        m_hWorldController.enabled  = false;
        m_hRenderer                 = this.GetComponent<Renderer>();
        m_hAnimatorController       = this.GetComponent<Animator>();
        
        //Create States
        StateTimed hFollow = new StateFollow(FollowTime);
        StateTimed hFire = new StateTargetAndFire(TargetAndFireTime);
        StateTimed hEvade = new StateEvade(EvadeTime);
        StateTimed hThrottle = new StateFullThrottle(FullThrottleTime);

        //Connect States
        hFollow.Add(hFire);
        hFollow.Add(hEvade);
        hFire.Add(hEvade);
        hFire.Add(hFollow);
        hEvade.Add(hThrottle);
        hThrottle.Add(hFollow);
        hThrottle.Add(hFire);

        //Lock'N'Loud
        m_hCurrentState = hFollow;
        m_hFollowState = hFollow;

        m_hResourceExplosion = Resources.Load("Explosion_FX");
        
        currentHealth = Health;
    }


    void Start()
    {
        GameManager.Instance.RegisterEnemy(this);
    }


    void Update()
    {
        if (m_hCurrentState == null)
            return;

        m_hCurrentState = m_hCurrentState.OnUpdate();

        Vector3 vViewportPos = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);

        if (!(m_hCurrentState is StateFollow) && (vViewportPos.x > 1.0f || vViewportPos.x < 0f || vViewportPos.y > 1f || vViewportPos.y < 0f))
        {
            m_hCurrentState = m_hFollowState;
        }


        DEBUG_STATE = m_hCurrentState.ToString();
      
    }

    void FixedUpdate()
    {
        if (m_hCurrentState == null)
            return;

        m_hCurrentState.OnFixedUpdate(this);

        Quaternion vRot = this.transform.rotation;
        Vector3 vAngles = vRot.eulerAngles;
        vRot.eulerAngles = new Vector3(0f, vAngles.y, 0f);
        this.transform.rotation = vRot;

        if (!m_hWorldController.enabled)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
            if ((screenPos.x > 0f && screenPos.y > 0f) && (screenPos.x < Camera.main.pixelWidth && screenPos.y < Camera.main.pixelHeight))
            {
                m_hWorldController.enabled = true;
            }
        }
    }

    public void Damage(float fDmg)
    {
        
        currentHealth -= fDmg;
        if (currentHealth < Health / 2)
        {
            //particle on fire
        }
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    private const float A = 10f;
    public void Death()
    {
        m_hCurrentState = null;
        m_hExplosion = GameObject.Instantiate(m_hResourceExplosion) as GameObject;
        m_hExplosion.transform.position = this.transform.position;
        if (m_hAnimatorController)
            m_hAnimatorController.SetTrigger("Die");

        //GameObject.Destroy(this.gameObject);
        Rigidbody hBody = this.GetComponent<Rigidbody>();
        hBody.angularDrag = 0f;
        hBody.drag = 0f;
        hBody.constraints = RigidbodyConstraints.None;
        Vector3 vRot = Random.rotation.eulerAngles;
        hBody.AddTorque(A * vRot.x, A * vRot.y, A * vRot.z);
        hBody.AddForce(A * Vector3.up * Mathf.Sign(Random.Range(-1f, 1f)), ForceMode.Impulse);


        this.gameObject.GetComponent<Collider>().enabled = false;
        this.gameObject.GetComponent<WorldController>().enabled = false;
        //m_hCannons.ForEach(x => x.StopFire());
        //m_hCannons.Clear();
        GameObject.Destroy(m_hExplosion);

        //istanziare particella esplosione
        //disattivare playerController
        //triggerare gui end game

        GameManager.Instance.AddScore(this.ScoreIncrement);


        StartCoroutine(DestroyShip(5f));
    }

    IEnumerator DestroyShip(float fWaitTIme)
    {
        yield return new WaitForSeconds(fWaitTIme);
        GameObject.Destroy(this.gameObject);
    }


    private interface IEnemyState
    {
        void OnFixedUpdate(AIController hShip);
        IEnemyState OnUpdate();
    }

    private abstract class StateTimed : IEnemyState
    {
        private float m_fTimeToChange;
        private float m_fTimeLeft;
        private List<IEnemyState> m_hNexts;

        public StateTimed(float fTime)
        {
            m_fTimeToChange = fTime;
            m_fTimeLeft = m_fTimeToChange;
            m_hNexts = new List<IEnemyState>();
        }

        public virtual IEnemyState OnUpdate()
        {
            m_fTimeLeft -= Time.deltaTime;

            if (m_fTimeLeft <= 0.0f)
            {
                m_fTimeLeft = m_fTimeToChange;

                return this.ChangeState();
            }
            else
            {
                return this;
            }
        }

        public abstract void OnFixedUpdate(AIController hShip);

        protected virtual IEnemyState ChangeState()
        {
            return m_hNexts[UnityEngine.Random.Range(0, m_hNexts.Count)];
        }

        public void Add(IEnemyState hNext)
        {
            m_hNexts.Add(hNext);
        }

        protected static float TurnDirection(Vector3 vShipPosition, Vector3 vShipRight, Vector3 vPlayerPosition)
        {
            float sign = Mathf.Sign(Vector3.Angle(vShipPosition, vPlayerPosition));

            Vector3 v1 = vShipPosition;
            Vector3 v2 = vPlayerPosition;
            Vector3 v3 = v2 - v1;

            return Mathf.Sign(Vector3.Dot(v3.normalized, vShipRight));
        }
    }

    private class StateFollow : StateTimed
    {
        public StateFollow(float fTime)
            : base(fTime)
        {

        }

        public override void OnFixedUpdate(AIController hShip)
        {
            if (PlayerController.Instance == null)
                return;
            float res = TurnDirection(hShip.transform.position, hShip.transform.right, PlayerController.Instance.transform.position);

            hShip.Rigidbody.AddTorque(0f, res * hShip.SidethrusterPower, 0f);
            hShip.Rigidbody.AddForce(hShip.transform.forward * hShip.MainthrusterPower);
        }
    }

    private class StateTargetAndFire : StateTimed
    {
        public StateTargetAndFire(float fTime)
            : base(fTime)
        {

        }
        public override void OnFixedUpdate(AIController hShip)
        {
            if (PlayerController.Instance == null)
                return;
            float res = TurnDirection(hShip.transform.position, hShip.transform.right, PlayerController.Instance.transform.position);

            hShip.Rigidbody.AddTorque(0f, res * hShip.SidethrusterPower, 0f);


            Vector3 vPlayerPos = PlayerController.Instance.transform.position;
            Vector3 vPosition = hShip.transform.position;
            Vector3 vDiff = (vPlayerPos - vPosition).normalized;

            if (Vector3.Angle(hShip.transform.forward, vDiff) <= 15f)
            {
                //for (int i = 0; i < hShip.m_hCannons.Count; i++)
                //{
                //    Cannon hCurrent = hShip.m_hCannons[i];
                //    hCurrent.Fire();
                //}
            }
            else
            {
                //for (int i = 0; i < hShip.m_hCannons.Count; i++)
                //{
                //    Cannon hCurrent = hShip.m_hCannons[i];
                //    hCurrent.StopFire();
                //}
            }
        }
    }

    private class StateEvade : StateTimed
    {
        public StateEvade(float fTime)
            : base(fTime)
        {

        }

        public override void OnFixedUpdate(AIController hShip)
        {
            PlayerController hToFollow = PlayerController.Instance;
            if (hToFollow == null)
                return;

            Vector3 vEvadePoint = hToFollow.transform.position;
            vEvadePoint.x = -vEvadePoint.x;
            vEvadePoint.z = -vEvadePoint.z;

            float res = TurnDirection(hShip.transform.position, hShip.transform.right, vEvadePoint);

            hShip.Rigidbody.AddTorque(0f, -res * hShip.SidethrusterPower, 0f);
        }
    }

    private class StateFullThrottle : StateTimed
    {
        public StateFullThrottle(float fTime)
            : base(fTime)
        {
        }

        public override void OnFixedUpdate(AIController hShip)
        {
            hShip.Rigidbody.AddForce(hShip.transform.forward * hShip.MainthrusterPower);
        }
    }
}
