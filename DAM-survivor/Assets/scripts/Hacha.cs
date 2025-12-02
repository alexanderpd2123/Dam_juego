using UnityEngine;

public class Hacha : MonoBehaviour
{
    [Header("Datos del Hacha")]
    public float speed = 10f;
    public float tiempoVida = 5f;
    public int damage = 25;

    // Update is called once per frame
    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Recibirdano(damage);
            }
        }
    }
  
}