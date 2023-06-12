using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // private Camera SceneCam;

    private Player Player;

    public GameObject Focus;

    public Vector2 Offset;

    public float Speed = 10.0F;

    public float VerticalFlatten = 0.1F;

    public float MinX = -1000.0F;

    public float MaxX = 1000.0F;

    // Start is called before the first frame update
    void Start()
    {
        Player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        var follow = Focus;
        if (follow == null)
        {
            follow = Player.gameObject;
        }
        var targetPos = follow.transform.position.With(
                x: Mathf.Clamp(follow.transform.position.x, MinX, MaxX),
                y: Player.transform.position.y * VerticalFlatten)
            .Truncate() + Offset;
        var currentPos = transform.position.Truncate();
        var newPos = Vector2.Lerp(currentPos, targetPos, Time.deltaTime * Speed);
        transform.position = new Vector2(newPos.x, Mathf.Max(0.0F, newPos.y)).Expand(transform.position.z);
    }

    public void OverrideFocus(GameObject go)
    {
        Focus = go;
    }
}
