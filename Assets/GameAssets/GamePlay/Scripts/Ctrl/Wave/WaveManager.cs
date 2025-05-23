using PathCreation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;



public class WaveManager : GameMonoBehaviour
{
    [SerializeField] protected float startDelay = 2f;
    public float StartDelay => startDelay;
    [SerializeField] protected List<PathCreator> _paths;
    [SerializeField] protected List<Transform> _spawnedUnits;
    [SerializeField] protected List<SubRoom> _subRoom;

    [Space]
    [SerializeField] protected State currentState;
    public State CurrentState => currentState;

    [Space]
    [SerializeField] protected string enemyName = "no-name";
    [SerializeField] private float _unitSpeed = 2f;

    [Space]
    [Header("SetUp Formation")]
    [SerializeField]
    protected TypeSetUpWave typeSetUpWave;
    protected int curIndexRoom = 1;
    [SerializeField]
    protected float delayChangeSetUp;

    protected bool isWaveSpawnComplete = false;
    protected bool isAllSpawnedUnitsDead = false;
    protected int amountOfUnit = 1;
    //protected bool isSpawn;

    protected override void OnEnable()
    {
        base.OnEnable();

    }
    protected override void Start()
    {
        base.Start();
    }

    protected virtual void Update()
    {
        this.CheckOnWaveCompleted();
        this.CheckOnAllUnitDead();
        this.CheckIsWaveSpawnComplete();
        this.CheckOnFormationCompleted();
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
    }
    private void LoadPaths()
    {
        if (_paths.Count > 0) _paths.Clear();
        Transform prefabsObj = transform.Find("MovePaths");
        foreach (Transform prefab in prefabsObj)
        {
            this._paths.Add(prefab.GetComponent<PathCreator>());
        }
        Debug.Log(transform.name + ": LoadPrefabs", gameObject);
    }

    protected virtual void CheckOnWaveCompleted()
    {
        if (this.currentState == State.NotStarted) return;
        if (!this.isWaveSpawnComplete) return;
        if (!this.isAllSpawnedUnitsDead) return;
        this.currentState = State.Completed;
    }

    public virtual void StartWave()
    {
        if (this.currentState == State.NotStarted)
        {
            this.currentState = State.Started;
            StartCoroutine(this.StartSpawn());
        }
    }

    protected virtual IEnumerator StartSpawn()
    {
        yield return new WaitForSeconds(this.startDelay);
        StartCoroutine(this.SpawnEnemyRandom());

    }

    protected Dictionary<PathCreator, int> GetPathAndAmount(int posCount, int pathCount)
    {
        // calculate the number of times n can be divided equally among the elements of lst
        int amountDivided = posCount / pathCount;
        // calculate the remainder of the division
        int amountRemainder = posCount % pathCount;
        // create the dictionary
        Dictionary<PathCreator, int> movePaths = new Dictionary<PathCreator, int>();
        for (int i = 0; i < pathCount; i++)
        {
            int value = amountDivided;
            if (amountRemainder > 0) // distribute remainder
            {
                value += 1;
                amountRemainder -= 1;
            }
            movePaths[_paths[i]] = value;
        }
        return movePaths;
    }

    protected virtual IEnumerator SpawnEnemyRandom()
    {
        int posCount = this.amountOfUnit; // example integer
        int pathCount = this._paths.Count;
        // create the dictionary
        Dictionary<PathCreator, int> movePaths = this.GetPathAndAmount(posCount, pathCount);
        while (movePaths.Count > 0)
        {
            var ramdomPath = movePaths.ElementAt(Random.Range(0, movePaths.Count));
            if (ramdomPath.Value <= 0)
            {
                movePaths.Remove(ramdomPath.Key);
                continue;
            }
            if (this.SpawnEnemyInPath(ramdomPath.Key))
            {
                movePaths[ramdomPath.Key] -= 1;
                yield return new WaitForSeconds(3f);
            }
        }
        this.isWaveSpawnComplete = true;
    }

    private int index;
    protected virtual bool SpawnEnemyInPath(PathCreator movePath)
    {
        Transform newEnemy;
        /*if (isSpawn)
        {
            newEnemy = _spawnedUnits[index];
            index++;
        } else
        {
            Vector3 spawnPos = movePath.path.GetPoint(0);
            Quaternion enemyRot = Quaternion.Euler(0, 0, 0);
            *//*Transform*//* newEnemy = EnemySpawner.Instance.Spawn(enemyName, spawnPos, enemyRot);
            if (newEnemy == null) return false;
            if (!this._spawnedUnits.Contains(newEnemy))
            {
                this._spawnedUnits.Add(newEnemy);
                this.distanceTravelled.Add(0);
                this.isFollowPathDone.Add(false);
            }
            newEnemy.gameObject.SetActive(true);
        }*/
        Vector3 spawnPos = movePath.path.GetPoint(0);
        Quaternion enemyRot = Quaternion.Euler(0, 0, 0);
        /*Transform*/
        newEnemy = EnemySpawner.Instance.Spawn(enemyName, spawnPos, enemyRot);
        if (newEnemy == null) return false;
        if (!this._spawnedUnits.Contains(newEnemy))
        {
            this._spawnedUnits.Add(newEnemy);
            this.distanceTravelled.Add(0);
            this.isFollowPathDone.Add(false);
        }
        newEnemy.gameObject.SetActive(true);

        StartCoroutine(MoveOnPath(newEnemy, movePath));
        return true;
    }


