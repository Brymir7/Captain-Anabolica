using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    [SerializeField] PlayerStateSummary player;
    [SerializeField] Image xpBarImage;
    void Update()
    {
        xpBarImage.fillAmount = player.GetXpProgress();
    }
}