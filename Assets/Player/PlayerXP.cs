using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerXp : MonoBehaviour
    {
        public int level = 1;
        [FormerlySerializedAs("currentXP")] public int currentXp = 0;
        [FormerlySerializedAs("baseXP")] public int baseXp = 100; // XP required for level 1
        public float xpMultiplier = 1.5f; // Adjust this for curve steepness

        public void AddXp(int amount)
        {
            currentXp += amount;
            if (currentXp >= GetXpForNextLevel())
            {
                LevelUp();
            }
        }

        public int GetXpForNextLevel()
        {
            return Mathf.FloorToInt(baseXp * Mathf.Pow(xpMultiplier, level - 1));
        }

        private void LevelUp()
        {
            level++;
            currentXp = 0; // Optionally reset XP after level up, or carry over remaining XP
            Debug.Log("Level Up! New level: " + level);
        }
    }
}