using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Managers.Models;
using UnityEngine;

namespace Gameplay.Managers.Spawner
{
    public sealed class SpawnerPoints : MonoBehaviour
    {
        [SerializeField] private List<SpawnPoint> _spawnPoints;
        
        public Transform GetFreePoint(ETeam teamType)
        {
            var team = _spawnPoints.FirstOrDefault(t => t.Team == teamType);
            
            if(team == null)
                return null;
            
            var freePoint = team.SpawnPoints.FirstOrDefault(p => p.IsFree);
            
            if(freePoint == null)
                return null;

            freePoint.IsFree = false;
            return freePoint.Transform;
        }

        public void SetAllIsFree()
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                foreach (var point in spawnPoint.SpawnPoints)
                {
                    point.IsFree = true;
                }
            }
        }

        [Serializable]
        private sealed class SpawnPoint
        {
            [field:SerializeField] public ETeam Team { get; private set; }
            [field:SerializeField] public List<Point> SpawnPoints { get; private set; }
        }

        [Serializable]
        private sealed class Point
        {
            [field:SerializeField] public Transform Transform { get; private set; }
            
            public bool IsFree { get; set; }
        }
    }
}