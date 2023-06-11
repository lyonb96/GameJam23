using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBobber : MonoBehaviour
{
    public float BobAmount = 0.5F;

    // Update is called once per frame
    void Update()
    {
        var beat = RhythmManager.GetInstance().GetBeatScalar();
        transform.localPosition = transform.localPosition.With(y: beat * beat * BobAmount);
    }
}
