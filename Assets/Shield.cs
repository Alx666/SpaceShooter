using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class Shield : MonoBehaviour
{
    public Light ShieldLightStatic;
    public Light ShieldLightDynamic;
    public Animator ShieldEnergyFX;
    public AudioSource AudioSFX;

    private Collider m_hCollider;
    private Collider m_hParentCollider;
    private Renderer m_hRenderer;

    public float ActivationTime = 3f;
    public float CurrentActivationTime;
    public float RechargeCoeff = 1.0f;
    public float ExaustedRecoverTime = 1.0f;

    private bool m_bExausted;

    void Start()
    {
        m_hCollider = this.gameObject.GetComponent<Collider>();
        m_hRenderer = this.gameObject.GetComponent<Renderer>();
        m_hParentCollider = this.gameObject.transform.parent.GetComponent<Collider>();

        m_hRenderer.enabled = false;
        m_hCollider.enabled = false;

        CurrentActivationTime = ActivationTime;
    }


    void Update()
    {
        if (Input.GetMouseButton(1) && CurrentActivationTime > 0.0f)
        {
            if (!m_bExausted)
            {
                m_hRenderer.enabled = true;
                m_hCollider.enabled = true;
                m_hParentCollider.enabled = false;
                ShieldLightStatic.enabled = true;
                ShieldLightDynamic.enabled = true;
                ShieldEnergyFX.SetTrigger("ShieldEnabled");
                if (!AudioSFX.isPlaying)
                    AudioSFX.Play();

                if (PlayerController.Instance != null)
                    PlayerController.Instance.ShieldEnabled = true;
            }

            CurrentActivationTime -= Time.deltaTime;

            if (CurrentActivationTime <= 0f)
            {
                m_bExausted = true;
            }
        }
        else
        {
            m_hRenderer.enabled = false;
            m_hCollider.enabled = false;
            m_hParentCollider.enabled = true;
            ShieldLightStatic.enabled = false;
            ShieldLightDynamic.enabled = false;
            ShieldEnergyFX.SetTrigger("ShieldDisabled");
            AudioSFX.Stop();

            if (PlayerController.Instance != null)
                PlayerController.Instance.ShieldEnabled = false;

            if (CurrentActivationTime < ActivationTime)
            {
                CurrentActivationTime += Time.deltaTime * RechargeCoeff;

                if (CurrentActivationTime >= ExaustedRecoverTime && m_bExausted)
                    m_bExausted = false;
            }
            else
            {
                CurrentActivationTime = ActivationTime;
            }
        }

    }
}
