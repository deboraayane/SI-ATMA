using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereMovement : MonoBehaviour
{
    
    public float speed = 6f;
    public float jumpForce = 6f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    Rigidbody rb;
    Collider col;
    float moveInput;
    bool wantJump;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        if (groundCheck == null) groundCheck = transform;
    }

    void Update()
    {
        // Captura entrada (esquerda/direita)
        moveInput = Input.GetAxisRaw("Horizontal"); // -1, 0 ou 1

        // Captura intenção de pular apenas com a barra de espaço
        if (Input.GetKeyDown(KeyCode.Space))
            wantJump = true;
    }

    void FixedUpdate()
    {
        // Aplica movimento horizontal preservando velocidade vertical
        Vector3 vel = rb.linearVelocity;
        vel.x = moveInput * speed;
        rb.linearVelocity = vel;

        // Aplica pulo via força impulsiva somente quando estiver no chão
        if (wantJump && IsGrounded())
        {
            // zera componente vertical antes de aplicar para pulos consistentes
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            wantJump = false;
        }
        // opcional: descarta intenção se muito tempo passar (não implementado aqui)
    }

    bool IsGrounded()
    {
        // Calcula posição de verificação na base do colisor (evita checar no centro da esfera)
        Vector3 origin = (groundCheck != null) ? groundCheck.position : transform.position;
        float bottomOffset = (col != null) ? col.bounds.extents.y : 0f;
        Vector3 checkPos = origin + Vector3.down * bottomOffset;

        return Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Collider c = col ? col : GetComponent<Collider>();
        float bottomOffset = (c != null) ? c.bounds.extents.y : 0f;
        Vector3 checkPos = groundCheck.position + Vector3.down * bottomOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}