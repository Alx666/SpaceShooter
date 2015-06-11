using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public  static  GameManager     Instance            { get; private set; }
    private const   float           OFFSET = 200.0f;


    //Inspector Editable Stuff
    public List<DifficultyLevel>    Difficulties;
    public List<Wave>               Waves;
    public float                    WaveWaitTime = 3.0f;
    public Text                     WaveCount;
    public Text                     GameOver;


    //Public Access Logic
    public int                      CurrentWaveIndex    { get; private set; }
    public DifficultyLevel          CurrentDifficulty   { get; set; }
    
    //Private Stuff      
    private IGameManagerState   m_hGMState;
    private List<Vector3>       m_hSpawnPoints;
    private List<IPoolable>     m_hActiveEnemies;

    
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject); //avvisa unity che al cambio scena non va distrutto

        if (Instance != null)
            throw new Exception("Multiple GameManager Detected");

        Instance = this;

        m_hActiveEnemies = new List<IPoolable>();

        m_hSpawnPoints = new List<Vector3>();
        m_hSpawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.25f)); //riempio lista con punti schermo grezzi
        m_hSpawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.5f));
        m_hSpawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.75f));
        m_hSpawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.75f));
        m_hSpawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.5f));
        m_hSpawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.25f));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.75f, Screen.height + OFFSET));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.5f, Screen.height + OFFSET));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.25f, Screen.height + OFFSET));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.5f, 0.0f - OFFSET));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.25f, 0.0f - OFFSET));
        m_hSpawnPoints.Add(new Vector3(Screen.width * 0.75f, 0.0f - OFFSET));


        for (int i = 0; i < m_hSpawnPoints.Count; i++)
        {
            Ray vRay = Camera.main.ScreenPointToRay(m_hSpawnPoints[i]);

            Vector3 intersection;
            if (RayPlaneIntersection(vRay.origin, vRay.direction, Vector3.zero, Vector3.up, out intersection))
            {
                m_hSpawnPoints[i] = intersection;
            }
        }

        //Setup the whole game
        Waves.ForEach(hW => hW.Initialize());

        //Allocate States
        GMStateWaitNextWave hCountDownState     = new GMStateWaitNextWave(this);
        GMStateSpawn        hSpawnState         = new GMStateSpawn(this);
        GMStateWaitCombat   hWaitCombatState    = new GMStateWaitCombat(this);
        GMStateIntro        hIntro              = new GMStateIntro(this);

        //StateMachine Setup
        hIntro.Next             = hCountDownState;
        hCountDownState.Next    = hSpawnState;
        hSpawnState.Next        = hWaitCombatState;
        hWaitCombatState.Next   = hCountDownState;
        m_hGMState              = hIntro;

        this.Intro = true;

        
        WaveCount.text = "Wave: " + CurrentWaveIndex.ToString();
        GameObject.Destroy(this.gameObject.GetComponent<BoxCollider>());      
    }


    void Start()
    {
        this.transform.position = Vector3.zero;
        this.CurrentDifficulty = Difficulties[0];
    }

    void Update()
    {
        m_hGMState = m_hGMState.Update();
    }

    #region Misc

    public void RegisterForWaveEnd(IPoolable hItem)
    {
        m_hActiveEnemies.Add(hItem);
    }

    public void UnregisterForWaveEnd(IPoolable hItem)
    {
        m_hActiveEnemies.Remove(hItem);
    }

    public bool WaveCompleted { get { return  m_hActiveEnemies.Count == 0; } }

    public bool Intro { get; set; }    

    private Wave GetNextWave()
    {
        Wave hWave = Waves[CurrentWaveIndex];
        CurrentWaveIndex++;
        return hWave;
    }

    public Vector3 GetRandomSpawnPosition()
    {
        return this.m_hSpawnPoints[UnityEngine.Random.Range(0, this.m_hSpawnPoints.Count)];
    }


    private bool RayPlaneIntersection(Vector3 rayOrigin, Vector3 direction, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
    {
        float d = Vector3.Dot((planePoint - rayOrigin), planeNormal) / Vector3.Dot(direction, planeNormal);
        if (float.IsInfinity(d))
        {
            intersection = Vector3.zero;
            return false;
        }
        intersection = d * direction + rayOrigin;
        return true;
    }

    #endregion

    #region Game Manager State Machine

    private interface IGameManagerState
    {
        IGameManagerState Update();

        IGameManagerState Next { get; set; }

        void OnStateEnter();
    }
    private class GMStateWaitNextWave : IGameManagerState
    {
        private GameManager m_hOwner;
        private float m_fWaitTime;
        private float m_fCurrentWaitTime;


        public GMStateWaitNextWave(GameManager hThis)
        {
            m_hOwner            = hThis;
            m_fWaitTime         = m_hOwner.WaveWaitTime;
            m_fCurrentWaitTime  = m_fWaitTime;
        }

        public IGameManagerState Update()
        {
            if (m_fCurrentWaitTime > 0.0f)
            {
                m_fCurrentWaitTime -= Time.deltaTime;
                m_hOwner.WaveCount.text = m_fCurrentWaitTime.ToString();
                return this;
            }
            else
            {
                m_fCurrentWaitTime = m_fWaitTime;
                Next.OnStateEnter();
                return Next;
            }
        }

        public void OnStateEnter()
        {
            
        }

        public IGameManagerState Next { get; set; }
    }

    private class GMStateSpawn : IGameManagerState
    {
        private GameManager m_hOwner;
        private Wave        m_hCurrentWave;        
        


        public GMStateSpawn(GameManager hThis)
        {
            m_hOwner = hThis;
        }

        public IGameManagerState Update()
        {
            if (!m_hCurrentWave.SpawnComplete)
            {
                m_hCurrentWave.Spawn();
                return this;
            }
            else
            {
                m_hCurrentWave = null;
                Next.OnStateEnter();
                return Next;
            }
        }

        public void OnStateEnter()
        {
            m_hCurrentWave = m_hOwner.GetNextWave();                                  
        }

        public IGameManagerState Next { get; set; }
    }



    private class GMStateWaitCombat : IGameManagerState
    {
        private GameManager m_hOwner;
        public GMStateWaitCombat(GameManager hThis)
        {
            m_hOwner = hThis;
        }

        public IGameManagerState Update()
        {
            if (!m_hOwner.WaveCompleted)
                return this;
            else
            {
                Next.OnStateEnter();
                return Next;
            }
        }

        public void OnStateEnter()
        {
        }

        public IGameManagerState Next { get; set; }
    }

    private class GMStateIntro : IGameManagerState
    {
        private GameManager m_hOwner;
        public GMStateIntro(GameManager hThis)
        {
            m_hOwner = hThis;
        }

        public IGameManagerState Update()
        {
            if (m_hOwner.Intro)
            {
                return this;
            }
            else
            {
                Next.OnStateEnter();
                return Next;
            }
        }

        public void OnStateEnter()
        {
        }

        public IGameManagerState Next { get; set; }
    }

    #endregion

    #region Wave Configuration

    [Serializable]
    public class DifficultyLevel
    {
        public string Name;

        [Range(0.01f, 3.0f)]
        public float TurnCoeff;
        [Range(0.01f, 3.0f)]
        public float SpeedCoeff;
        [Range(0.01f, 3.0f)]
        public float HpCoeff;
        [Range(0.01f, 3.0f)]
        public float ShieldCoeff;
        [Range(0.01f, 3.0f)]
        public float DmgCoeff;
    }


    [Serializable]
    public class ShipSpawn
    {
        public GameObject ShipResource;

        public int  Max;
        public int  Min;        
        public bool OverTime;
        public int  Rate;

        [NonSerialized]
        public int Count;

        private ISpawnStrategy m_hSpawner;


        public void Initialize()
        {
            Count         = UnityEngine.Random.Range(Min, Max + 1);

            if (OverTime)
                m_hSpawner = new SpawnOverTime(ShipResource, Count, Rate);
            else
                m_hSpawner = new SpawnToNumber(ShipResource, Count, Rate);

        }

        public void Spawn()
        {
            m_hSpawner.Spawn();
        }

        
        public interface ISpawnStrategy
        {
            void Spawn();

            bool IsComplete { get; }
        }

        public bool IsComplete { get { return m_hSpawner.IsComplete; } }

        public class SpawnOverTime : ISpawnStrategy
        {
            private Pool    m_hPool;
            private int     m_iTotalToSpawn;
            private float   m_fSpawnTime;
            private float   m_fCurrTime;

            public SpawnOverTime(GameObject hResource, int iCount, int iRate)
            {
                m_hPool         = GlobalFactory.GetPool(hResource);
                m_iTotalToSpawn = iCount;
                m_fSpawnTime    = (float)iRate;
                m_fCurrTime     = m_fSpawnTime;

            }

            public void Spawn()
            {
                if (m_iTotalToSpawn > 0)
                {
                    if (m_fCurrTime < 0f)
                    {
                        GameObject hSpawn = m_hPool.Get();


                        hSpawn.transform.position = GameManager.Instance.GetRandomSpawnPosition();
                        m_iTotalToSpawn--;
                        m_fCurrTime = m_fSpawnTime;
                    }
                    else
                    {
                        m_fCurrTime -= Time.deltaTime;
                    } 
                }
            }


            public bool IsComplete
            {
                get { return m_iTotalToSpawn == 0; }
            }
        }

        public class SpawnToNumber : ISpawnStrategy
        {
            private int m_iTotalToSpawn;
            private int m_iMaxConcurrent;
            private Pool m_hPool;

            public SpawnToNumber(GameObject hResource, int iCount, int iMaxConcurrent)
            {
                m_iTotalToSpawn     = iCount;
                m_iMaxConcurrent    = iMaxConcurrent;
                m_hPool             = GlobalFactory.GetPool(hResource);
            }

            public void Spawn()
            {
                while (m_iTotalToSpawn > 0 && m_iMaxConcurrent > m_hPool.ActiveInstances)
                {
                    GameObject hItem = m_hPool.Get();                    

                    int iIndex = UnityEngine.Random.Range(0, GameManager.Instance.m_hSpawnPoints.Count);
                    hItem.transform.position = GameManager.Instance.m_hSpawnPoints[iIndex];
                    m_iTotalToSpawn--;
                }
            }

            public bool IsComplete
            {
                get { return m_iTotalToSpawn == 0; }
            }
        }
    }


    [Serializable]
    public class Wave
    {
        public List<ShipSpawn> ShipSpawns;


        public void Initialize()
        {
            ShipSpawns.ForEach(hS => hS.Initialize());
        }
        
        public void Spawn()
        {
            for (int i = 0; i < ShipSpawns.Count; i++)
            {
                ShipSpawns[i].Spawn();

                if (ShipSpawns[i].IsComplete)
                {
                    ShipSpawns.RemoveAt(i);
                }
            }
        }

        public bool SpawnComplete { get { return ShipSpawns.Count == 0; } }
    }

    #endregion

    #region Score Update

    public Text ScoreLabel;
    public int TotalScore { get; set; }
    internal void AddScore(int p)
    {
        //TotalScore += p;

        //if (ScoreLabel != null)
        //    ScoreLabel.text = "Player-1: " + TotalScore.ToString();
    }

    #endregion


}

