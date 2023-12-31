using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Extensions
{
    public static Vector2 Truncate(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 Expand(this Vector2 v, float z = 0.0F)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }

    public static Vector2 With(this Vector2 v, float? x = null, float? y = null)
    {
        return new Vector2(x ?? v.x, y ?? v.y);
    }

    public static Color WithAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public static IEnumerable<Collider2D> CheckForHits(
        Vector2 center,
        Vector2 extent,
        string ignoreTag = null,
        GameObject[] ignores = null)
    {
        ignores ??= Array.Empty<GameObject>();
        var hits = Physics2D.OverlapBoxAll(center, extent, 0.0F);
        foreach (var hit in hits)
        {
            if (hit.GetComponent<Health>() != null
                && !ignores.Contains(hit.gameObject)
                && (ignoreTag == null || !hit.CompareTag(ignoreTag)))
            {
                yield return hit;
            }
        }
    }

    public static IEnumerator WriteText(
        string fullText,
        Action<string> eachStep,
        int lettersPerSec = 10,
        float startDelay = 3.0F,
        float endDelay = 2.0F,
        Action onComplete = null)
    {
        yield return new WaitForSeconds(startDelay);
        for (var i = 0; i < fullText.Length + 1; i++)
        {
            eachStep(fullText[..i]);
            yield return new WaitForSeconds(1.0F / lettersPerSec);
        }
        yield return new WaitForSeconds(endDelay);
        onComplete?.Invoke();
    }

    public static IEnumerator Fade(float t, float from, float to, Action<float> setter)
    {
        var direction = from > to ? -1.0F : 1.0F;
        var counter = from;
        while (counter != to)
        {
            counter = Mathf.Clamp(counter + ((Time.deltaTime / t) * direction), Mathf.Min(from, to), Mathf.Max(from, to));
            setter(counter);
            yield return null;
        }
    }

    public static Color HeartTextColor = new(245.0F / 255.0F, 144.0F / 255.0F, 154.0F / 255.0F, 1.0F);
}

public class CooldownTimer
{
    public float Duration;

    private float LastUse;

    public void Use()
    {
        LastUse = Time.time;
    }

    public bool IsOnCooldown => LastUse + Duration > Time.time;
}

public class LevelScript
{
    private List<ScriptEvent> events = new();

    public bool Continued;

    public bool WaitingForContinue { get; private set; }

    private List<string> Triggers = new();

    private Func<UnityEngine.Object, Vector3, Quaternion, Transform, UnityEngine.Object> Instantiate;

    private Action<UnityEngine.Object> Destroy;

    private GameObject Canvas;

    public LevelScript(
        Func<UnityEngine.Object, Vector3, Quaternion, Transform, UnityEngine.Object> instantiate,
        Action<UnityEngine.Object> destroy,
        GameObject canvas)
    {
        Instantiate = instantiate;
        Destroy = destroy;
        Canvas = canvas;
    }

    public IEnumerator Execute()
    {
        var index = 0;
        while (index < events.Count)
        {
            var currentEvent = events[index];
            switch (currentEvent)
            {
                case WriteText writer:
                    yield return Extensions.WriteText(writer.Text, s => writer.TextBox.text = s, endDelay: 0, startDelay: 1.5F);
                    index++;
                    continue;
                case WaitForContinue continuer:
                    WaitingForContinue = true;
                    if (!Continued)
                    {
                        yield return new WaitForSeconds(0.1F);
                        continue;
                    }
                    Continued = false;
                    WaitingForContinue = false;
                    index++;
                    continue;
                case DoEvent doer:
                    doer.DoAction();
                    index++;
                    continue;
                case DoCoroutine doCoroutine:
                    yield return doCoroutine.Coroutine;
                    index++;
                    continue;
                case WaitForSecondsEvent waitForSec:
                    yield return new WaitForSeconds(waitForSec.Seconds);
                    index++;
                    continue;
                case WaitForTrigger trigger:
                    if (!Triggers.Contains(trigger.TriggerName))
                    {
                        yield return new WaitForSeconds(0.1F);
                        continue;
                    }
                    index++;
                    continue;
                case ShowSpeechBox showSpeechBox:
                    var bubble = Instantiate(showSpeechBox.Prefab, Vector3.zero, Quaternion.identity, Canvas.transform) as GameObject;
                    var script = bubble.GetComponentInChildren<SpeechBubble>();
                    script.Text = showSpeechBox.Text;
                    script.Speaker = showSpeechBox.Speaker;
                    script.Offset = showSpeechBox.Offset;
                    script.Color = showSpeechBox.TextColor;
                    yield return script.StartWriting();
                    index++;
                    continue;
                case FadeToBlack fadeToBlack:
                    var pos = Camera.main.transform.position;
                    var rotation = Camera.main.transform.rotation;
                    var overlay = Instantiate(fadeToBlack.Prefab, pos, rotation, Canvas.transform) as GameObject;
                    var fadeScript = overlay.GetComponent<Fader>();
                    yield return fadeScript.Fade(0.0F, 1.0F, fadeToBlack.Seconds);
                    index++;
                    continue;
                case FadeFromBlack fadeFromBlack:
                    var pos2 = Camera.main.transform.position;
                    var rotation2 = Camera.main.transform.rotation;
                    var overlay2 = Instantiate(fadeFromBlack.Prefab, pos2, rotation2, Canvas.transform) as GameObject;
                    var fadeScript2 = overlay2.GetComponent<Fader>();
                    yield return fadeScript2.Fade(1.0F, 0.0F, fadeFromBlack.Seconds, true);
                    index++;
                    continue;
                case LoadLevel level:
                    SceneManager.LoadScene(level.Level);
                    index++;
                    continue;
            }
        }
    }

