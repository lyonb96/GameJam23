using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private Health PlayerHealth;

    public bool PlayerMode;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerMode)
        {
            var player = FindObjectOfType<Player>();
            PlayerHealth = player.GetComponent<Health>();
        }
        else
        {
            var boss = FindObjectOfType<Boss>();
            PlayerHealth = boss.GetComponent<Health>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var percent = PlayerHealth.CurrentHealthPercent;
        transform.localScale = transform.localScale.With(x: percent);
    }
}
