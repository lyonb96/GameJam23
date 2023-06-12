using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3 : MonoBehaviour
{
    private LevelScript levelScript;

    public GameObject SpeechBubblePrefab;

    public GameObject OverlayPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("WorldCanvas");
        var player = FindObjectOfType<Player>();
        var boss = FindObjectOfType<Boss>();
        var heart = FindObjectOfType<HeartFollower>();
        var camera = FindObjectOfType<CameraFollower>();
        var anchor = GameObject.Find("BossFightAnchor");
        levelScript = new LevelScript(Instantiate, Destroy, canvas)
            .Do(() => player.PauseMovement())
            .FadeFromBlack(OverlayPrefab, 1.0F)
            .Do(() => player.ResumeMovement())
            .WaitForTrigger("EnterBossFight")
            .Do(() => player.PauseMovement())
            .Do(() => camera.OverrideFocus(boss.gameObject))
            .ShowSpeechBox(SpeechBubblePrefab, boss.gameObject, Vector2.up, Color.white, "So... another worthless traveler", "Do you know how many of you I've slain?")
            .Do(() => camera.OverrideFocus(null))
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, Vector2.up, Color.white, "Slaydeez nuts, bitch")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, Vector2.up, Extensions.HeartTextColor, "Gottem")
            .Do(() => player.ResumeMovement())
            .Do(() => boss.Pause = false)
            .Do(() => camera.OverrideFocus(anchor));
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

    public void Checkpoint(string name)
    {
        levelScript.Trigger(name);
    }
}
