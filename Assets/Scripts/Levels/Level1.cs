using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Level1 : MonoBehaviour
{
    private TextMeshProUGUI OpeningDialogText;
    private Image Overlay;

    private LevelScript levelScript;

    public GameObject SpeechBubblePrefab;

    // Start is called before the first frame update
    void Start()
    {
        OpeningDialogText = GameObject.Find("OpeningDialogTextBox").GetComponent<TextMeshProUGUI>();
        Overlay = GameObject.Find("OpeningDialogOverlay").GetComponent<Image>();
        var player = FindObjectOfType<Player>();
        var heart = FindObjectOfType<HeartFollower>();
        var canvas = FindObjectOfType<Canvas>().gameObject;
        levelScript = new LevelScript(Instantiate, Destroy, canvas)
            .Do(() => player.PauseMovement())
            .WriteText("Where is it?", OpeningDialogText)
            .WaitForContinue()
            .Do(() => OpeningDialogText.text = "")
            .WriteText("Any other script here...", OpeningDialogText)
            .WaitForContinue()
            .DoCoroutine(Extensions.Fade(2.0F, 1.0F, 0.0F, opacity => OpeningDialogText.color = OpeningDialogText.color.WithAlpha(opacity)))
            .DoCoroutine(Extensions.Fade(0.5F, 1.0F, 0.0F, opacity => Overlay.color = Overlay.color.WithAlpha(opacity)))
            .Do(() =>
            {
                Destroy(OpeningDialogText);
                Destroy(Overlay);
            })
            .WaitForSeconds(1.0F)
            .Do(() => player.ResumeMovement())
            .WaitForSeconds(1.0F)
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, "Hello there...")
            .WaitForTrigger("Tutorial1")
            .Do(() => Debug.Log("Hit the first trigger"));
        StartCoroutine(levelScript.Execute());
    }

    // Update is called once per frame
    void Update()
    {
        if (levelScript.WaitingForContinue && Input.GetButtonDown("Continue"))
        {
            levelScript.Continue();
        }
    }
}
