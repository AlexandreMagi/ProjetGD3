using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SpawnerTrigger : MonoBehaviour
{
    public void StartSpawn()
    {
        GetComponent<C_Spawner>().EnableSpawn();
    }
}
