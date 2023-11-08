using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1; // default = -1, just as so that we'll get an error if we forget to set this from Unity
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
        StartCoroutine(SwitchScene());
    }

    public bool TriggerRepeatedly => false;

    Fader fader;
    private void Start() {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        //Debug.Log("Logging form portal after scene switch");

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return fader.FadeOut(0.5f);
        GameController.Instance.PauseGame(false);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier {A, B, C, D, E}