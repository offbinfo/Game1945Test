using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : GameMonoBehaviour
{

    [SerializeField] private List<WaveManager> waves;
    [SerializeField] private int currentWaveIndex = 0;

    private State currentState = State.NotStarted;

    public State CurrentState { get => currentState; }

    private static LevelManager instance;
    public static LevelManager Instance { get => instance; }

    public int CurrentWaveIndex { get => currentWaveIndex; }

    public int MaxWave { get => waves.Count; }

    protected override void Awake()
    {
        base.Awake();
        LevelManager.instance = this;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadWaves();
    }

    private void LoadWaves()
    {
        if (waves.Count > 0) return;
        foreach (Transform wave in transform)
        {
            wave.gameObject.SetActive(false);
            this.waves.Add(wave.transform.GetComponent<WaveManager>());
        }
        Debug.Log(transform.name + ": LoadWaves", gameObject);
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (currentState != State.Started) return;
        /*if (waves[currentWaveIndex].CurrentState != State.Completed) return;*/
        if (currentWaveIndex < waves.Count)
        {
            // Check if the current wave has been completed
            if (waves[currentWaveIndex].CurrentState == State.Completed)
            {
                waves[currentWaveIndex].gameObject.SetActive(false);
                currentWaveIndex++;
                if (currentWaveIndex < waves.Count)
                {
                    waves[currentWaveIndex].gameObject.SetActive(true);
                    this.SetUpAndShowWaveNotification();
                    waves[currentWaveIndex].StartWave();
                }
            }
        }
        // End the level if all waves have been completed
        else if (currentState == State.Started && currentWaveIndex == waves.Count)
        {
            EndLevel();
        }
    }


    public void StartLevel()
    {
        if (currentState == State.NotStarted)
        {
            if (waves.Count <= 0) return;
            waves[currentWaveIndex].gameObject.SetActive(true);
            this.SetUpAndShowWaveNotification();
            waves[currentWaveIndex].StartWave();
            currentState = State.Started;
        }
    }

    private void EndLevel()
    {
        currentState = State.Completed;
        Debug.Log("End level");
    }
    private void SetUpAndShowWaveNotification()
    {
        WaveNotification.Instance.SetMaxWave(waves.Count);
        WaveNotification.Instance.SetTimeShow(waves.Count);
        WaveNotification.Instance.SetCurrentWave(currentWaveIndex + 1);
        WaveNotification.Instance.ShowTextInTime();
    }

    /*    IEnumerator SpawnWave()
        {
            Wave currentWave = Waves[CurrentWaveIndex];

            yield return new WaitForSeconds(1.0f);


            while (GameObject.FindWithTag("Enemy") != null)
            {
                yield return null;
            }

            waveCompleted = true;
            CurrentWaveIndex++;
            if (CurrentWaveIndex >= Waves.Count)
            {
                CurrentWaveIndex = 0;
            }
        }*/
}
