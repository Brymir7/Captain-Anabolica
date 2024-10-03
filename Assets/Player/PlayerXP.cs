using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerXp : MonoBehaviour
    {
        public int level = 1;
        [FormerlySerializedAs("currentXP")] private int currentXp = 0;
        [FormerlySerializedAs("baseXP")] private int baseXp = 100; // XP required for level 1
        public float xpMultiplier = 1.5f; // Adjust this for curve steepness
        [SerializeField] private LevelManager levelManager;

        public void AddXp(int amount)
        {
            currentXp += amount;
            if (currentXp >= GetXpRequirementForNextLevel())
            {
                LevelUp();
            }
        }

        public int GetCurrentXP()
        {
            return currentXp;
        }

        public int GetXpRequirementForNextLevel()
        {
            return Mathf.FloorToInt(baseXp * Mathf.Pow(xpMultiplier, level - 1));
        }

        private void LevelUp()
        {
            level++;
            currentXp = 0; // Optionally reset XP after level up, or carry over remaining XP
            levelManager.OnPlayerLevelUp(level);
        }
    }
}