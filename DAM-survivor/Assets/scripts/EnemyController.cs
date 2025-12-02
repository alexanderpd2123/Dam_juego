using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ... (Variables de Stats, HP, etc.) ...
    
    private GameObject player;
    public EnemyStats Stats; 
    
    private int maxHP;
    private int currentHP;
    private int damage;
    private int defense;
    private float baseSpeed; 
    private float currentSpeed; 
    
    
    void Awake()
    {
        maxHP = Stats.MaxHP;
        currentHP = maxHP;
        damage = Stats.Damage;
        defense = Stats.Defense;
        
        baseSpeed = Stats.Speed;
        currentSpeed = baseSpeed;
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direccion = player.transform.position - transform.position;
            direccion.Normalize();
            
            transform.position += direccion * currentSpeed * Time.deltaTime;
        }
    }
    
    // La función Recibirdano debe recibir un INT
    public void Recibirdano(int danio)
    {
        int danioFinal = danio - defense;
        if (danioFinal < 0)
        {
            danioFinal = 0;
        }
        currentHP -= danioFinal;
        
        if (currentHP <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Destroy(gameObject);
    }
    
    public void ApplySlow(float slowPercentage)
    {
        currentSpeed = baseSpeed * (1f - slowPercentage);
    }

    public void RemoveSlow()
    {
        currentSpeed = baseSpeed;
    }
}