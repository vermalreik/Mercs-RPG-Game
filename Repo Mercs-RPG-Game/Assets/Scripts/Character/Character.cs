using System;
using System.Collections;
using System.Collections.Generic;
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

        if(!IsWalkable(targetPos))
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

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        }
        return true;
    }

    public CharacterAnimator Animator{
        get => animator;
    }
}
