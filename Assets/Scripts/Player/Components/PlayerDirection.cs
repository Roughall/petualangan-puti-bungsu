using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    public enum FacingDirection
    {
        Down,
        Up,
        Left,
        Right
    }

    [Header("Current Direction")]
    [SerializeField]
    private FacingDirection currentDirection = FacingDirection.Down;

    public FacingDirection CurrentDirection
    {
        get { return currentDirection; }
    }

    public void SetDirection(Vector2 moveInput)
    {
    // Jangan ubah arah jika Player sedang diam
        if (moveInput.sqrMagnitude < 0.01f)
        return;

    // Prioritaskan sumbu yang paling besar
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                    currentDirection = FacingDirection.Right;
                else
                    currentDirection = FacingDirection.Left;
            }
                else
                {
                    if (moveInput.y > 0)
                    currentDirection = FacingDirection.Up;
                    else
                    currentDirection = FacingDirection.Down;
                }
        Debug.Log("[PlayerDirection] " + currentDirection);
    }
}