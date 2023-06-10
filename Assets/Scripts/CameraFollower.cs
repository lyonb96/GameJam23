using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Camera SceneCam;

    // public GameObject Focus;

    public Vector2 Offset;

    public float Speed = 10.0F;

    public float VerticalFlatten = 0.1F;

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
        var targetPos = transform.position.With(y: transform.position.y * VerticalFlatten).Truncate() + Offset;
        var currentPos = SceneCam.transform.position.Truncate();
        var newPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * Speed);
        SceneCam.transform.position = new Vector2(newPos.x, Mathf.Max(0.0F, newPos.y)).Expand(SceneCam.transform.position.z);
    }
}
