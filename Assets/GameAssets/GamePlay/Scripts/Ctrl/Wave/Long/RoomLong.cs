using PathCreation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomLong : WaveLongManager
{

    [SerializeField] private float _unitFormationSpeed = 2f;
    private List<Vector3> _formationPoints;

    private List<float> _unitOscillatesSpeeds = new List<float>();
    private float amplitudeOscillates = 0.03f;
    private float curActiveSubRoomNew;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadFormation();
    }

    protected override void Start()
    {
        base.Start();
        this.curActiveSubRoomNew = delayChangeSetUp;
        this.amountOfUnit = this._formation.GetPositions().Count;
    }

    protected override void Update()
    {
        base.Update();
        this.SetUpFormation();
        this.SetUpSubRoomNew();
    }

    private void SetUpSubRoomNew()
    {
        if(curActiveSubRoomNew <= 0)
        {
            curIndexRoom++;
            if (curIndexRoom >= _subRoom.Count)
            {
                Debug.Log("No more SubRooms. End of wave sequence.");
                return;
            }

            _subRoom[curIndexRoom].gameObject.SetActive(true);

            curActiveSubRoomNew = delayChangeSetUp;
        } else
        {
            curActiveSubRoomNew -= Time.deltaTime;
        }
        //SubRoom nextRoom = _subRoom[curIndexRoom];
    }

    private void LoadFormation()
    {
        if (_formation != null) return;
        _formation = transform.GetComponentInChildren<FormationBase>();
        Debug.Log(transform.name + ": LoadFormation", gameObject);
    }


    protected override IEnumerator StartSpawn()
    {
        yield return null;
        this.SpawnEnemy();
    }

    protected virtual void SpawnEnemy()
    {
        int posCount = this.amountOfUnit; // example integer
        int pathCount = this._paths.Count;
        // create the dictionary
        Dictionary<PathCreator, int> movePaths = this.GetPathAndAmount(posCount, pathCount);
        foreach (var item in movePaths)
        {
            StartCoroutine(SpawnEnemyEachPath(item.Key, item.Value));
        }
    }

    protected virtual IEnumerator SpawnEnemyEachPath(PathCreator movePath, int numEnemies)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            if (this.SpawnEnemyInPath(movePath))
            {
                this._unitOscillatesSpeeds.Add(Random.Range(0.05f, 0.08f));
                yield return new WaitForSeconds(0.15f);
            }
            else
            {
                i--;
            }
        }
        this.isWaveSpawnComplete = true;
    }

    protected void SetUpFormation()
    {
        switch (typeSetUpWave)
        {
            case TypeSetUpWave.SetUpWave:
                FormationWave();
                break;
            case TypeSetUpWave.SetUpPath:
                break;
        }
    }

    public void FormationWave()
    {
        if (this.isAllUnitInFormation) return;
        this.SetFormationPoints(this._formation.GetPositions().ToList());
        for (var i = 0; i < _spawnedUnits.Count; i++)
        {
            this._spawnedUnits[i].transform.position = Vector3.MoveTowards(this._spawnedUnits[i].transform.position, this._formationPoints[i], this._unitFormationSpeed * Time.deltaTime);
        }
        this.CheckOnAllUnitInFormation();
    }

    private void SetFormationPoints(List<Vector3> points)
    {
        this._formationPoints = points.ToList();

    }

    public void CheckOnAllUnitInFormation()
    {
        if (!this.isWaveSpawnComplete) return;

        foreach (var spawnedUnit in this._spawnedUnits)
        {
            if (spawnedUnit.transform.position != this._formationPoints[this._spawnedUnits.IndexOf(spawnedUnit)]) return;
        }
        this.isAllUnitInFormation = true;
    }

    // state change wave

    protected override void OnFormationCompleted()
    {
        base.OnFormationCompleted();
        
    }
}
