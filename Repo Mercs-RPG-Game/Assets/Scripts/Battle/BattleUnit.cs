using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit{
        get{ return isPlayerUnit; }
    }

    public BattleHud Hud{
        get{ return hud; }
    }

    public Pokemon Pokemon { get;set; } // to store the pokemon we create in Setup()

    // For testing the battle system we will set these values from the Inspector
    // but later on we will start these dynamically when we start a battle

    Image image;
    Vector3 originalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition; //localPosition because we want the position relative to the canvas, no its actual position in the world
        originalColor = image.color;
    }

    // This function will create a Pokemon based from the base and level
    public void Setup(Pokemon pokemon)
    {
        Pokemon =  pokemon;
        if(isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        hud.SetData(pokemon);

        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if(isPlayerUnit)
            image.transform.localPosition = new Vector3(-600f, originalPos.y);
        else
             image.transform.localPosition = new Vector3(600f, originalPos.y);
    
        image.transform.DOLocalMoveX(originalPos.x, 1f); // second parameter is the duration
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if(isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
        // If we use Append() here too then we will only start playing the fade animation once the first one is complete
        // Join() will make that both of them play toguether
    }
}
