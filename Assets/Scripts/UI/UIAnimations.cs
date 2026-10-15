using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Helper animations for modern automotive UI: button bouncers, star reveals, and banner pops.
/// </summary>
public static class UIAnimations
{
    public static IEnumerator PopIn(Transform target, float duration = 0.3f)
    {
        if (target == null) yield break;
        target.localScale = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Overshoot elastic curve
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            if (t > 0.7f) scale = 1f + Mathf.Sin((t - 0.7f) * Mathf.PI * 3.33f) * 0.1f;
            target.localScale = Vector3.one * scale;
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    public static IEnumerator RevealStars(GameObject[] stars, int count, float delayBetween = 0.35f)
    {
        if (stars == null) yield break;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null) stars[i].SetActive(false);
        }

        for (int i = 0; i < count && i < stars.Length; i++)
        {
            yield return new WaitForSecondsRealtime(delayBetween);
            if (stars[i] != null)
            {
                stars[i].SetActive(true);
                AudioManager.Instance?.PlayCountdownTick();
                yield return PopIn(stars[i].transform, 0.25f);
            }
        }
    }
}
