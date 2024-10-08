using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer playerSprite;

    [SerializeField]
    private LayerMask grassLayer;

    [SerializeField]
    private int stepsInGrass;

    [SerializeField]
    private int minStepsToEncounter;

    [SerializeField]
    private int maxStepsToEncounter;

    private PlayerControls playerControls;
    private Rigidbody rb;
    private Vector3 movement;
    private bool movingInGrass;
    private float stepTimer;
    private int stepsToEncounter;
    private PartyManager partyManager;

    // Reference to a parameter in the animator
    private const string IS_WALK_PARAM = "IsWalking";
    private const float TIME_PER_STEP = 0.5f; // how long it takes to count as a step
    private const string BATTLE_SCENE = "BattleScene";

    private void Awake()
    {
        playerControls = new PlayerControls();
        CalculateStepsToEncounter();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        partyManager = FindFirstObjectByType<PartyManager>();

        if (partyManager.GetPosition() != Vector3.zero)
        {
            transform.position = partyManager.GetPosition();
        }
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

        // Retrieve all the colliders in GrassTriggers and check if the player is colliding with them
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);
        movingInGrass = colliders.Length > 0 && movement != Vector3.zero;

        if (movingInGrass)
        {
            stepTimer += Time.fixedDeltaTime;

            if (stepTimer >= TIME_PER_STEP)
            {
                stepsInGrass++;
                stepTimer = 0;

                if (stepsInGrass >= stepsToEncounter)
                {
                    partyManager.SetPosition(transform.position);
                    SceneManager.LoadScene(BATTLE_SCENE);
                }
            }
        }
    }

    private void CalculateStepsToEncounter()
    {
        stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
    }
}
