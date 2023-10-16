using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate=0.16f) // 60 frames per second
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 1;
        timer = 0;
        spriteRenderer.sprite = frames[1];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime; // every frame we will increment the timer
        if(timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            // if the current frame is the last frame it will go back to the first frame of the animation
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate; // reset timer
        }
    }

    public List<Sprite> Frames{
        get{ return frames; }
    }
}
