using UnityEngine;
using System.Collections;

internal interface IEnemyState
{
    void OnFixedUpdate(Enemy hShip);
    IEnemyState OnUpdate();
}