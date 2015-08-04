using UnityEngine;
using System.Collections.Generic;


public interface IAIActor : IDamageable, IPoolable
{
   Rigidbody Rigidbody { get; }
   Collider Collider { get; }
   IAIActor Target { get; set; }
   Transform Transform { get; }
   float EngineForce { get; }
   float TurnForce { get; }
   List<IWeapon> Weapons { get; }
}
