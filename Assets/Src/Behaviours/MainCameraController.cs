using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour {

    public ParticleSystem HyperSpace;
    public AudioSource SweepSFX;
    public AudioSource BattleBegin;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCameraZoomOut()
    {
        HyperSpace.enableEmission = true;
        HyperSpace.Play();
    }

    void OnAnimationCompleted()
    {
        PlayerController.Instance.EnableControl = true;
        BattleBegin.Play();
    }

    public void OnSweepSFX()
    {
        SweepSFX.Play();
    }
}
