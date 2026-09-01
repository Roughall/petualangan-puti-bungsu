using UnityEngine;

public class BossArenaBoundary : MonoBehaviour
{
    [Header("Boundary")]
    public string boundaryName;

    private void Start()
    {
        Debug.Log(
            "[BOSS ARENA BOUNDARY READY] " +
            boundaryName
        );
    }
}