    public void CheckOnAllUnitDead()
    {
        if (!this.isWaveSpawnComplete) return;
        foreach (var spawnedUnit in this._spawnedUnits)
        {
            if (!EnemySpawner.Instance.CheckObjectInPool(spawnedUnit))
                return;
        }
        this._spawnedUnits.Clear();
        this.isAllSpawnedUnitsDead = true;
    }

    protected void CheckIsWaveSpawnComplete()
    {
        if (this._spawnedUnits.Count == amountOfUnit)
        {
            isWaveSpawnComplete = true;
        }
    }

    protected List<float> distanceTravelled = new List<float>();
    protected List<bool> isFollowPathDone = new List<bool>();
    protected IEnumerator MoveOnPath(Transform unit, PathCreator path)
    {
        int index = _spawnedUnits.IndexOf(unit);
        if (index < 0 || index >= _spawnedUnits.Count)
        {
            yield break;
        }

        if (path.path.length <= 0)
        {
            isFollowPathDone[index] = true;
            yield break;
        }

        while (!isFollowPathDone[index])
        {
            distanceTravelled[index] += _unitSpeed * Time.deltaTime;
            _spawnedUnits[index].position = path.path.GetPointAtDistance(distanceTravelled[index], EndOfPathInstruction.Loop);

            if (distanceTravelled[index] >= path.path.length)
            {
                if (typeSetUpWave == TypeSetUpWave.Loop)
                {
                    distanceTravelled[index] = 0f; // loop
                }
                else
                {
                    isFollowPathDone[index] = true;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private bool hasFormationCompleted = false;
    protected virtual void CheckOnFormationCompleted()
    {
        if (!isWaveSpawnComplete) return;
        if (isFollowPathDone.Count == 0) return;
        if (hasFormationCompleted) return;

        if (isFollowPathDone.All(done => done))
        {
            hasFormationCompleted = true;
            Debug.Log("✅ Tất cả enemy đã hoàn thành xếp đội hình!");
        }
    }

/*    [Button("Test")]
    protected virtual void ChangeWaveUsingPath()
    {
        isSpawn = true;
        Debug.Log("curIndexRoom " + curIndexRoom);

        if (curIndexRoom >= _roomWave.Count) return;

        //_paths = _roomWave[curIndexRoom].paths;

        if (curIndexRoom - 1 < _roomWave.Count)
            _roomWave[curIndexRoom - 1].gameObject.SetActive(false);
        _roomWave[curIndexRoom].gameObject.SetActive(true);

        distanceTravelled.Clear();
        isFollowPathDone.Clear();

        for (int i = 0; i < _spawnedUnits.Count; i++)
        {
            var unit = _spawnedUnits[i];
            var path = _paths[i % _paths.Count];
            distanceTravelled.Add(0);
            isFollowPathDone.Add(false);
            StartCoroutine(MoveOnPath(unit, path));
        }

        // Reset trạng thái
        isWaveSpawnComplete = true;
        hasFormationCompleted = false;
        curIndexRoom++;
    }

    [Button("Test")]
    protected virtual void ChangeWaveUsingPath()
    {
        isSpawn = true;
        curIndexRoom++;
        Debug.Log("curIndexRoom " + curIndexRoom);
        if (curIndexRoom > _roomWave.Count - 1) return;
        _paths = _roomWave[curIndexRoom].paths;

        _roomWave[curIndexRoom - 1].gameObject.SetActive(false);
        _roomWave[curIndexRoom].gameObject.SetActive(true);


    }

    [Button("Test Path To Path")]
    protected virtual void ChangePathNew()
    {
        if (_pathWaves == null || _pathWaves.Count < indexPath + 2)
        {
            Debug.LogWarning("Không đủ path trong _pathWaves để thay thế.");
            return;
        }
        indexPath += 2;

        _paths.Clear();
        _paths.Add(_pathWaves[indexPath]);
        _paths.Add(_pathWaves[indexPath + 1]);

        Debug.Log($"🔁 Thay path bằng path {indexPath} và {indexPath + 1}");

        distanceTravelled.Clear();
        isFollowPathDone.Clear();

        StartCoroutine(MoveUnitsToNewPaths());

        hasFormationCompleted = false;

        isWaveSpawnComplete = true;
    }*/

    protected virtual IEnumerator MoveUnitsToNewPaths()
    {
        for (int i = 0; i < _spawnedUnits.Count; i++)
        {
            var unit = _spawnedUnits[i];
            var path = _paths[i % _paths.Count]; 

            distanceTravelled.Add(0f);
            isFollowPathDone.Add(false);

            StartCoroutine(MoveOnPath(unit, path));

            yield return new WaitForSeconds(0.25f);
        }
    }

}
