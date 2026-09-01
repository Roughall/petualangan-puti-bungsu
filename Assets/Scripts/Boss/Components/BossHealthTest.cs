using UnityEngine;

public class BossHealthTest : MonoBehaviour
{
    private BossHealth bossHealth;

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();

        if (bossHealth == null)
        {
            Debug.LogError(
                "[BOSS HEALTH TEST] BossHealth NOT FOUND");
            return;
        }

        Debug.Log(
            "[BOSS HEALTH TEST] Ready | HP = " +
            bossHealth.CurrentHP +
            "/" +
            bossHealth.MaxHP);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(
                "[BOSS HEALTH TEST] Applying 10 damage");

            bossHealth.TakeDamage(10);
        }
    }
}