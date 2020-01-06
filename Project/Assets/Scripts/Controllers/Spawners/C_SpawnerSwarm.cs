using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnerSwarm : C_Spawner
{
    [SerializeField]
    C_Pather path = null;

    protected override GameObject SpawnEnemy()
    {
        //Call parent method
        GameObject spawnedEnemy = base.SpawnEnemy();

        C_PathedEnemy swarmComponent = spawnedEnemy.GetComponent<C_PathedEnemy>();
        if(swarmComponent != null)
        {
            swarmComponent.SetPathToFollow(path);
        }

        return spawnedEnemy;
    }
}
