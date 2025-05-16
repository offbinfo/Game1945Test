using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDespawn : DespawnByDistance
{
/*    [SerializeField] protected List<IObjDespawnObsever> obsevers = new List<IObjDespawnObsever>();

    private void OnDespawnObject()
    {
        foreach (var obj in obsevers)
        {
            obj.OnDespawnObject();
        }
    }*/

    public override void DespawnObject()
    {
        //this.OnDespawnObject();
        EnemySpawner.Instance.Despawn(transform.parent);
    }
    
/*
    public virtual void ObseverAdd(IObjDespawnObsever obsever)
    {
        this.obsevers.Add(obsever);
    }*/
}
