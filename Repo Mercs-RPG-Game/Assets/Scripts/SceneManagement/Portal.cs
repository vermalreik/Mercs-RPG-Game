using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1; // default = -1, just as so that we'll get an error if we forget to set this from Unity
    [SerializeField] Transform spawnPoint;

    PlayerController player;

    //private void OnTriggerEnter2D(Collider2D other){}
    // we can't use this apporach for a tile base game, the reason is because we don't want to trigger the code as soon as the player enters the trigger
    // we only want to do that once the player reaches the center of the tile that has the trigger
    
    public void onPlayerTriggered(PlayerController player)
    {
        this.player = player;
        //Debug.Log("Player entered the portal");
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);
        //Debug.Log("Logging form portal after scene switch");

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}