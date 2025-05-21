using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDamageReceiver : DamageReceiver
{

    [SerializeField] protected ShipController shipController;
    public ShipController ShipController => shipController;

    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadShipController();
    }

    protected virtual void LoadShipController()
    {
        if (shipController != null) return;
        shipController = transform.parent.GetComponent<ShipController>();
        Debug.Log(transform.name + ": LoadShipController", gameObject);
    }

    protected override void SetupMaxHealth()
    {
        baseMaxHealthPoint = shipController.ShipProfile.maxHeath;
        base.SetupMaxHealth();
    }

    protected override void OnDead()
    {
        OnDeadFX();
        Destroy(transform.parent.gameObject);
    }

    protected virtual void OnDeadFX()
    {
        string fxOnDeadName = this.GetOnDeadFXName();
        Transform fxOnDead = FXSpawner.Instance.Spawn(fxOnDeadName, transform.position, transform.rotation);
        if (fxOnDead == null) return;
        fxOnDead.gameObject.SetActive(true);
    }

    protected virtual string GetOnDeadFXName()
    {
        return this.onDeadFXName;
    }
}
