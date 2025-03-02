using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;  // Assign a UI Text in the Inspector
    private int frameCount = 0;
    private float elapsedTime = 0f;
    private float fps = 0f;

    void Update()
    {
        frameCount++;
        elapsedTime += Time.unscaledDeltaTime;

        if (elapsedTime >= 1f)  // Every second
        {
            fps = frameCount / elapsedTime;
            fpsText.text = "FPS: " + Mathf.RoundToInt(fps);

            // Reset counters
            frameCount = 0;
            elapsedTime = 0f;
        }
    }
}
