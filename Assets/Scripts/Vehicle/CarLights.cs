using UnityEngine;

/// <summary>
/// Controls vehicle lighting: Headlights (for night parking),
/// Brake lights (glow brightly on brake/reverse), and reverse lights.
/// </summary>
public class CarLights : MonoBehaviour
{
    [Header("Headlights")]
    public Light leftHeadlight;
    public Light rightHeadlight;
    public Renderer leftHeadlightRenderer;
    public Renderer rightHeadlightRenderer;

    [Header("Taillights / Brake Lights")]
    public Renderer leftTaillightRenderer;
    public Renderer rightTaillightRenderer;
    public Light rearGlowLight;

    [Header("Materials / Colors")]
    public Material headlightOnMat;
    public Material taillightOnMat;
    public Material taillightOffMat;

    private bool headlightsActive = false;

    private void Awake()
    {
        // Setup default materials if unassigned
        if (headlightOnMat == null) headlightOnMat = Resources.Load<Material>("Materials/Car_Headlight_On");
        if (taillightOnMat == null) taillightOnMat = Resources.Load<Material>("Materials/Car_Taillight_On");
        if (taillightOffMat == null) taillightOffMat = Resources.Load<Material>("Materials/Car_Taillight_Off");
    }

    public void SetHeadlights(bool on)
    {
        headlightsActive = on;
        if (leftHeadlight != null) leftHeadlight.enabled = on;
        if (rightHeadlight != null) rightHeadlight.enabled = on;

        if (headlightOnMat != null)
        {
            if (leftHeadlightRenderer != null) leftHeadlightRenderer.material = headlightOnMat;
            if (rightHeadlightRenderer != null) rightHeadlightRenderer.material = headlightOnMat;
        }
    }

    public void SetBrakeLights(bool isBraking)
    {
        Material targetMat = isBraking ? taillightOnMat : taillightOffMat;
        if (targetMat != null)
        {
            if (leftTaillightRenderer != null) leftTaillightRenderer.material = targetMat;
            if (rightTaillightRenderer != null) rightTaillightRenderer.material = targetMat;
        }

        if (rearGlowLight != null)
        {
            rearGlowLight.intensity = isBraking ? 1.5f : 0.2f;
        }
    }
}
