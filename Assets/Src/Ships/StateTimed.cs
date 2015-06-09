using UnityEngine;
using System.Collections;
using System.Collections.Generic;


internal abstract class StateTimed : IEnemyState
{
    private float m_fTimeToChange;
    private float m_fTimeLeft;
    private List<IEnemyState> m_hNexts;

    public StateTimed(float fTime)
    {
        m_fTimeToChange = fTime;
        m_fTimeLeft = m_fTimeToChange;
        m_hNexts = new List<IEnemyState>();
    }

    public virtual IEnemyState OnUpdate()
    {
        m_fTimeLeft -= Time.deltaTime;

        if (m_fTimeLeft <= 0.0f)
        {
            m_fTimeLeft = m_fTimeToChange;

            return this.ChangeState();
        }
        else
        {
            return this;
        }
    }

    public abstract void OnFixedUpdate(Enemy hShip);

    protected virtual IEnemyState ChangeState()
    {
        return m_hNexts[UnityEngine.Random.Range(0, m_hNexts.Count)];
    }

    public void Add(IEnemyState hNext)
    {
        m_hNexts.Add(hNext);
    }

    protected static float TurnDirection(Vector3 vShipPosition, Vector3 vShipRight, Vector3 vPlayerPosition)
    {
        float sign = Mathf.Sign(Vector3.Angle(vShipPosition, vPlayerPosition));

        Vector3 v1 = vShipPosition;
        Vector3 v2 = vPlayerPosition;
        Vector3 v3 = v2 - v1;

        return Mathf.Sign(Vector3.Dot(v3.normalized, vShipRight));
    }
}