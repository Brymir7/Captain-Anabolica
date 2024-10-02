using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField] private PlayerStateSummary playerStateSummary;
    [SerializeField] private Text stateText;
    [SerializeField] private Slider healthSlider;

    private StringBuilder stringBuilder = new StringBuilder();

    void Update()
    {
        UpdateHealthUI();
        UpdateStateText();
    }

    void UpdateHealthUI()
    {
        int currentHealth = playerStateSummary.GetHealth();
        int maxHealth = playerStateSummary.GetMaxHealth();
        
        healthSlider.value = (float)currentHealth / maxHealth;
        healthSlider.GetComponentInChildren<Text>().text = $"{currentHealth} / {maxHealth}";
    }

    void UpdateStateText()
    {
        stringBuilder.Clear();
        var weaponCooldowns = playerStateSummary.GetWeaponCooldowns();
        for (int i = 0; i < weaponCooldowns.Length; i++)
        {
            stringBuilder.AppendLine($"Weapon {i} Cooldown: {weaponCooldowns[i]:F2}s");
        }
        stateText.text = stringBuilder.ToString();
    }
}