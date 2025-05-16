using PathCreation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


[ExecuteInEditMode]
public class FormationWaveManager : WaveManager
{
    [SerializeField] private FormationBase _formation;
    [SerializeField] private float _unitFormationSpeed = 2f;
    private List<Vector3> _formationPoints;

    private List<float> _unitOscillatesSpeeds = new List<float>();
    private float amplitudeOscillates = 0.03f;

    public bool isAllUnitInFormation = false;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadFormation();
    }

    protected override void Start()
    {
        base.Start();
        this.amountOfUnit = this._formation.GetPositions().Count;
    }

    protected override void Update()
    {
        base.Update();
        this.SetUpFormation();
        this.FormationOscillatesX();
        this.FormationUnitOscillatesY();
    }

    private void LoadFormation()
    {
        if (_formation != null) return;
        _formation = transform.GetComponentInChildren<FormationBase>();
        Debug.Log(transform.name + ": LoadFormation", gameObject);
    }


    protected override IEnumerator StartSpawn()
    {
        yield return new WaitForSeconds(2f);
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
        switch(typeSetUpWave)
        {
            case TypeSetUpWave.Loop:
                break;
            case TypeSetUpWave.ChangeWave:
            case TypeSetUpWave.ChangeWaveUsingPath:
                FormationWave();
                break;
        }
    }

    private void FormationWave()
    {
        if (this.isAllUnitInFormation) return;
        this.SetFormationPoints(this._formation.GetPositions().ToList());
        for (var i = 0; i < _spawnedUnits.Count; i++)
        {
            if (!isFollowPathDone[i]) continue;
            this._spawnedUnits[i].transform.position = Vector3.MoveTowards(this._spawnedUnits[i].transform.position, this._formationPoints[i], this._unitFormationSpeed * Time.deltaTime);
        }
        this.CheckOnAllUnitInFormation();
    }

    private void SetFormationPoints(List<Vector3> points)
    {
        this._formationPoints = points.ToList();

    }

    private bool movingToA = true;
    private void FormationOscillatesX()
    {
        if (!this.isAllUnitInFormation) return;
        bool isOscillatesX = true;
        if (isOscillatesX)
        {
            float maxX = this._formationPoints.Select(point => point.x).Max();
            float minX = this._formationPoints.Select(point => point.x).Min();
            float amplitudeMax = GameCtrl.Instance.M_maxX - maxX - 0.1f;
            float amplitudeMin = GameCtrl.Instance.M_minX - minX + 0.1f;

            for (var i = 0; i < _spawnedUnits.Count; i++)
            {
                if (!isFollowPathDone[i]) continue;
                Vector3 pointA = new Vector3(this._formationPoints[i].x + amplitudeMax, this._spawnedUnits[i].position.y, 0);
                Vector3 pointB = new Vector3(this._formationPoints[i].x + amplitudeMin, this._spawnedUnits[i].position.y, 0);
                if (movingToA)
                {
                    this._spawnedUnits[i].position = Vector3.MoveTowards(this._spawnedUnits[i].position, pointA, Time.deltaTime * 0.05f);

                    if (this._spawnedUnits[i].position == pointA)
                        movingToA = false;
                }
                else
                {
                    this._spawnedUnits[i].position = Vector3.MoveTowards(this._spawnedUnits[i].position, pointB, Time.deltaTime * 0.05f);

                    if (this._spawnedUnits[i].position == pointB)
                        movingToA = true;
                }
            }
        }
    }
    private float oscillatesYTimer = 0.0f;

    protected virtual void FormationUnitOscillatesY()
    {
        if (!this.isAllUnitInFormation) return;
        bool isOscillatesY = true;
        if (isOscillatesY)
        {
            for (var i = 0; i < _spawnedUnits.Count; i++)
            {
                if (!isFollowPathDone[i]) continue;
                // Move the object towards the target position
                float newY = _formationPoints[i].y + Mathf.PingPong(oscillatesYTimer * _unitOscillatesSpeeds[i], amplitudeOscillates * 2) - amplitudeOscillates;

                // move the object to its new position
                _spawnedUnits[i].position = new Vector3(_spawnedUnits[i].position.x, newY, 0);
            }
            this.oscillatesYTimer += Time.deltaTime;
        }
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

    protected override void ChangeWaveUsingPath()
    {
        this._formation = _roomWave[curIndexRoom].formation;
        this.isAllUnitInFormation = false;

        base.ChangeWaveUsingPath();


    }

    protected override void ChangeWave()
    {
        base.ChangeWave();

        StartCoroutine(DelayChangeWave());
    }

    private IEnumerator DelayChangeWave()
    {
        yield return new WaitForSeconds(2f);

        this._formation = _roomWave[curIndexRoom].formation;

        this.isAllUnitInFormation = false;
        FormationWave();

        yield return new WaitForSeconds(5f);

        this.curIndexRoom++;
        _paths = _roomWave[curIndexRoom].paths;

        this._formation = _roomWave[curIndexRoom].formation;

        this.isAllUnitInFormation = false;
        FormationWave();
    }
}
