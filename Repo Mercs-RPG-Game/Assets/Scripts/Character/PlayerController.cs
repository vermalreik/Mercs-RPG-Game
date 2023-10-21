using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

     const float offsetY = 0.3f;

    public event Action OnEncountered; // It's an event :D following the Observer Pattern we will notify whenever th aplayer starts a battle, so the controller passes to the Battle System
    public event Action<Collider2D> OnEnterTrainersView;

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
        CheckForEncounters();
        CheckIfInTrainerView();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, GameLayers.i.GrassLayer) != null) 
        {// "- new Vector3(0, offsetY)" xD so battles don't start when the player is one tile below the grass. Now we'll create the overlap circle at the center of the tile, which is 0.5 :D because the player Tranform Position Y is in 0.8 (offsetY = 0.3)
            if (UnityEngine.Random.Range(1, 101) <= 10) // owo Escribo UnityEngine porque al importar using. System para usar eventos "Random" queda ambiguo por estar en las dos librerias
            {
                character.Animator.IsMoving = false;
                //Debug.Log("Encountered a wild pokemon");
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainerView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            //Debug.Log("In Trainer's view");
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name{
        get => name;
    }

    public Sprite Sprite{
        get => sprite;
    }
}
