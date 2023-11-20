using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// :D This class will work as the Unity Animator

public class CharacterAnimator : MonoBehaviour
{
    // Walk Sprites
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // Surf Sprites
    [SerializeField] List<Sprite> surfDownSprites;
    [SerializeField] List<Sprite> surfUpSprites;
    [SerializeField] List<Sprite> surfLeftSprites;
    [SerializeField] List<Sprite> surfRightSprites;

    // Parameters
    public float MoveX{ get; set; }
    public float MoveY{ get; set; }
    public bool IsMoving{ get; set; }
    public bool IsJumping{ get; set; }
    public bool IsSurfing{ get; set; }

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator surfDownAnim;
    SpriteAnimator surfUpAnim;
    SpriteAnimator surfRightAnim;
    SpriteAnimator surfLeftAnim;

    SpriteAnimator currentAnim;
    bool wasPreviouslyMoving;

    //References
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);

        surfDownAnim = new SpriteAnimator(surfDownSprites, spriteRenderer);
        surfUpAnim = new SpriteAnimator(surfUpSprites, spriteRenderer);
        surfRightAnim = new SpriteAnimator(surfRightSprites, spriteRenderer);
        surfLeftAnim = new SpriteAnimator(surfLeftSprites, spriteRenderer);

        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim;
    }

    private void Update()
    {
        var prevAnim = currentAnim;

        if(!IsSurfing)
        {
            if(MoveX == 1)
            currentAnim = walkRightAnim;
            else if(MoveX == -1)
                currentAnim = walkLeftAnim;
            if(MoveY == 1)
                currentAnim = walkUpAnim;
            else if(MoveY == -1)
                currentAnim = walkDownAnim;

            if(currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start(); // We have also call it if the value of IsMoving changue xD or else sometimes the character slides, moves without playing animation

            if(IsJumping)
                spriteRenderer.sprite = currentAnim.Frames[currentAnim.Frames.Count-1];
            else if(IsMoving)
                currentAnim.HandleUpdate();
            else
                spriteRenderer.sprite = currentAnim.Frames[0];
        }
        else
        {
            if(MoveX == 1)
                currentAnim = surfRightAnim; // spriteRenderer.sprite = surfSprite[2];
            else if(MoveX == -1)
                currentAnim = surfLeftAnim;
            if(MoveY == 1)
                currentAnim = surfUpAnim;
            else if(MoveY == -1)
                currentAnim = surfDownAnim;

            if(currentAnim != prevAnim || IsMoving != wasPreviouslyMoving)
                currentAnim.Start();

            if(IsMoving)
                currentAnim.HandleUpdate();
            else
                spriteRenderer.sprite = currentAnim.Frames[0];
        }

        

        wasPreviouslyMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if(dir == FacingDirection.Right)
            MoveX = 1;
        else if(dir == FacingDirection.Left)
            MoveX = -1;
        else if(dir == FacingDirection.Down)
            MoveY = -1;
        else if(dir == FacingDirection.Up)
            MoveY = 1;
        
    }

    public FacingDirection DefaultDirection{
        get => defaultDirection;
    }
}

public enum FacingDirection{ Up, Down, Left, Right}
