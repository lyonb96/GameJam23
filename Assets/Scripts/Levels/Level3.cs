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
        var deeznuts = Random.Range(0.0F, 1.0F);
        levelScript = new LevelScript(Instantiate, Destroy, canvas)
            .Do(() => player.PauseMovement())
            .FadeFromBlack(OverlayPrefab, 1.0F)
            .Do(() => player.ResumeMovement())
            .WaitForTrigger("EnterBossFight")
            .Do(() => player.PauseMovement())
            .Do(() => camera.OverrideFocus(boss.gameObject))
            .ShowSpeechBox(SpeechBubblePrefab, boss.gameObject, Vector2.up, Color.white, "So... another worthless traveler", "Do you know how many of you I've slain?")
            .Do(() => camera.OverrideFocus(null))
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, Vector2.up, Color.white, deeznuts > 0.9F ? "Slaydeez nuts, bitch" : "I'm taking you down and going home.")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, Vector2.up, Extensions.HeartTextColor, deeznuts > 0.9F ? "Gottem" : "Be careful")
            .Do(() => player.ResumeMovement())
            .Do(() => boss.Pause = false)
            .Do(() => camera.OverrideFocus(anchor))
            .WaitForTrigger("BossDead")
            .Do(() => camera.OverrideFocus(boss.gameObject))
            .Do(() => player.PauseMovement())
            .ShowSpeechBox(SpeechBubblePrefab, boss.gameObject, Vector2.up, Color.white, "I never thought this day would come...", "Enjoy your life...")
            .Do(() => boss.Die())
            .WaitForSeconds(1.0F)
            .Do(() => camera.OverrideFocus(null))
            // Dialog between player and heart
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, Vector2.up, Color.white, "I may have lost my heart, but I think I found a new one.")
            .FadeToBlack(OverlayPrefab, 3.0F);
        StartCoroutine(levelScript.Execute());
    }

    // Update is called once per frame
    void Update()
    {
        if (levelScript.WaitingForContinue && Input.GetButtonDown("Attack"))
        {
            levelScript.Continue();
        }
    }

    public void Checkpoint(string name)
    {
        levelScript.Trigger(name);
    }
}
