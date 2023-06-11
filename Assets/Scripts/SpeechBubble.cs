using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public string[] Text;

    public GameObject Speaker;

    public Vector2 Offset;

    public bool Continued;

    public bool Waiting;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Waiting && !Continued && Input.GetButtonDown("Continue"))
        {
            Continued = true;
        }
        transform.position = Speaker.transform.position + Offset.Expand(0.0F);
    }

    public IEnumerator StartWriting()
    {
        var textBox = GetComponentInChildren<TextMeshProUGUI>();
        foreach (var text in Text)
        {
            yield return Extensions.WriteText(
                text,
                t => textBox.text = t,
                startDelay: 0,
                endDelay: 0,
                onComplete: OnComplete);
            while (!Continued)
            {
                yield return new WaitForSeconds(0.1F);
            }
            Continued = false;
        }
        Destroy(gameObject);
    }

    private void OnComplete()
    {
        // Unhide the continue button and signal that we're waiting for completion
        Waiting = true;
    }
}
