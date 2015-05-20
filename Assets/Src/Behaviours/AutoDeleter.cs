using UnityEngine;
using System.Collections;

public class AutoDeleter : MonoBehaviour 
{

    private ParticleSystem m_hParticles;
	// Use this for initialization
	void Start () 
    {
        m_hParticles = this.GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!m_hParticles.IsAlive())
            GameObject.Destroy(this.gameObject);
	}
}
