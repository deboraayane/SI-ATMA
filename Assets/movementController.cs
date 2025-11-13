using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    // === Propriedades de Movimento ===
    public float speed = 3f;
    public float jumpForce = 4f;

    // === Verificação de solo ===
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    // === Estado público (Animator usa isso) ===
    public float CurrentHorizontalSpeed { get; private set; }
    public bool IsGrounded { get; private set; }

    [Tooltip("Se true, trava rigidbody no eixo Z (recomendado para jogos 2D em X/Y).")]
    public bool freezePositionZ = true;

    // === Privadas ===
    Rigidbody rb;
    float moveInput;
    bool wantJump;

    // Debug
    bool firstFrame = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody não encontrado!");
            enabled = false;
            return;
        }

        rb.freezeRotation = true;
        if (freezePositionZ)
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    void Start()
    {
        // Diagnóstico inicial
        Debug.Log($"MovementController.Start: position={transform.position}, freezePositionZ={freezePositionZ}, groundCheck={(groundCheck? groundCheck.name : "NULL")}, groundLayer={groundLayer.value}");
        Debug.Log($"MovementController.Start: initial rb.velocity={rb.linearVelocity}");
        IsGrounded = CheckGrounded();
        Debug.Log($"MovementController.Start: CheckGrounded() => {IsGrounded}");
    }

    void Update()
    {
        // 1. Captura entrada
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Captura intenção de pular
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wantJump = true;
            Debug.Log("MovementController: wantJump = true (input Space)");
        }

        // 3. Verifica o solo no Update
        IsGrounded = CheckGrounded();

        // Debug: relatar condições anormais na primeira frames
        if (firstFrame)
        {
            Debug.Log($"MovementController.Update(first): IsGrounded={IsGrounded}, rb.velocity={rb.linearVelocity}, moveInput={moveInput}");
            firstFrame = false;
        }

        // Log quando IsGrounded for false logo no início (ajuda a entender por que Jump inicia)
        if (!IsGrounded && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            Debug.LogWarning("MovementController: IsGrounded está false mas vertical velocity ~ 0 — verifique groundCheck/groundLayer/posicionamento do collider.");
        }
    }

    void FixedUpdate()
    {
        // 1. Aplica movimento horizontal preservando Z (ou forçando 0 se freezePositionZ)
        Vector3 vel = rb.linearVelocity; // use API correta
        vel.x = moveInput * speed;
        if (freezePositionZ) vel.z = 0f;
        rb.linearVelocity = vel;

        // Atualiza velocidade para Animator
        CurrentHorizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

        // 2. Aplica pulo
        if (wantJump && IsGrounded)
        {
            // limpa Y antes de aplicar impulso
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            if (freezePositionZ) v.z = 0f;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            wantJump = false;
            IsGrounded = false;
            Debug.Log("MovementController: JUMP EXECUTADO! rb.velocity=" + rb.linearVelocity);
        }
    }

    bool CheckGrounded()
    {
        if (groundCheck == null)
        {
            // Se não foi atribuído, avisar explicitamente — isso é causa comum de IsGrounded errôneo
            Debug.LogError("MovementController.CheckGrounded: groundCheck não atribuído no Inspector!");
            return false;
        }

        bool hit = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        return hit;
    }
}