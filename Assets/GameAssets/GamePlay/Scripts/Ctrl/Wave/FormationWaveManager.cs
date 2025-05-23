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

    [Button("AsyncFormationWave")]    
    public void AsyncFormationWave()
    {
        roomWaves.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            roomWaves.Add(transform.GetChild(i).GetComponent<RoomWave>());
        }
    }
    
}
