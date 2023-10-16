using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed; //5
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered; // It's an event :D following the Observer Pattern we will notify whenever th aplayer starts a battle, so the controller passes to the Battle System

    private bool isMoving;
    private Vector2 input;

    private CharacterAnimator animator;

    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); // Raw para k el input sea siempre 1 o -1
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.MoveX = input.x;
                animator.MoveY = input.y;

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(new Vector3(targetPos.x, targetPos.y - 0.5f)))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.IsMoving = isMoving;

        if(Input.GetKeyDown(KeyCode.Z) | Input.GetKeyDown(KeyCode.E))
            Interact();
    }

    void Interact()
    {
        // first I find the direction in which the player is facing
        var facingDir = new Vector3(animator.MoveX, animator.MoveY);

        // find position of the tile next to the player
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
            // ? Null coalescing operator
        }
    }

    IEnumerator Move(Vector3 targetPos) // IEnumerator is used to do something over a period of time
    {
        isMoving = true;

        // check if the current position is greater than a very small value Mathf.Epsilon
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            // if it is, then we will use the MoveTowards function to move the player towards the target position by a very small amount
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null; // this stops the execution of the coroutine and resume it in the next Update function
        }
        transform.position = targetPos; // set the player current position to the target position
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10) // owo Escribo UnityEngine porque al importar using. System para usar eventos "Random" queda ambiguo por estar en las dos librerias
            {
                animator.IsMoving = false;
                //Debug.Log("Encountered a wild pokemon");
                OnEncountered();
            }
        }
    }
}
