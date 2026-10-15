using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Touch-friendly UI button component with pointer-down holding state
/// and tactile scaling animation.
/// </summary>
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public bool IsPressed { get; private set; } = false;

    private Vector3 originalScale;
    private const float PRESSED_SCALE = 0.92f;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
        transform.localScale = originalScale * PRESSED_SCALE;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
        transform.localScale = originalScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPressed = false;
        transform.localScale = originalScale;
    }

    private void OnDisable()
    {
        IsPressed = false;
        transform.localScale = originalScale;
    }
}
