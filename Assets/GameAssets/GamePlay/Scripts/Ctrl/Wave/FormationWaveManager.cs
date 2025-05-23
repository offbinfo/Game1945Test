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
public class FormationWaveManager : GameMonoBehaviour
{

    public List<RoomWave> roomWaves;
    [SerializeField]
    private ExecutionMode executionMode;
    [SerializeField]
    private float delayStartWaveNext;

    [Button("AsyncFormationWave")]
    public void AsyncFormationWave()
    {
        roomWaves.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            roomWaves.Add(transform.GetChild(i).GetComponent<RoomWave>());
        }
    }

    public void StartRoomWave()
    {
        switch (executionMode)
        {
            case ExecutionMode.Sequential:
                StartCoroutine(DelayNextRoomWave());
                break;
            case ExecutionMode.Simultaneous:
                foreach (RoomWave roomWave in roomWaves)
                {
                    roomWave.StartWave();
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator DelayNextRoomWave()
    {
        for (int i = 0; i < roomWaves.Count; i++)
        {
            if (i > 0)
                yield return Yielders.Get(delayStartWaveNext);

            roomWaves[i].StartWave();
        }
    }
}
