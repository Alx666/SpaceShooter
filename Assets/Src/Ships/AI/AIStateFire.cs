using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AIStateFire : IAIState
{
    public IAIState Next { get; set; }
    private bool m_bToRelease;

    public void OnFixedUpdate(IAIActor hShip)
    {
        
    }

    public IAIState Update(IAIActor hShip)
    {
        if (m_bToRelease)
        {
            hShip.Weapons.ForEach(hW => hW.OnbuttonReleased());
            m_bToRelease = false;
        }

        bool bWeaponsFiring = true;
        for (int i = 0; i < hShip.Weapons.Count; i++)
        {
            bWeaponsFiring &= hShip.Weapons[i].IsFiring;
        }

        if (bWeaponsFiring)
        {
            return this;
        }
        else
        {
            Next.OnStateEnter(hShip, this);
            return Next;
        }
    }


    public void OnStateEnter(IAIActor hActor, IAIState hPrevState)
    {
        hActor.Weapons.ForEach(hW => hW.OnbuttonPressed());
        m_bToRelease = true;
    }
}