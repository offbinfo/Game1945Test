using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomWave : MonoBehaviour
{
    public List<PathCreator> paths = new();
    public FormationBase formation;
    [SerializeField]
    private Transform parentPath;

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            UpdatePaths();
        }
    }

    private void UpdatePaths()
    {
        if (parentPath == null) return;

        paths.Clear();
        foreach (Transform child in parentPath)
        {
            PathCreator path = child.GetComponent<PathCreator>();
            if (path != null)
            {
                paths.Add(path);
            }
        }
    }
#endif
}
