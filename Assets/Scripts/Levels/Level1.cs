using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Level1 : MonoBehaviour
{
    private TextMeshProUGUI OpeningDialogText;
    private Image Overlay;

    // Start is called before the first frame update
    void Start()
    {
        OpeningDialogText = GameObject.Find("OpeningDialogTextBox").GetComponent<TextMeshProUGUI>();
        Overlay = GameObject.Find("OpeningDialogOverlay").GetComponent<Image>();
        StartCoroutine(
            Extensions.WriteText(
                "Where is it? ",
                SetDialog,
                onComplete: OnFinish));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDialog(string dialog)
    {
        OpeningDialogText.text = dialog;
    }

    void OnFinish()
    {
        Destroy(Overlay);
    }
}
