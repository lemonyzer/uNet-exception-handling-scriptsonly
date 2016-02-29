using UnityEngine;
using System.Collections;
using UnityEditor.Animations;

[System.Serializable]
public class SMWAnimation
{
    public string name;
    public int framesPerSecond;  // sample			// 2013.12.16 Live Training 16 Dec 2013 - 2D Character Controllers (720p).mp4  @@ 26:14
    public int keyFrames;
    public int frameDistance;
    public Sprite[] sprites;
    public WrapMode wrapMode;
    public AnimatorState animState;

    public SMWAnimation(string name, int framesPerSecond, int keyFrames, Sprite[] sprites, WrapMode wrapMode, AnimatorState animState)
    {
        this.name = name;
        this.wrapMode = wrapMode;
        this.framesPerSecond = framesPerSecond;
        this.keyFrames = keyFrames;
        this.animState = animState;
        this.sprites = sprites;
    }
}
