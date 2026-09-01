using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 2;

    private int currentHP;

    public int CurrentHP
    {
        get { return currentHP; }
    }
    public int MaxHP
    {
        get { return maxHP; }
    }

    void Start()
    {
        currentHP = maxHP;

        Debug.Log(gameObject.name + " HP = " + currentHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log(gameObject.name + " HP = " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void RecoverHealth(float amount)
    {
        currentHP += Mathf.CeilToInt(amount);

        if (currentHP > maxHP)
            currentHP = maxHP;

        Debug.Log(gameObject.name + " [RECOVERY] HP = " + currentHP + "/" + maxHP);
    }
    void Die()
    {
        Debug.Log(gameObject.name + " Mati");

        Destroy(gameObject);
    }
}