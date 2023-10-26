using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    private Vector2 input;

   private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); // Raw para k el input sea siempre 1 o -1
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if(Input.GetKeyDown(KeyCode.Z) | Input.GetKeyDown(KeyCode.E))
            Interact();
    }

    void Interact()
    {
        // first I find the direction in which the player is facing
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);

        // find position of the tile next to the player
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
            // ? Null coalescing operator
        }
    }

    private void OnMoveOver()
    {
       var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        // OverlapCircle will only return the first Game Object with which it overlapped
        // OverlapCircleAll = returns ALL Game Objects with which it overlapped
    
        foreach(var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                triggerable.onPlayerTriggered(this);
                break;
            }
        }
    }

    public string Name{
        get => name;
    }

    public Sprite Sprite{
        get => sprite;
    }

    public Character Character => character;
}
