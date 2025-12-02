using UnityEngine;

public class RayoDeEnergia : MonoBehaviour
{
    [SerializeField] private float duracionTotal = 3.0f;
    [SerializeField] private float intervaloDano = 0.25f;
    [SerializeField] private int danoPorTick = 2;
    
    private float tiempoDesdeUltimoDano;
    
    private System.Collections.Generic.List<EnemyController> enemigosImpactados = 
        new System.Collections.Generic.List<EnemyController>();

    void Start()
    {
        Invoke("DesacoplarYDestruir", duracionTotal); 
        tiempoDesdeUltimoDano = 0f; 
    }

    void Update()
    {
        tiempoDesdeUltimoDano += Time.deltaTime;
            
        if (tiempoDesdeUltimoDano >= intervaloDano)
        {
            tiempoDesdeUltimoDano = 0f;
            AplicarDanoATodosLosEnemigos();
        }
    }
    
    private void AplicarDanoATodosLosEnemigos()
    {
        for (int i = enemigosImpactados.Count - 1; i >= 0; i--)
        {
            EnemyController enemigo = enemigosImpactados[i];
            
            if (enemigo != null) 
            {
                enemigo.Recibirdano(danoPorTick);
            }
            else
            {
                enemigosImpactados.RemoveAt(i);
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            
            if (enemigo != null && !enemigosImpactados.Contains(enemigo))
            {
                enemigosImpactados.Add(enemigo);
                enemigo.Recibirdano(danoPorTick);
            }
        }
    }
    
    private void DesacoplarYDestruir()
    {
        if (this != null) 
        {
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            Destroy(gameObject);
        }
    }
}