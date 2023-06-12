using UnityEngine;

public class Level2 : MonoBehaviour
{
    public GameObject SpeechBubblePrefab;

    public GameObject OverlayPrefab;

    private LevelScript levelScript;

    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("WorldCanvas");
        var player = FindObjectOfType<Player>();
        var heart = FindObjectOfType<HeartFollower>();
        var firstSlime = GameObject.Find("/FirstSlime");
        var firstSlimeScript = firstSlime.GetComponent<GroundEnemy>();
        firstSlimeScript.enabled = false;
        var doorSlot = GameObject.Find("DoorSlot");
        // var door = FindObjectOfType<Door>();
        var music = FindObjectOfType<AudioSource>();
        var startVolume = music.volume;
        levelScript = new LevelScript(Instantiate, Destroy, canvas)
            .Do(() => player.PauseMovement())
            .FadeFromBlack(OverlayPrefab, 1.0F)
            .WaitForSeconds(0.5F)
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "Woah... What is this place?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "Welcome to the Lost Souls Forest")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "Lost souls?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "Yep. This is the realm that the Keeper of Souls dreamt to keep those who lost their hearts from ever finding them again.")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "Why would anyone do that?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "He himself lost his heart and never found it. I guess he wants to make sure no one else can either...")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "I'm going to! I'm going to find my heart, even if it kills me.")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "Ironic...")
            .Do(() => player.ResumeMovement())
            .WaitForTrigger("SeeFirstEnemy")
            .Do(() => player.PauseMovement())
            .ShowSpeechBox(
                SpeechBubblePrefab,
                heart.gameObject,
                new Vector2(0, 0.5F),
                Extensions.HeartTextColor,
                "That slime is a creature the Keeper of Souls brought into this realm to stop travelers",
                "Luckily, that sword of yours should serve you well.",
                "Try timing your attacks to the beat!")
            .Do(() => player.ResumeMovement())
            .Do(() => firstSlimeScript.enabled = true)
            .WaitForTrigger("FinalDoor")
            .Do(() => player.PauseMovement())
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "There's another gate")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "I can power it up", "...", "This gate leads to the domain of the Keeper of Souls")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "Are you scared?")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "I've seen countless heroes fall to him.", "Are you sure you want to do this?")
            .ShowSpeechBox(SpeechBubblePrefab, player.gameObject, new Vector2(0, 1.0F), Color.white, "Don't worry. I'll find my way out, and if I can I'll get you out of this realm too.")
            .ShowSpeechBox(SpeechBubblePrefab, heart.gameObject, new Vector2(0, 0.5F), Extensions.HeartTextColor, "...", "I trust you.")
            .Do(() => heart.OverrideFocus(doorSlot))
            .Do(() => player.ResumeMovement())
            .WaitForSeconds(1.0F)
            // .Do(() => door.Open())
            .WaitForTrigger("AtDoor")
            .Do(() => player.PauseMovement())
            .FadeToBlack(OverlayPrefab)
            .DoCoroutine(Extensions.Fade(1.0F, 1.0F, 0.0F, volScale => music.volume = startVolume * volScale))
            .LoadLevel("Level3");
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
