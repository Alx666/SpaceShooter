using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class StateMachineFactory
{
    public static IAIState Interceptor { get; private set; }


    static StateMachineFactory()
    { 
        //Build Interceptor AI
        AIStateAim  hAim    = new AIStateAim(3f);
        AIStateFire hFire   = new AIStateFire();

        hAim.Success = hFire;
        hAim.Fail    = hAim;
        hFire.Next   = hAim;

        Interceptor = hAim;
    }
}

public interface IAIActor : IDamageable, IPoolable
{
    Rigidbody       Rigidbody   { get; }
    Collider        Collider    { get; }
    IAIActor        Target      { get; set; }
    Transform       Transform   { get; }
    float           EngineForce { get; }
    float           TurnForce   { get; }
    List<IWeapon>   Weapons     { get; }
}
