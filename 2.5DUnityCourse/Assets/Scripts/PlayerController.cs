using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer playerSprite;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;

    // Reference to a parameter in the animator
    private const string IS_WALK_PARAM = "IsWalking";

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // All of these values are retrieved from PlayerController input actions
        float x = playerControls.Player.Move.ReadValue<Vector2>().x;
        float z = playerControls.Player.Move.ReadValue<Vector2>().y;

        movement = new Vector3(x, 0, z).normalized;

        animator.SetBool(IS_WALK_PARAM, movement != Vector3.zero);

        if (x != 0 && x < 0) // moving to the left
        {
            playerSprite.flipX = true;
        }
        else if (x != 0 && x > 0) // moving to the right
        {
            playerSprite.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
