using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public string Text;

    public GameObject Speaker;

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
        var position = Camera.main.WorldToScreenPoint(Speaker.transform.position);
        // var image = GetComponentInChildren<Image>();
        var tf = transform as RectTransform;
        tf.localPosition = position.With(z: tf.localPosition.z);
    }

    public IEnumerator StartWriting()
    {
        var textBox = GetComponentInChildren<TextMeshProUGUI>();
        yield return Extensions.WriteText(
            Text,
            t => textBox.text = t,
            startDelay: 0,
            endDelay: 0,
            onComplete: OnComplete);
        while (!Continued)
        {
            yield return new WaitForSeconds(0.1F);
        }
        Destroy(gameObject);
    }

    private void OnComplete()
    {
        // Unhide the continue button and signal that we're waiting for completion
        Waiting = true;
    }
}
