using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBobber : MonoBehaviour
{
    public float StartY;

    public float BobAmount = 0.5F;

    private void Start()
    {
        StartY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        var beat = RhythmManager.GetInstance().GetBeatScalar();
        //if (transform is RectTransform rt)
        //{
        //    rt.anchoredPosition = rt.anchoredPosition.With(y: beat * beat * BobAmount);
        //}
        //else
        //{
        //}
        var offset = beat * beat * BobAmount;
        transform.localPosition = transform.localPosition.With(y: StartY + offset);
    }
}
