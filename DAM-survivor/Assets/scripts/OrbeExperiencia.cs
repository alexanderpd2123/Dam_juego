using UnityEngine;

public class OrbeExperiencia : MonoBehaviour
{
    [Tooltip("Cantidad de EXP que otorga este orbe (10, 50, o 100).")]
    public int expValue = 10;
    
    // Asumimos que el jugador tiene la etiqueta "Player"
    private const string PLAYER_TAG = "Player"; 

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si colisionó con el jugador
        if (other.CompareTag(PLAYER_TAG))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            
            if (playerStats != null)
            {
                // 1. Otorgar la EXP al jugador
                playerStats.GainEXP(expValue);
                
                // 2. Destruir el orbe
                Destroy(gameObject);
            }
        }
    }
}