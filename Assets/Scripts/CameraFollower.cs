using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Camera SceneCam;

    // public GameObject Focus;

    public Vector2 Offset;

    public float Speed = 10.0F;

    // Start is called before the first frame update
    void Start()
    {
        SceneCam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneCam == null)
        {
            return;
        }
        var targetPos = transform.position.Truncate() + Offset;
        var currentPos = SceneCam.transform.position.Truncate();
        SceneCam.transform.position = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * Speed).Expand(SceneCam.transform.position.z);
    }
}
