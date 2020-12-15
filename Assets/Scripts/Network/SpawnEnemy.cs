﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEnemy : MonoBehaviour
{
    public bool isGame;

    public bool normal;
    public bool shotgun;
    public bool missile;
    public bool bomb;

    public float spawnTime;

    public int spawnNum;
    public int maxSpawnNum = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator spawnEnmey()
    {
        while(isGame && spawnNum < maxSpawnNum)
        {
            if (normal) normalSpawn();
            else if (shotgun) ;
            else if (missile) ;
            else if (bomb) ;

            spawnNum++;

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void normalSpawn()
    {
        PhotonNetwork.InstantiateRoomObject("EnemyTankAI", this.transform.position, this.transform.rotation);
    }

    private void shotgunSpawn()
    {
        PhotonNetwork.InstantiateRoomObject("EnemyTankAI", this.transform.position, this.transform.rotation);
    }

    private void missileSpawn()
    {
        PhotonNetwork.InstantiateRoomObject("EnemyTankAI", this.transform.position, this.transform.rotation);
    }

    private void bombSpawn()
    {
        PhotonNetwork.InstantiateRoomObject("EnemyTankAI", this.transform.position, this.transform.rotation);
    }
}
