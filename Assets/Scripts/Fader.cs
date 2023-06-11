using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public IEnumerator Fade(float from, float to, float duration, bool destroy = false)
    {
        var image = GetComponent<Image>();
        yield return Extensions.Fade(duration, from, to, a => image.color = image.color.WithAlpha(a));
        if (destroy)
        {
            Destroy(gameObject);
        }
    }
}
