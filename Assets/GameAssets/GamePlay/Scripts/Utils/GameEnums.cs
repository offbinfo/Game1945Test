using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeSetUpWave
{
    None,
    Loop,
    ChangeWaveUsingPath,
    ChangeWave,
    PathToPath,
    SetUpWave,
    SetUpPath
}

public enum ExecutionMode
{
    Simultaneous,   // Đồng thời
    Sequential,    // Tuần tự
}

public enum TypeWave
{
    Short,
    Long,
}