    public void Continue()
    {
        Continued = true;
    }

    public void Trigger(string name)
    {
        if (!Triggers.Contains(name))
        {
            Triggers.Add(name);
        }
    }

    public LevelScript WriteText(string text, TextMeshProUGUI textBox)
    {
        events.Add(new WriteText
        {
            Text = text,
            TextBox = textBox,
        });
        return this;
    }

    public LevelScript WaitForContinue()
    {
        events.Add(new WaitForContinue());
        return this;
    }

    public LevelScript Do(Action doEvent)
    {
        events.Add(new DoEvent { DoAction = doEvent });
        return this;
    }

    public LevelScript DoCoroutine(IEnumerator routine)
    {
        events.Add(new DoCoroutine { Coroutine = routine });
        return this;
    }

    public LevelScript WaitForTrigger(string triggerName)
    {
        events.Add(new WaitForTrigger { TriggerName = triggerName });
        return this;
    }

    public LevelScript WaitForSeconds(float seconds)
    {
        events.Add(new WaitForSecondsEvent { Seconds = seconds });
        return this;
    }

    public LevelScript ShowSpeechBox(
        GameObject prefab,
        GameObject speaker,
        Vector2 offset,
        Color textColor,
        params string[] text)
    {
        events.Add(new ShowSpeechBox
        {
            Prefab = prefab,
            Text = text,
            Speaker = speaker,
            Offset = offset,
            TextColor = textColor,
        });
        return this;
    }

    public LevelScript FadeToBlack(GameObject prefab, float seconds = 1.0F)
    {
        events.Add(new FadeToBlack
        {
            Prefab = prefab,
            Seconds = seconds,
        });
        return this;
    }

    public LevelScript FadeFromBlack(GameObject prefab, float seconds = 1.0F)
    {
        events.Add(new FadeFromBlack
        {
            Prefab = prefab,
            Seconds = seconds,
        });
        return this;
    }

    public LevelScript LoadLevel(string level)
    {
        events.Add(new LoadLevel { Level = level });
        return this;
    }
}

public class ScriptEvent
{
}

public class WriteText : ScriptEvent
{
    public string Text;

    public TextMeshProUGUI TextBox;
}

public class WaitForContinue : ScriptEvent
{
}

public class DoEvent : ScriptEvent
{
    public Action DoAction;
}

public class DoCoroutine : ScriptEvent
{
    public IEnumerator Coroutine;
}

public class WaitForTrigger : ScriptEvent
{
    public string TriggerName;
}

public class ShowSpeechBox : ScriptEvent
{
    public string[] Text;

    public GameObject Speaker;

    public GameObject Prefab;

    public Vector2 Offset;

    public Color TextColor;
}

public class WaitForSecondsEvent : ScriptEvent
{
    public float Seconds;
}

public class FadeToBlack : ScriptEvent
{
    public GameObject Prefab;

    public float Seconds;
}

public class FadeFromBlack : ScriptEvent
{
    public GameObject Prefab;

    public float Seconds;
}

public class LoadLevel : ScriptEvent
{
    public string Level;
}