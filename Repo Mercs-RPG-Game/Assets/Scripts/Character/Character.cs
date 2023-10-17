using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed; //5

    public bool IsMoving { get; set; }
    CharacterAnimator animator;

    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
    }
    public IEnumerator Move(Vector2 moveVec, Action OnMoveOver=null) // IEnumerator is used to do something over a period of time
    {
        // set parameters of the animator
        animator.MoveX = Mathf.Clamp(moveVec.x, -1f, 1f);
        animator.MoveY = Mathf.Clamp(moveVec.y, -1f, 1f);;

        // calculate the target position
        var targetPos = transform.position;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;

        if(!IsPathClear(targetPos))
            yield break;

        // Move the character to the target position
        IsMoving = true;

        // check if the current position is greater than a very small value Mathf.Epsilon
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // if it is, then we will use the MoveTowards function to move the player towards the target position by a very small amount
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null; // this stops the execution of the coroutine and resume it in the next Update function
        }
        transform.position = targetPos; // set the player current position to the target position
        IsMoving = false;

        OnMoveOver?.Invoke(); // ? = int case it's null we don't call it
    }

    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }

    private bool IsPathClear(Vector3 targetPos)
    {
        var diff = targetPos - transform.position;
        var dir = diff.normalized;

        if( Physics2D.BoxCast(transform.position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer) == true)
            return false;

        return true;
    }       

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        var xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x); // difference in the x coordinate // "Math.Floor" to get the diff in number of tiles
        // we get the difference as an integer instead of a decimal value
        var ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if(xdiff == 0 || ydiff == 0) // si diff de alguno d elos ejes es 0 entonces no esta en diagonal y el NPC podra firar up/down/right/left para verlo
        {
            animator.MoveX = Mathf.Clamp(xdiff, -1f, 1f);
            animator.MoveY = Mathf.Clamp(ydiff, -1f, 1f);;
        }
        else
            Debug.LogError("Error in Look Towards: You can't ask the character to look diagonal");
    }

    public CharacterAnimator Animator{
        get => animator;
    }
}
