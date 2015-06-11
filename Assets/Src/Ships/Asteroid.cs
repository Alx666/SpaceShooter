using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(WorldController))]
public class Asteroid : MonoBehaviour, IDamageable, IPoolable
{       
    public float        ImpactDamage;
    public float        Hp;
    public float        MinForce = 5f;
    public float        MaxForce = 10f;
    public GameObject   ExplosionPrefab;

        
    private Rigidbody       m_hRigidBody;
    private Collider        m_hCollider;
    private WorldController m_hWorldController;


    void Awake()
    {
        m_hRigidBody        = this.GetComponent<Rigidbody>();
        m_hCollider         = this.GetComponent<Collider>();
        m_hWorldController  = this.GetComponent<WorldController>();
        this.Health         = Hp;
    }

    void Start()
    {
        
    }
	
	
	void Update () 
    {
	    
	}

    #region IDamageable


    public void Damage(float fDmg)
    {
        this.Health -= fDmg;

        if (this.Health <= 0f)
            this.Destroy();
    }

    public void Destroy()
    {
        GameObject hExplosion = GlobalFactory.GetInstance(ExplosionPrefab);
        hExplosion.transform.position = this.transform.position;

        this.Pool.Recycle(this.gameObject);
    }

    public float Health { get; private set; }

    #endregion



    #region IPoolable

    public Pool Pool { get; set; }

    public void Enable()
    {
        this.Health = Hp;
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
