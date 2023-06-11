using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartFollower : MonoBehaviour
{
    public Vector2 Offset = new(1.5F, 1.5F);

    public bool Follow = true;

    private Player Player;

    private void Awake()
    {
        Player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Follow)
        {
            return;
        }
        var targetPos = Player.transform.position.Truncate() + Offset;
        var currentPos = transform.position.Truncate();
        transform.position = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * 15.0F).Expand(transform.position.z);
    }
}
