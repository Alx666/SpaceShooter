using UnityEngine;
using System.Collections;

public interface IWeapon
{
    void OnbuttonReleased();

    void OnbuttonPressed();

    bool IsFiring { get; }
}
