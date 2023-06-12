using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private LevelScript levelScript;

    public GameObject OverlayPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var canvas = FindObjectOfType<Canvas>().gameObject;
        levelScript = new LevelScript(Instantiate, Destroy, canvas)
            .FadeFromBlack(OverlayPrefab, 2.0F)
            .WaitForContinue()
            .FadeToBlack(OverlayPrefab, 2.0F)
            .LoadLevel("Level1");
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
}
