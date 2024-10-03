using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] PlayerStateSummary player;
    [SerializeField] Image healthbarImage;
    public float healthPercentage;

    void Update()
    {
        healthbarImage.fillAmount = player.GetHealth() / (float)player.GetMaxHealth();
    }

    private void OnValidate()
    {
        Update();
    }
}