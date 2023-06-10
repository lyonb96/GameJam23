using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float StartPos;
    private float Width;
    private GameObject Camera;
    public float ParallaxAmount;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.position.x;
        Width = GetComponent<SpriteRenderer>().bounds.size.x;
        Camera = FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var t = (Camera.transform.position.x * (1 - ParallaxAmount));
        var d = (Camera.transform.position.x * ParallaxAmount);

        transform.position = transform.position.With(x: StartPos + d);

        if (t > StartPos + Width)
        {
            StartPos += Width;
        }
        else if (t < StartPos - Width)
        {
            StartPos -= Width;
        }
    }
}
