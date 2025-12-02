using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int currentHealth;
    private int maxHealth = 100;
    private int ataque = 5;
    private int defensa = 0;
    private float velMov = 5f;

    private float velAtk = 1f;

    private bool estaVivo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake() 
    {
        currentHealth = maxHealth;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    //////////////////////////////// Funciones propias /////////////////////////
    
    public void RecibirDmg(int dmg)
    {
        if (!estaVivo) return;

        // Solo recibe daño si el daño es mayor que la defensa.
        if (dmg > defensa)
        {
            //Le quitamos el daño menos la defensa
            currentHealth -= dmg - defensa;

            //Si la vida es menor que 0 lo matamos
            if (currentHealth <= 0) estaVivo = false;
        }
    }
}
