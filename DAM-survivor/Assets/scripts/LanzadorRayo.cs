using UnityEngine;

public class LanzadorRayo : MonoBehaviour
{
    [SerializeField] private GameObject prefabRayo;

    [SerializeField] private float cooldownRayo = 5.0f;
    private float tiempoSiguienteUso; 
    
    [SerializeField] private float desplazamientoDelantero = 1.0f; 

    void Update()
    {
        if (Time.time >= tiempoSiguienteUso)
        {
            ActivarRayo();
        }
    }

    private void ActivarRayo()
    {
        Vector3 posicionInstanciacion = 
            transform.position + 
            transform.forward * desplazamientoDelantero; 
            
        GameObject rayoInstanciado = Instantiate(
            prefabRayo, 
            posicionInstanciacion, 
            transform.rotation 
        );
        rayoInstanciado.transform.SetParent(this.transform);
        
        tiempoSiguienteUso = Time.time + cooldownRayo;
    }
}