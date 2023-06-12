using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Flyout : MonoBehaviour
{
    public Sprite Miss, Hit, Slice, Slash;

    public int FlyoutToShow;

    private Vector3 StartPos;

    private Vector3 EndPos;

    public void Display()
    {
        var renderer = GetComponent<Image>();
        StartPos = transform.position;
        EndPos = transform.position + Vector3.up;
        renderer.sprite = FlyoutToShow switch
        {
            0 => Miss,
            1 => Hit,
            2 => Slice,
            3 => Slash,
            _ => Miss,
        };
        StartCoroutine(DisplayFlyout());
    }

    private IEnumerator DisplayFlyout()
    {
        yield return Extensions.Fade(1.0F, 1.0F, 0.0F, SetOpacity);
        Destroy(gameObject);
    }

    private void SetOpacity(float a)
    {
        var renderer = GetComponent<Image>();
        renderer.color = renderer.color.WithAlpha(a);
        renderer.transform.position = Vector3.Lerp(EndPos, StartPos, a);
    }
}
