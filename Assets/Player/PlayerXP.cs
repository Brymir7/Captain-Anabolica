using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerXp : MonoBehaviour
    {
        public int level = 1;
        [FormerlySerializedAs("currentXP")] private int _currentXp = 0;
        [FormerlySerializedAs("baseXP")] private int _baseXp = 100; // XP required for level 1
        public float xpMultiplier = 1.5f; // Adjust this for curve steepness
        [SerializeField] private LevelManager levelManager;

        public void AddXp(int amount)
        {
            _currentXp += amount;
            if (_currentXp >= GetXpRequirementForNextLevel())
            {
                LevelUp();
            }
        }

        public int GetCurrentXp()
        {
            return _currentXp;
        }

        public int GetXpRequirementForNextLevel()
        {
            return Mathf.FloorToInt(_baseXp * Mathf.Pow(xpMultiplier, level - 1));
        }

        private void LevelUp()
        {
            level++;
            _currentXp = 0; // Optionally reset XP after level up, or carry over remaining XP
            levelManager.OnPlayerLevelUp(level);
        }
    }
}