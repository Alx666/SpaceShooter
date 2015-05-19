using UnityEngine;
using System.Collections;

public interface IWeapon
{

    void OnUpdate();

    void OnbuttonReleased();


    void OnbuttonPressed();


    bool IsPressed { get; }
}
