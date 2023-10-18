using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
     [SerializeField] string name;
     [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    Character character;

    private void Awake() {
        character = GetComponent<Character>();
    }

    private void Start() {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized; // substract one tile, so the trainer don't walk onto the tile on which the player is standing
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y)); // now it will be an integer

        yield return character.Move(moveVec);

        // Show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            //Debug.Log("Starting Trainer Battle");
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;

        if(dir == FacingDirection.Right)
            angle = 90f;
        else if(dir == FacingDirection.Up)
            angle = 180f;
        else if(dir == FacingDirection.Left)
            angle = 270f;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
        // ".rotation" is a quaternion
        // If you want to set the rotation as a vector then you can use the ".eulerAngles" angle property
    }

    public string Name{
        get => name;
    }

    public Sprite Sprite{
        get => sprite;
    }
}
