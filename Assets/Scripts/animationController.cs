using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public Transform modelTransform;
    public bool flipWithScale = true;

    // Configuração de velocidades (ajuste no Inspector se quiser)
    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float runDelay = 0.05f; // segundos para começar a correr

    private MovementController movement;

    float walkTimer = 0f;
    float originalSpeed;

    enum MoveState { Idle, Walking, Running }
    MoveState state = MoveState.Idle;

    void Start()
    {
        movement = GetComponentInParent<MovementController>();
        if (movement == null)
        {
            Debug.LogError("MovementController não encontrado. Animação desativada.");
            enabled = false;
            return;
        }

        if (animator == null) animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>() ?? GetComponentInParent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator não atribuído. Desabilitando script.");
            enabled = false;
            return;
        }

        // Evita que o Animator mova o transform (root motion pode sobrescrever rotações)
        animator.applyRootMotion = false;

        if (modelTransform == null) modelTransform = transform;

        originalSpeed = movement.speed;
    }

    void Update()
    {
        if (!enabled || animator == null || movement == null) return;

        // Atualiza Speed e flag de pulo
        animator.SetFloat("Speed", movement.CurrentHorizontalSpeed);
        animator.SetBool("IsJumping", !movement.IsGrounded);

        float hInput = Input.GetAxisRaw("Horizontal");
        bool inputPressed = Mathf.Abs(hInput) > 0.01f;
        bool grounded = movement.IsGrounded;

        // Handle facing visually
        if (hInput > 0.01f) FaceRight();
        else if (hInput < -0.01f) FaceLeft();

        // Estado e transições
        switch (state)
        {
            case MoveState.Idle:
                if (inputPressed && grounded) EnterWalking();
                break;

            case MoveState.Walking:
                if (!inputPressed || !grounded) EnterIdle();
                else
                {
                    walkTimer += Time.deltaTime;
                    if (walkTimer >= runDelay) EnterRunning();
                }
                break;

            case MoveState.Running:
                if (!inputPressed || !grounded) EnterIdle();
                else movement.speed = runSpeed;
                break;
        }

        // Se pular enquanto andava/corria, volta ao idle
        if (!movement.IsGrounded && state != MoveState.Idle) EnterIdle();
    }

    void EnterWalking()
    {
        state = MoveState.Walking;
        walkTimer = 0f;
        movement.speed = walkSpeed;
        animator.SetBool("IsWalking", true);
    }

    void EnterRunning()
    {
        state = MoveState.Running;
        movement.speed = runSpeed;
        animator.SetBool("IsWalking", true);
    }

    void EnterIdle()
    {
        state = MoveState.Idle;
        walkTimer = 0f;
        movement.speed = originalSpeed;
        animator.SetBool("IsWalking", false);
    }

    // Métodos de Flip (agora aplicam rotação Y limpa)
    void FaceRight()
    {
        // se você usa flipWithScale, mantém compatibilidade
        if (flipWithScale) SetScaleSign(1f);
        // ajuste aqui a orientação correta do seu modelo (ex.: -90 ou 0)
        modelTransform.localRotation = Quaternion.Euler(0f, 90f, 0f);
    }

    void FaceLeft()
    {
        if (flipWithScale) SetScaleSign(-1f);
        modelTransform.localRotation = Quaternion.Euler(0f, -90f, 0f);
    }

    void SetScaleSign(float sign)
    {
        Vector3 s = modelTransform.localScale;
        s.x = Mathf.Abs(s.x) * (sign >= 0f ? 1f : -1f);
        modelTransform.localScale = s;
    }
}