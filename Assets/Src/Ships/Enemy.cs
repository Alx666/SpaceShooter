using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(WorldController))]
public class Enemy : MonoBehaviour, IAIActor
{
    public GameObject   ExplosionPrefab;
    public float        Hp                  = 100f;
    public float        SideThrustersPower  = 10f;
    public float        EnginePower         = 10f;
    public float        ImpactDamage        = 0.0f;
    public float        ChanceToSpawn       = 1.0f;
    public List<GameObject> OnDeathSpawns;
    #region IAIActor

    public Rigidbody    Rigidbody   { get; private set; }
    public Collider     Collider    { get; private set; }
    public IAIActor     Target      { get; set; }
    public Transform    Transform   { get { return this.transform; } }
    public float        EngineForce { get { return EnginePower;} }
    public float        TurnForce   { get { return SideThrustersPower; } }
    public List<IWeapon> Weapons    { get; private set; }

    #endregion

    private WorldController m_hWorldController;
    

    void Awake()
    {
        this.Rigidbody              = this.GetComponent<Rigidbody>();
        this.Collider               = this.GetComponent<Collider>();

        m_hWorldController          = this.gameObject.GetComponent<WorldController>();
        m_hWorldController.enabled  = false;

        this.Health = Hp;

        //m_hAI = StateMachineFactory.Interceptor;

        Weapons = this.GetComponentsInChildren<IWeapon>().ToList();
    }

    void Start()
    {
        this.Target = PlayerController.Instance;
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }


    void OnCollisionEnter(Collision hColl)
    {
        if (ImpactDamage <= 0.0f)
            return;

        IDamageable hDamageable = hColl.gameObject.GetComponent<IDamageable>();
        Shield hShield = hColl.gameObject.GetComponentInChildren<Shield>();

        if (hDamageable != null)
        {
            if(hShield == null || !hShield.Active)
                hDamageable.Damage(this.ImpactDamage);

            this.Damage(this.ImpactDamage);
        }
    }


    public float Health { get; private set; }

    public void Damage(float fDmg)
    {
        this.Health -= fDmg;
        if (this.Health < 0f)
            this.Destroy();
    }

    public void Destroy()
    {

        GameObject hExplosion = GlobalFactory.GetInstance(ExplosionPrefab);
        hExplosion.transform.position = this.transform.position;

        if (OnDeathSpawns.Count > 0f && Random.Range(0f, 1f) <= this.ChanceToSpawn)
        {
            GameObject hBonus = GlobalFactory.GetInstance(OnDeathSpawns[Random.Range(0, OnDeathSpawns.Count)]);
            hBonus.transform.position = this.gameObject.transform.position;
            Debug.Log("Spawn");
        }

        this.Pool.Recycle(this.gameObject);
    }

    public Pool Pool { get; set; }

    public void Enable()
    {
        this.Health = Hp;
        this.gameObject.SetActive(true);
        GameManager.Instance.RegisterForWaveEnd(this);
    }

    public void Disable()
    {
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);

        GameManager.Instance.UnregisterForWaveEnd(this);
    }



}
