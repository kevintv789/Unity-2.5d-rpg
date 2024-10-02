using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;

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
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
