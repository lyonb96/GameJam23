using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartFollower : MonoBehaviour
{
    public Vector2 Offset = new(1.5F, 1.5F);

    public bool Follow = true;

    private Player Player;

    private GameObject Focus;

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
        var offset = Offset;
        if (Player.IsRunningRight)
        {
            offset = offset.With(x: offset.x * 2.0F);
        }
        var toFollow = Focus;
        if (toFollow == null)
        {
            toFollow = Player.gameObject;
        }
        else
        {
            offset = Vector2.zero;
        }
        var targetPos = toFollow.transform.position.Truncate() + offset;
        var currentPos = transform.position.Truncate();
        transform.position = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * 15.0F).Expand(transform.position.z);
    }

    public void OverrideFocus(GameObject focus)
    {
        Focus = focus;
    }
}
