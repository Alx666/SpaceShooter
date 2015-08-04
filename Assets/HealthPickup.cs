using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour, IPoolable
{
    private GameObject m_hInnerCube;
    private Rigidbody m_hRigidbody;

    private Vector3 m_vInnerAngles;
    private Vector3 m_vOuterAngles;

    public float RepairPoint = 25f;

	void Awake () 
    {
        m_hInnerCube = this.transform.GetChild(0).gameObject;
        m_hRigidbody = this.GetComponent<Rigidbody>();
	}

	void Update () 
    {
        m_hInnerCube.transform.Rotate(m_vInnerAngles);
	}


    void OnCollisionEnter(Collision hColl)
    {
        IAIActor hActor = hColl.gameObject.GetComponent<IAIActor>();
        
        if (hActor != null && hActor is PlayerController)
        {
            hActor.Damage(-this.RepairPoint);                

            this.Pool.Recycle(this.gameObject);
        }
    }



    public Pool Pool { get; set; }

    public void Enable()
    {
        this.gameObject.SetActive(true);
        
        m_vInnerAngles = new Vector3(Random.Range(0f, 5f), Random.Range(0f, 5f), Random.Range(0f, 5f));
        m_vOuterAngles = new Vector3(Random.Range(0f, 5f), Random.Range(0f, 5f), Random.Range(0f, 5f));
        m_hRigidbody.AddTorque(m_vOuterAngles, ForceMode.VelocityChange);
        m_hRigidbody.AddForce(new Vector3(Random.Range(0f, 5f), 0f, Random.Range(0f, 5f)), ForceMode.VelocityChange);

        GameManager.Instance.RegisterForWaveEnd(this);
    }

    public void Disable()
    {
        m_hRigidbody.velocity = Vector3.zero;
        m_hRigidbody.angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);        
    }
}
