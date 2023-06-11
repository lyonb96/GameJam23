using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public UnityEvent OnPlayerEnter;

    private void Awake()
    {
        OnPlayerEnter ??= new();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.SetCheckpoint(transform.position);
            OnPlayerEnter.Invoke();
        }
    }
}
