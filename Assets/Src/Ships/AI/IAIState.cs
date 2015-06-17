using UnityEngine;
using System.Collections;

public interface IAIState
{
    IAIState Update(IAIActor hShip);
    void OnFixedUpdate(IAIActor hShip);
    void OnStateEnter(IAIActor hActor, IAIState hPrevState);
}