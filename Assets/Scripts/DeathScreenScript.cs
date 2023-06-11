using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreenScript : MonoBehaviour
{
    private bool WaitForContinue;

    private bool Continue;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunDeathScreen());
    }

    private IEnumerator RunDeathScreen()
    {
        var items = GetComponentsInChildren<Graphic>();
        var bgOpacity = items.First(x => x.name.StartsWith("DeathScreen")).color.a;
        foreach (var item in items)
        {
            if (item.name == "FadeOverlay")
            {
                continue;
            }
            item.color = item.color.WithAlpha(0.0F);
        }
        yield return new WaitForSeconds(1);
        Debug.Log(bgOpacity);
        yield return Extensions.Fade(1.25F, 0.0F, 1.0F, opacity =>
        {
            foreach (var item in items)
            {
                if (item.name == "FadeOverlay")
                {
                    continue;
                }
                var target = 1.0F;
                if (item.name.StartsWith("DeathScreen"))
                {
                    target = bgOpacity;
                }
                item.color = item.color.WithAlpha(opacity * target);
            }
        });
        WaitForContinue = true;
        while (!Continue)
        {
            yield return new WaitForSeconds(0.1F);
        }
        WaitForContinue = false;
        var fadeOverlay = items.First(x => x.name == "FadeOverlay");
        yield return Extensions.Fade(1.5F, 0.0F, 1.0F, opacity => fadeOverlay.color = fadeOverlay.color.WithAlpha(opacity));
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
        if (WaitForContinue && Input.GetButtonDown("Continue"))
        {
            Continue = true;
        }
    }
}
