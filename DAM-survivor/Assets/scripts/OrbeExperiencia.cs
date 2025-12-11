using UnityEngine;

public class OrbeExperiencia : MonoBehaviour
{
    [Tooltip("Cantidad de EXP que otorga este orbe (10, 50, o 100).")]
    public int expValue = 10;
    
    private void OnTriggerEnter(Collider other)
    {
        // 1. Intenta obtener PlayerStats en el objeto que colisionó.
        PlayerStats playerStats = other.GetComponent<PlayerStats>();
        
        // 2. Si no lo encuentra, busca en los objetos padre (para mayor robustez)
        if (playerStats == null)
        {
             playerStats = other.GetComponentInParent<PlayerStats>();
        }
        
        if (playerStats != null)
        {
            // 3. Otorgar la EXP al jugador
            playerStats.GainEXP(expValue);
            
            // 4. Destruir el orbe
            Destroy(gameObject);
        }
    }
}