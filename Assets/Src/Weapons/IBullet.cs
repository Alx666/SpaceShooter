using UnityEngine;
using System.Collections;

public interface IBullet
{
    void Shoot(Vector3 vPosition, Vector3 vDirection, float fForce, ForceMode eMode);
    Transform Transform { get; }
}
