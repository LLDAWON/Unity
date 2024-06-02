using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>
{
    public GameObject _monsterPrefab;
    public List<GameObject> monsters = new List<GameObject>();
    public List<SpawnData> data = new List<SpawnData>();

    private void Awake()
    {
        _monsterPrefab = Resources.Load<GameObject>("Prefabs/Monster_Minion");
    }

    public void CreateMonsters(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(_monsterPrefab, transform);
            obj.SetActive(false);
            monsters.Add(obj);
        }
    }

    public void GetData(SpawnData spawnData)
    {
        data.Add(spawnData);        
    }

    public void Spawn()
    {
        for(int i = 0; i < data.Count;i++)
        {
            monsters[i].GetComponent<MonsterControl>().Spawn(data[i]);
        }
    }
}
