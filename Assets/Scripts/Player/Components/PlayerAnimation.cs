using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int updateCount = 0;

    public void Initialize(Animator anim, SpriteRenderer sprite)
    {
        animator = anim;
        spriteRenderer = sprite;

        Debug.Log("[PlayerAnimation] Initialize");
    }

    public void UpdateAnimation(Vector2 moveInput)
{
    if (animator == null)
    {
        Debug.LogError("Animator NULL");
        return;
    }

    if (spriteRenderer == null)
    {
        Debug.LogError("SpriteRenderer NULL");
        return;
    }

    //------------------------------------
    // Flip Sprite
    //------------------------------------

    if (moveInput.x > 0)
        spriteRenderer.flipX = true;
    else if (moveInput.x < 0)
        spriteRenderer.flipX = false;

    //------------------------------------
    // Animator Parameter
    //------------------------------------

    bool isMoving = moveInput.sqrMagnitude > 0.01f;

    animator.SetBool("IsMoving", isMoving);
    animator.SetFloat("InputX", moveInput.x);
    animator.SetFloat("InputY", moveInput.y);

    //------------------------------------
    // DEBUG
    //------------------------------------

    if (Time.frameCount % 30 == 0)
    {
        Debug.Log("===== PLAYER ANIMATION =====");

        Debug.Log("MoveInput = " + moveInput);

        Debug.Log("IsMoving = " + animator.GetBool("IsMoving"));

        Debug.Log("InputX = " + animator.GetFloat("InputX"));

        Debug.Log("InputY = " + animator.GetFloat("InputY"));

        Debug.Log("Current State = " +
            animator.GetCurrentAnimatorStateInfo(0).shortNameHash);

        Debug.Log("===========================");
    }
}
}