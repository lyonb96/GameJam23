using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatStarter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartMusic());
    }

    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(1);
        RhythmManager.GetInstance().PlayOnBeat(GetComponent<AudioSource>());
    }
}
