using UnityEngine;

public class SlashAtaque : MonoBehaviour
{
    [SerializeField] private float duracionAtaque = 0.2f;

    [SerializeField] private int danoAtaque = 10;
    
    void Start()
    {
        Destroy(gameObject, duracionAtaque);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();

            if (enemigo != null)
            {
                enemigo.Recibirdano(danoAtaque);
            }
        }
    }
}