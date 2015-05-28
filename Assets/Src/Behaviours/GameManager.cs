using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


//Information Expert un oggetto che crea tutte le istanze dei gameobject che servono nella scena
// anche chiamato Creator
public class GameManager : MonoBehaviour
{
    public delegate void WaveCompletedHandler();
    public event WaveCompletedHandler WaveCompleted;

    public List<DifficultyLevel> Difficulty;
    private List<ShipSpawn> Spawns;
    public List<Wave> Waves;
    public Text WaveCount;
    public Text GameOver;

    public static GameManager Instance { get; private set; }



    List<Vector3> spawnPoints;

    const float OFFSET = 200.0f;

    public DifficultyLevel CurrentDifficulty { get; set; }
    public int currentWaveIndex = 0;

    private List<AIController> m_hEnemies;


    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject); //avvisa unity che al cambio scena non va distrutto

        if (Instance != null)
            throw new Exception("Multiple GameManager Detected");

        Instance = this;

        m_hEnemies = new List<AIController>();

        spawnPoints = new List<Vector3>();
        spawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.25f)); //riempio lista con punti schermo grezzi
        spawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.5f));
        spawnPoints.Add(new Vector3(Screen.width + OFFSET, Screen.height * 0.75f));
        spawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.75f));
        spawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.5f));
        spawnPoints.Add(new Vector3(0.0f - OFFSET, Screen.height * 0.25f));
        spawnPoints.Add(new Vector3(Screen.width * 0.75f, Screen.height + OFFSET));
        spawnPoints.Add(new Vector3(Screen.width * 0.5f, Screen.height + OFFSET));
        spawnPoints.Add(new Vector3(Screen.width * 0.25f, Screen.height + OFFSET));
        spawnPoints.Add(new Vector3(Screen.width * 0.5f, 0.0f - OFFSET));
        spawnPoints.Add(new Vector3(Screen.width * 0.25f, 0.0f - OFFSET));
        spawnPoints.Add(new Vector3(Screen.width * 0.75f, 0.0f - OFFSET));


        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Ray ray1 = Camera.main.ScreenPointToRay(spawnPoints[i]);//trasforma un punto 2D in un raggio
            RaycastHit hHit;

            Vector3 intersection;
            if (RayPlaneIntersection(ray1.origin, ray1.direction, Vector3.zero, Vector3.up, out intersection))
            {
                spawnPoints[i] = intersection; //sostituisco con punto 3D fatto bene
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = spawnPoints[i];
            }
        }


        Spawns = new List<ShipSpawn>();

        GameObject.Destroy(this.gameObject.GetComponent<BoxCollider>());

        WaveCount.text = "Wave: " + currentWaveIndex.ToString();
        

        //fare gestione wave successive
        //currentWave = Waves[0];
    }

    bool RayPlaneIntersection(Vector3 rayOrigin, Vector3 direction, Vector3 planePoint, Vector3 planeNormal, out Vector3 intersection)
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

    void Start()
    {
        this.transform.position = Vector3.zero;
        this.CurrentDifficulty = Difficulty[0];
        //this.currentWave = Waves[0];
        //Calcolo centrale destro
    }

    void Update()
    {
        return;
        //ScoreLabel.text = Bullet.Pool.Count.ToString();
        
        //if (CurrentDifficulty == null)
        //    return;

        //if (PlayerController.Instance == null)
        //    GameOver.text = "Game Over";

        //bool allClear = true;

        //for (int i = 0; i < m_hEnemies.Count; i++)
        //{
        //    if (m_hEnemies[i] != null)
        //    {
        //        allClear = false;
        //        break;
        //    }
        //}
       
        //if (allClear)
        //{
        //    m_hEnemies.Clear();

        //    Waves[currentWaveIndex].Spawn(spawnPoints);

        //    if (WaveCompleted != null)
        //        WaveCompleted();

        //    currentWaveIndex++;
        //    WaveCount.text = "Wave: " + currentWaveIndex.ToString();
        //}

    }

    //aggiornare con AI di Alex
    public void RegisterEnemy(AIController enemy)
    {
        m_hEnemies.Add(enemy);
    }

    private IEnumerator WaitForTeleportEnable(WorldController wController)
    {
        yield return new WaitForSeconds(3.0f);
        if (wController != null)
            wController.enabled = true;
    }


    //informa il runtime di c# di salvare tutte le info dello script in modo che possa essere convertito in binario
    // disabilita alcune ottimizzazioni e salva le varie istanza della classe serializzata
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
        public int Max;
        public int Min;

        public UnityEngine.Object ShipResource;


        public int GetAmount()
        {
            int amount = UnityEngine.Random.Range(Min, Max + 1);
            return amount;
        }

        //[Range(0.0f, 1.0f)]
        //public float Chance;
        //public float ThreatPts;
    }


    [Serializable]
    public class Wave
    {
        public List<ShipSpawn> ShipSpawns = new List<ShipSpawn>();

        public void Spawn(List<Vector3> spawnPoints)
        {
            //List<Vector3> sPoints = new List<Vector3>(spawnPoints);

            //foreach (var s in ShipSpawns)
            //{
            //    for (int i = 0; i < s.GetAmount(); i++)
            //    {
            //        GameObject obj = UnityEngine.Object.Instantiate(s.ShipResource) as GameObject;
            //        int spawnPointIndex = UnityEngine.Random.Range(0, sPoints.Count);
            //        obj.transform.position = sPoints[spawnPointIndex];
            //        sPoints.RemoveAt(spawnPointIndex);

            //        obj.transform.LookAt(PlayerController.Instance.transform);

            //    }
            //}
        }
    }





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
