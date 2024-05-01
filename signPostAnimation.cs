using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GIFAnimator : MonoBehaviour
{
    public Sprite[] gifFrames; // Array of sprites representing GIF frames
    public float frameRate = 0.1f; // Frame rate for animation

    private Image imageComponent;
    private int currentFrameIndex = 0;

    void Start()
    {
        imageComponent = GetComponent<Image>();

        // Start the animation coroutine
        StartCoroutine(AnimateGIF());
    }

    IEnumerator AnimateGIF()
    {
        while (true)
        {
            // Change the sprite to the next frame
            imageComponent.sprite = gifFrames[currentFrameIndex];

            // Move to the next frame
            currentFrameIndex = (currentFrameIndex + 1) % gifFrames.Length;

            // Wait for the specified frame rate
            yield return new WaitForSeconds(frameRate);
        }
    }
}

