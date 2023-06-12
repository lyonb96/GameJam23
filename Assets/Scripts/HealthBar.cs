using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Health PlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        var player = FindObjectOfType<Player>();
        PlayerHealth = player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        var percent = PlayerHealth.CurrentHealthPercent;
        transform.localScale = transform.localScale.With(x: percent);
    }
}
