using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjFollowMouse : ObjMovement
{

    private float deltaX, deltaY;


    protected override void GetTargetPosition()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        //code here

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    this.isMoving = true;
                    deltaX = touchPos.x - transform.position.x;
                    deltaY = touchPos.y - transform.position.y;
                    break;
                case TouchPhase.Moved:
                    targetPosition = new Vector3(touchPos.x - deltaX, touchPos.y - deltaY);
                    break;
                case TouchPhase.Ended:
                    this.isMoving = false;
                    break;
            }

        }
        return;
#endif
        this.isMoving = true;
        this.targetPosition = InputManager.Instance.MouseWorldPos;
        this.targetPosition.z = 0;
    }

    /*    protected virtual void CheckMoving()
   {
       this.isMoving = InputManager.Instance.OnMoving;
   }*/
}


