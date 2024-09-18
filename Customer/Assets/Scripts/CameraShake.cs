using UnityEngine;
using DG.Tweening;

public class Camerashake : MonoBehaviour
{
    public static Camerashake instance;
    private Vector3 originalPosition;

    private void Awake()
    {
        instance = this;
        originalPosition = transform.localPosition; // Store the original position of the camera
    }

    private void OnShake(float duration, float strength)
    {
        // Start shaking the position and rotation, and return to the original position when done
        transform.DOShakePosition(duration, strength)
            .OnComplete(() => transform.localPosition = originalPosition); // Reset to original position

        // Optionally, shake rotation (if necessary)
        transform.DOShakeRotation(duration, strength);
    }

    public static void Shake(float duration, float strength)
    {
        instance.OnShake(duration, strength);
    }
}
