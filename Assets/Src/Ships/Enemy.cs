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
    private IAIState        m_hAI;

    void Awake()
    {
        this.Rigidbody              = this.GetComponent<Rigidbody>();
        this.Collider               = this.GetComponent<Collider>();

        m_hWorldController          = this.gameObject.GetComponent<WorldController>();
        m_hWorldController.enabled  = false;

        this.Health = Hp;

        m_hAI = StateMachineFactory.Interceptor;

        Weapons = this.GetComponentsInChildren<IWeapon>().ToList();
    }

    void Start()
    {
        this.Target = PlayerController.Instance;
    }

    void Update()
    {
        m_hAI = m_hAI.Update(this);
    }

    void FixedUpdate()
    {
        m_hAI.OnFixedUpdate(this);
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
        
    }

    public Pool Pool { get; set; }

    public void Enable()
    {
        
    }

    public void Disable()
    {
        
    }



}
