using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField]
    private Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI levelText;

    private int currentHealth;
    private int maxHealth;
    private int level;
    private Animator animator;

    private const string LEVEL_ABBV = "Lvl: ";
    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetStartingValues(int maxHealth, int currentHealth, int level)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.level = level;

        levelText.text = LEVEL_ABBV + level.ToString();
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void ChangeHealth(int curHealth)
    {
        this.currentHealth = curHealth;

        if (curHealth <= 0)
        {
            // Play death animation and destroy object
            PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }

        UpdateHealthBar();
    }

    public void PlayAttackAnimation()
    {
        animator.SetTrigger(IS_ATTACK_PARAM);
    }

    public void PlayHitAnimation()
    {
        animator.SetTrigger(IS_HIT_PARAM);
    }

    public void PlayDeathAnimation()
    {
        animator.SetTrigger(IS_DEAD_PARAM);
    }
}
