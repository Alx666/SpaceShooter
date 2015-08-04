using UnityEngine;
using System.Collections;
using System;

//TODO: implementare spostamento su layer o Physics.IgnoreCollision con chi lo ha sparato
//TODO: risolvere bug che mostra il trail quando il pool sottostante inserisce e rimuove continuamente
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class Bullet : MonoBehaviour, IPoolable, IBullet
{

    public GameObject ExplosionPrefab;

    public float Damage;
    public float DestoryTime = 5.0f;

    private AudioSource     m_hAudioShoot;
    private Renderer        m_hRenderer;
    private TrailRenderer   m_hTrailRenderer;

    public Rigidbody    RigidBody   { get; private set; }
    public Collider     Collider { get; private set; }    
        
    void Start()
    {
        //this.Speed *= GameManager.Instance.CurrentDifficulty.SpeedCoeff;
    }
	
    void Awake()
    {
        RigidBody               = this.GetComponent<Rigidbody>();
        Collider                = this.GetComponent<Collider>();
        m_hRenderer             = this.GetComponent<Renderer>();
        m_hAudioShoot           = this.GetComponent<AudioSource>();
        m_hTrailRenderer        = this.GetComponent<TrailRenderer>();

        m_hAudioShoot.pitch     = UnityEngine.Random.Range(0.9f, 1.1f);
        m_hAudioShoot.priority  = UnityEngine.Random.Range(128, 256);
        m_hAudioShoot.volume    = UnityEngine.Random.Range(0.5f, 1f);
    }
	
	void Update () 
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);

        if ((screenPos.x < -0.5f || screenPos.x > 1.5f) || (screenPos.y < -0.5f || screenPos.y > 1.5f))
        {
            Pool.Recycle(this.gameObject);
        }        
	}

    void OnCollisionEnter(Collision hColl)
    {
        this.OnImpact(hColl.collider, hColl.contacts[0].point);
    }

    void OnTriggerEnter(Collider hColl)
    {
        this.OnImpact(hColl, this.transform.position);
    }


    private void OnImpact(Collider hColl, Vector3 vImpactPoint)
    {
        IDamageable hToDmg = hColl.gameObject.GetComponent<IDamageable>();
        if (hToDmg != null)
        {
            try
            {
                GameObject hExplosion = GlobalFactory.GetInstance(ExplosionPrefab);
                hExplosion.transform.position = vImpactPoint;
                hExplosion.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                hToDmg.Damage(this.Damage);
            }
            catch (Exception hEx)
            {
                Debug.LogError(hEx);
            }
            finally
            {
                Pool.Recycle(this.gameObject);
            }
        }
        else
        {
            Shield hShield = hColl.gameObject.GetComponent<Shield>();
            if (hShield)
            {
                //Has hit the shield, so reverse bullet direction
                Vector3 vCounterDirection = -(hShield.transform.position - this.transform.position);
                //Quaternion vRandomAngle = Quaternion.EulerAngles(0f, UnityEngine.Random.Range(0f, 10.0f), 0f);
                //vCounterDirection = vCounterDirection;
                vCounterDirection.Normalize();

                Rigidbody hPhysBody = this.GetComponent<Rigidbody>();
                this.transform.forward = vCounterDirection;
                hPhysBody.velocity = vCounterDirection * hPhysBody.velocity.magnitude;
            }
        }
    }

    public Pool Pool { get; set; }

    public void Enable()
    {
        this.gameObject.SetActive(true);
        this.StartCoroutine(this.TrailRendererReactivation());
    }

    public void Disable()
    {
        m_hTrailRenderer.enabled = false;
        this.gameObject.SetActive(false);
        this.RigidBody.velocity         = Vector3.zero;
        this.RigidBody.angularVelocity  = Vector3.zero;

    }

    private IEnumerator TrailRendererReactivation()
    {
        m_hTrailRenderer.enabled = true;
        float trailTime = m_hTrailRenderer.time;
        m_hTrailRenderer.time = 0;
        yield return null;
        m_hTrailRenderer.time = trailTime;
    }

    public void Shoot(Vector3 vPosition, Vector3 vDirection, float fForce, ForceMode eMode)
    {
        this.gameObject.transform.position = vPosition;
        this.gameObject.transform.forward = vDirection;
        this.RigidBody.AddForce(this.gameObject.transform.forward * fForce, eMode);
    }

    public Transform Transform
    {
        get { return this.transform; }
    }
}
