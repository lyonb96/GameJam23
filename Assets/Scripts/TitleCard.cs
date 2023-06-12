using System.Collections;
using TMPro;
using UnityEngine;

public class TitleCard : MonoBehaviour
{
    public string LevelName;

    public void Display()
    {
        var textComp = GetComponent<TextMeshProUGUI>();
        textComp.text = LevelName;
        StartCoroutine(DoTitleCardSequence());
    }

    private IEnumerator DoTitleCardSequence()
    {
        yield return Extensions.Fade(0.5F, 0.0F, 1.0F, Fade);
        yield return new WaitForSeconds(5.0F);
        yield return Extensions.Fade(0.5F, 1.0F, 0.0F, Fade);
    }

    private void Fade(float amount)
    {
        var textComp = GetComponent<TextMeshProUGUI>();
        textComp.color = textComp.color.WithAlpha(amount);
    }
}
