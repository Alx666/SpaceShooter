using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class Bullet : MonoBehaviour, IPoolable
{
    public static Pool<Bullet> Pool;

    public float Speed;
    public float Damage;
    public float DestoryTime = 5.0f;

    private AudioSource m_hAudioShoot;
    private Renderer    m_hRenderer;

    public Rigidbody    RigidBody   { get; private set; }
    public Collider     Collider { get; private set; }
        
    static Bullet()
    {
        Pool = new Pool<Bullet>(Resources.Load("Bullet"));        
    }

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

        m_hAudioShoot.pitch     = UnityEngine.Random.Range(0.9f, 1.1f);
        m_hAudioShoot.priority  = UnityEngine.Random.Range(128, 256);
        m_hAudioShoot.volume    = UnityEngine.Random.Range(0.5f, 1f);
    }
	
	void Update () 
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(this.transform.position);

        if ((screenPos.x < -0.5f || screenPos.x > 1.5f) || (screenPos.y < -0.5f || screenPos.y > 1.5f))
        {
            Bullet.Pool.Recycle(this);
        }        
	}

    void OnTriggerEnter(Collider hColl)
    {
        IDamageable hToDmg = hColl.gameObject.GetComponent<IDamageable>();
        if (hToDmg != null)
        {
            try
            {
                hToDmg.Damage(this.Damage);
            }
            catch (Exception)
            {
                Debug.LogError("Bad Damage Implemenation");
            }
            finally
            {
                Bullet.Pool.Recycle(this);
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


    public void Enable()
    {
        this.gameObject.SetActive(true);
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
        this.RigidBody.velocity         = Vector3.zero;
        this.RigidBody.angularVelocity  = Vector3.zero;

    }
}
