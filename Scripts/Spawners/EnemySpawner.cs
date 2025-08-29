using Scripts.Interctables;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Spawners
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private List<Enemy> _possibleEnemies;
        [SerializeField] private float _spawnHeight = 0.5f;
        [SerializeField] private float _spawnRotationY = 180f;

        private void Start()
        {
            Enemy randomEnemy = _possibleEnemies[Random.Range(0, _possibleEnemies.Count)];
            Vector3 spawnPosition = new Vector3(transform.position.x, _spawnHeight, transform.position.z);
            Quaternion spawnRotation = Quaternion.Euler(new Vector3(0, _spawnRotationY, 0));

            Instantiate(randomEnemy, spawnPosition, spawnRotation);
        }
    }
}
