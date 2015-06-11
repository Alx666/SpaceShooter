using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(IWeapon))]
public class Enemy : MonoBehaviour, IDamageable, IPoolable
{
    //public delegate void EnemyDestroyedHandler(Enemy hEnemy);
    //public event EnemyDestroyedHandler Destroyed;

    private static  Object  m_hResourceExplosion;


    public float        MainthrusterPower;
    public float        SidethrusterPower;
    public float        Hp              = 100;
    public float        FollowTime;
    public float        TargetAndFireTime;
    public float        EvadeTime;
    public float        FullThrottleTime;
    public MeshFilter   DamagedDiffuse;
    public MeshFilter   DamagedColor;
    public int          ScoreIncrement      = 100;
    public string       DEBUG_STATE;


    public float Health { get; private set; }



    private Rigidbody       m_hRigidbody;
    private GameObject      m_hExplosion;
    private Animator        m_hAnimatorController;
    private IEnemyState     m_hCurrentState;
    private IEnemyState     m_hFollowState;
    private WorldController m_hWorldController;
    private Renderer        m_hRenderer;       


    void Awake()
    {
        m_hRigidbody                   = this.GetComponent<Rigidbody>();
        m_hWorldController          = this.gameObject.GetComponent<WorldController>();
        m_hWorldController.enabled  = false;
        m_hRenderer                 = this.GetComponent<Renderer>();
        m_hAnimatorController       = this.GetComponent<Animator>();
        
        //Create States
        StateTimed hFollow          = new StateFollow(FollowTime);
        StateTimed hFire            = new StateTargetAndFire(TargetAndFireTime);
        StateTimed hEvade           = new StateEvade(EvadeTime);
        StateTimed hThrottle        = new StateFullThrottle(FullThrottleTime);

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
        
        this.Health = Hp;
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
        
        Hp -= fDmg;
        if (Hp < Hp / 2)
        {
            //particle on fire
        }
        if (Hp <= 0)
        {
            Destroy();
        }
    }

    private const float A = 10f;
    public void Destroy()
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

    public Pool Pool { get; set; }

    public void Enable()
    {
        
    }

    public void Disable()
    {
        
    }
}
