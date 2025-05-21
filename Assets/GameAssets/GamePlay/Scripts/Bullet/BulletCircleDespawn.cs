using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCircleDespawn : DespawnByTime
{
    protected override void OnEnable()
    {
        base.OnEnable();
        this.delay = 5f;
    }

    public override void DespawnObject()
    {
        BulletSpawner.Instance.Despawn(transform.parent);
        var listLine = GameObject.FindGameObjectsWithTag("CircleLine");
        foreach (var item in listLine)
        {
            Destroy(item.gameObject);
        }
    }
}
