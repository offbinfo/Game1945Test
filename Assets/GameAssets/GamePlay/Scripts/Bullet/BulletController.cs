using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : GameMonoBehaviour
{
    [SerializeField] protected BulletDespawn bulletDespawn;
    public BulletDespawn BulletDespawn => bulletDespawn;

    [SerializeField] protected BulletDamageSender bulletDamageSender;
    public BulletDamageSender BulletDamageSender => bulletDamageSender;

    [SerializeField] protected Transform shooter;
    public Transform Shooter => shooter;

    public bool isSendDamage;

    protected override void OnEnable()
    {
        base.OnEnable();
        isSendDamage = true;
    }

    private void Update()
    {
        this.ShooterCheck();
    }

    private void ShooterCheck()
    {
        if (this.shooter == null || !this.shooter.gameObject.activeSelf)
        {
            BulletSpawner.Instance.Despawn(transform);
        }
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadBulletDespawn();
        this.LoadBulletDamageSender();
    }

    protected virtual void LoadBulletDamageSender()
    {
        if (this.bulletDamageSender != null) return;
        this.bulletDamageSender = transform.GetComponentInChildren<BulletDamageSender>();
        Debug.Log(transform.name + ": LoadBulletDamageSender", gameObject);
    }

    protected virtual void LoadBulletDespawn()
    {
        if (this.bulletDespawn != null) return;
        this.bulletDespawn = transform.GetComponentInChildren<BulletDespawn>();
        //Debug.Log(transform.name + ": LoadBulletDespawn", gameObject);
    }

    public virtual void SetShooter(Transform shooter)
    {
        this.shooter = shooter;
    }
}
