using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour
{
    private TextMeshProUGUI TooltipText;
    private Image TooltipBase;
    private float BaseOpacity;

    // Start is called before the first frame update
    void Start()
    {
        TooltipText = GetComponentInChildren<TextMeshProUGUI>();
        TooltipBase = GetComponent<Image>();
        BaseOpacity = TooltipBase.color.a;
        TooltipBase.color = TooltipBase.color.WithAlpha(0.0F);
        TooltipText.color = TooltipText.color.WithAlpha(0.0F);
    }

    public void Show(string text)
    {
        TooltipText.text = text;
        StartCoroutine(ShowTip());
    }

    IEnumerator ShowTip()
    {
        yield return Extensions.Fade(0.5F, 0.0F, 1.0F, SetOpacity);
        yield return new WaitForSeconds(3.0F);
        yield return Extensions.Fade(0.5F, 1.0F, 0.0F, SetOpacity);
    }

    void SetOpacity(float a)
    {
        TooltipText.color = TooltipText.color.WithAlpha(a);
        TooltipBase.color = TooltipBase.color.WithAlpha(a * BaseOpacity);
    }
}
