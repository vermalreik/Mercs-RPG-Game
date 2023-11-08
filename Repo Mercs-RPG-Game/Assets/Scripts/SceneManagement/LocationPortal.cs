using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Teleports the player to a different position without switching scenes
public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    //private void OnTriggerEnter2D(Collider2D other){}
    // we can't use this apporach for a tile base game, the reason is because we don't want to trigger the code as soon as the player enters the trigger
    // we only want to do that once the player reaches the center of the tile that has the trigger
    
    public void onPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        this.player = player;
        //Debug.Log("Player entered the portal");
        StartCoroutine(Teleport());
    }

    public bool TriggerRepeatedly => false;

    Fader fader;
    private void Start() {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator Teleport()
    {
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        var destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);
    }

    public Transform SpawnPoint => spawnPoint;
}
