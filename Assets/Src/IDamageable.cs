using UnityEngine;
using System.Collections;

public interface IDamageable
{
    float Health { get; }

    void Damage(float fDmg);

    void Destroy();
}
