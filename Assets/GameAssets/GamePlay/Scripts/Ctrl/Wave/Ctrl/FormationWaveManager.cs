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


public class FormationWaveManager : GameMonoBehaviour
{

    public List<RoomShort> roomShorts;
    public List<RoomLong> roomLongs;

    [SerializeField]
    private ExecutionMode executionMode;
    [SerializeField]
    private float delayStartWaveNext;
    [SerializeField]
    private TypeWave typeWave;

    [Button("AsyncFormationWave")]
    public void AsyncFormationWave()
    {
        roomShorts.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            roomShorts.Add(transform.GetChild(i).GetComponent<RoomShort>());
        }
        roomLongs.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            roomLongs.Add(transform.GetChild(i).GetComponent<RoomLong>());
        }
    }

    public void StartRoomWave()
    {
        if(typeWave == TypeWave.Short)
        {
            switch (executionMode)
            {
                case ExecutionMode.Sequential:
                    StartCoroutine(DelayNextRoomShortWave());
                    break;
                case ExecutionMode.Simultaneous:
                    foreach (RoomShort roomWave in roomShorts)
                    {
                        roomWave.StartWave();
                    }
                    break;
                default:
                    break;
            }
        } else
        {
            switch (executionMode)
            {
                case ExecutionMode.Sequential:
                    StartCoroutine(DelayNextRoomLongWave());
                    break;
                case ExecutionMode.Simultaneous:
                    foreach (RoomLong roomWave in roomLongs)
                    {
                        roomWave.StartWave();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private IEnumerator DelayNextRoomShortWave()
    {
        for (int i = 0; i < roomShorts.Count; i++)
        {
            if (i > 0)
                yield return Yielders.Get(delayStartWaveNext);

            roomShorts[i].StartWave();
        }
    }

    private IEnumerator DelayNextRoomLongWave()
    {
        for (int i = 0; i < roomLongs.Count; i++)
        {
            if (i > 0)
                yield return Yielders.Get(delayStartWaveNext);

            roomLongs[i].StartWave();
        }
    }
}
