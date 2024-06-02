using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct SpawnData
{
    public Vector2 startPos;
    public Vector2 destPos;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [SerializeField]
    private List<SpawnData> _monsterSpawns = new();

    private int _monstertPoolSize;

    [SerializeField]
    private PlayerControl _player;
    public PlayerControl Player
    {
        get { return _player; }
    }

    private SaveControl _savePointControl;
    private string _saveFilePath;

    private void Awake()
    {
        _instance = this;
        _monstertPoolSize = _monsterSpawns.Count;
        MonsterManager.Instance.CreateMonsters(_monstertPoolSize);

        for (int i = 0; i < _monstertPoolSize; i++)
        {
            MonsterManager.Instance.GetData(_monsterSpawns[i]);
        }

        MonsterManager.Instance.Spawn();
    }

    private void Start()
    {
        _saveFilePath = Application.dataPath + "/savepoint.dat";
        //LoadSavePoint();
    }

    public void SetSavePoint(Vector3 position)
    {
        _savePointControl = new SaveControl();
        _savePointControl.x = position.x;
        _savePointControl.y = position.y;
        _savePointControl.z = position.z;

        FileStream fileStream = new(_saveFilePath, FileMode.Create);
        BinaryFormatter binary = new BinaryFormatter();
        binary.Serialize(fileStream, _savePointControl);
        fileStream.Close();
    }

    private void LoadSavePoint()
    {
        if (File.Exists(_saveFilePath))
        {
            FileStream fileStream = new FileStream(_saveFilePath, FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();
            _savePointControl = binary.Deserialize(fileStream) as SaveControl;
        }
        else
        {
            _savePointControl = new SaveControl
            {
                x = 0, y = 0, z = 0
            };
        }
    }

    public Vector3 GetSavePoint()
    {
        return new Vector3(_savePointControl.x, _savePointControl.y, _savePointControl.z);
    }

    public void RespawnPlayer(PlayerControl player)
    {
        player.SpawnAtSavePoint();
    }

    private void OnDrawGizmos()
    {
        foreach (SpawnData data in _monsterSpawns)
        {
            Vector2 size = new Vector2(5.0f, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawCube(data.startPos, size);

            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(data.destPos, size);
        }
    }
}
