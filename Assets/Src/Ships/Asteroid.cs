using UnityEngine;
using System.Collections;



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(WorldController))]
public class Asteroid : MonoBehaviour, IDamageable, IPoolable
{
    public GameObject Explosion;
    static Object resourceExplosion;
    GameObject explosion;


    public float ImpactDamage;
    public float Hp;
    public float MinForce = 5f;
    public float MaxForce = 10f;
    public int   PoolNumber;

        
    private Rigidbody       m_hRigidBody;
    private Collider        m_hCollider;
    private WorldController m_hWorldController;


    void Awake()
    {
        //resourceExplosion = Resources.Load("Explosion_FX");

        m_hRigidBody        = this.GetComponent<Rigidbody>();
        m_hCollider         = this.GetComponent<Collider>();
        m_hWorldController  = this.GetComponent<WorldController>();        
    }

    void Start()
    {
        
    }
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    #region IDamageable


    public void Damage(float fDmg)
    {
        this.Hp -= fDmg;

        if (this.Hp <= 0f)
            this.Destroy();
    }

    public void Destroy()
    {
        this.Pool.Recycle(this);
    }

    public float Health 
    {
        get { return this.Hp; }
    }

    #endregion


    #region IPoolable

    public int PoolId
    {
        get { return PoolNumber; }
    }

    public Pool<IPoolable> Pool { get; set; }

    public void Enable()
    {
        this.gameObject.SetActive(true);
        m_hRigidBody.AddTorque(Random.insideUnitSphere * Random.Range(0, MaxForce), ForceMode.VelocityChange);

        Vector3 vForce = Random.insideUnitCircle * Random.Range(MinForce, MaxForce);
        vForce.z = vForce.y;
        vForce.y = 0f;
        m_hRigidBody.AddForce(vForce, ForceMode.VelocityChange);

        GameManager.Instance.RegisterForWaveEnd(this);
    }

    public void Disable()
    {
        m_hRigidBody.velocity           = Vector3.zero;
        m_hRigidBody.angularVelocity    = Vector3.zero;
        this.gameObject.SetActive(false);

        GameManager.Instance.UnregisterForWaveEnd(this);
    }

    #endregion
}
