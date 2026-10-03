using UnityEngine;

public class RandomCarColor : MonoBehaviour
{
    // Jeœli sprite jest na obiekcie podrzêdnym, przeci¹gnij go tutaj.
    // Puste = skrypt sam poszuka SpriteRenderera na tym obiekcie lub w dzieciach.
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Zakres losowego koloru (HSV)")]
    [SerializeField, Range(0f, 1f)] private float minSaturation = 0.6f;
    [SerializeField, Range(0f, 1f)] private float maxSaturation = 1f;
    [SerializeField, Range(0f, 1f)] private float minBrightness = 0.7f;
    [SerializeField, Range(0f, 1f)] private float maxBrightness = 1f;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogWarning("RandomCarColor: brak SpriteRenderera na " + name);
            return;
        }

        spriteRenderer.color = Random.ColorHSV(
            0f, 1f,
            minSaturation, maxSaturation,
            minBrightness, maxBrightness
        );
    }
}