using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Level1 : MonoBehaviour
{
    private TextMeshProUGUI OpeningDialogText;
    private Image Overlay;

    private LevelScript levelScript;

    public GameObject SpeechBubblePrefab;

    public GameObject OverlayPrefab;

    // Start is called before the first frame update
    void Start()
    {
        OpeningDialogText = GameObject.Find("OpeningDialogTextBox").GetComponent<TextMeshProUGUI>();
        Overlay = GameObject.Find("OpeningDialogOverlay").GetComponent<Image>();
        var player = FindObjectOfType<Player>();
        var heart = FindObjectOfType<HeartFollower>();
        var canvas = FindObjectOfType<Canvas>().gameObject;
        var doorSlot = GameObject.Find("DoorSlot");
        var music = FindObjectOfType<AudioSource>();
        var startVolume = music.volume;
        // var door = FindObjectOfType<Door>();
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
            .Do(() => RhythmManager.GetInstance().PlayOnBeat(music))
            .Do(() => player.ResumeMovement())
            .DoCoroutine(Extensions.Fade(1.0F, 0.0F, 1.0F, volScale => music.volume = startVolume * volScale))
            .WaitForTrigger("Tutorial1")
            .Do(() => Debug.Log("Hit the first trigger"))
            .WaitForTrigger("Tutorial2")
            .Do(() => Debug.Log("Hit the second one!"))
            .WaitForTrigger("MeetHeart")
            .Do(() => player.PauseMovement())
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "I've been waiting for someone to show up...")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1), Color.white, "What is this place?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "Many people call it many things", "But ultimately it's a place for lost hearts")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1), Color.white, "And lost people, it would seem...")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "Sometimes!", "But it's very rare, especially since...", "...well anyway, I can help guide you!")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1), Color.white, "Guide me?", "I suppose any help is better than none.")
            .Do(() => heart.Follow = true)
            .Do(() => player.ResumeMovement())
            .WaitForTrigger("HeartExplainDoor")
            .Do(() => player.PauseMovement())
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor,
                "This is a [Insert Cool Gate Name Here]",
                "These gates enable us to traverse the levels of this plane",
                "This one seems dormant...")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1), Color.white, "How do we pass through it?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "I think I know how to power it up", "Stand back... just in case.")
            .Do(() => heart.OverrideFocus(doorSlot))
            .WaitForSeconds(1.0F)
            // .Do(() => door.Open())
            .Do(() => player.ResumeMovement())
            .WaitForTrigger("AtDoor")
            .Do(() => player.PauseMovement())
            .FadeToBlack(OverlayPrefab)
            .DoCoroutine(Extensions.Fade(1.0F, 1.0F, 0.0F, volScale => music.volume = startVolume * volScale))
            .LoadLevel("Level2")
            ;
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
