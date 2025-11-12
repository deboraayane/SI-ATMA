using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    public Animator animator;
    public string speedParam = "Speed";
    public string isJumpingParam = "IsJumping";

    public Transform modelTransform;
    public bool flipWithScale = true;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    bool inAir;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (modelTransform == null) modelTransform = transform;
        if (groundCheck == null) groundCheck = transform;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // -1, 0, 1
        animator.SetFloat(speedParam, Mathf.Abs(h));

        if (h > 0.01f) FaceRight();
        else if (h < -0.01f) FaceLeft();

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            animator.SetBool(isJumpingParam, true);
            inAir = true;
        }
        if (inAir && IsGrounded())
        {
            animator.SetBool(isJumpingParam, false);
            inAir = false;
        }
    }

    void FaceRight()
    {
        if (flipWithScale) SetScaleSign(1f);
        else modelTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void FaceLeft()
    {
        if (flipWithScale) SetScaleSign(-1f);
        else modelTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void SetScaleSign(float sign)
    {
        Vector3 s = modelTransform.localScale;
        s.x = Mathf.Abs(s.x) * (sign >= 0f ? 1f : -1f);
        modelTransform.localScale = s;
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}