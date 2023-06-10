using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeatAccuracy
{
    Perfect,
    Early,
    Late,
    Miss,
}

public class RhythmManager : MonoBehaviour
{
    public int BeatsPerMinute;

    private float TimeBetweenBeats => 60.0F / (float)BeatsPerMinute;

    private SpriteRenderer Sprite;

    private static RhythmManager _instance;

    // Start is called before the first frame update
    void Start()
    {
        Sprite = GetComponent<SpriteRenderer>();
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        var acc = GetBeatScalar();
        acc = acc*acc;
        Sprite.transform.localScale = new Vector3(acc, acc, acc);
    }

    public float GetBeatScalar()
    {
        var between = Time.time % TimeBetweenBeats;
        // var last = Mathf.Floor(Time.time / TimeBetweenBeats) * TimeBetweenBeats;
        // var next = Mathf.Ceil(Time.time / TimeBetweenBeats) * TimeBetweenBeats;
        // return Mathf.Max(last )
        var asPercent = between / TimeBetweenBeats;
        // return asPercent;
        return Mathf.Abs((asPercent - 0.5F) * 2.0F);
    }

    public BeatAccuracy GetBeatAccuracy(float perfectTolerance, float maxTolerance)
    {
        var beatScalar = GetBeatScalar();
        var between = Time.time % TimeBetweenBeats;
        if (beatScalar >= perfectTolerance)
        {
            return BeatAccuracy.Perfect;
        }
        if (beatScalar >= maxTolerance)
        {
            if (between > 0.5F)
            {
                return BeatAccuracy.Early;
            }
            return BeatAccuracy.Late;
        }
        return BeatAccuracy.Miss;
    }

    public static RhythmManager GetInstance() => _instance;
}
