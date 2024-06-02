using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct CharacterData
{
    public int Key;
    public string Name;
    public float WalkSpeed;
    public float RunSpeed;
    public float JumpPower;
    public int JumpCount;
    public float MaxHp;
    public float Power;
    public float SkillPower;
}

public class DataTable : Singleton<DataTable>
{
    private Dictionary<int, CharacterData> characterDatas = new Dictionary<int, CharacterData>();

    public CharacterData GetCharacterData(int key)
    {
        return characterDatas[key];
    }

    private void Awake()
    {
        LoadData();
    }

    public void LoadData()
    {
        LoadCharacterTable();
    }

    private void LoadCharacterTable()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("TextData/CharacterTable");

        string temp = textAsset.text.Replace("\r\n", "\n");

        string[] str = temp.Split('\n');

        for (int i = 1; i < str.Length; i++)
        {
            string[] data = str[i].Split(',');

            CharacterData characterData;
            characterData.Key = int.Parse(data[0]);
            characterData.Name = data[1];
            characterData.WalkSpeed = float.Parse(data[2]);
            characterData.RunSpeed = float.Parse(data[3]);
            characterData.JumpPower = float.Parse(data[4]);
            characterData.JumpCount = int.Parse(data[5]);
            characterData.MaxHp = float.Parse(data[6]);
            characterData.Power = float.Parse(data[7]);
            characterData.SkillPower = float.Parse(data[8]);

            characterDatas.Add(characterData.Key, characterData);
        }
    }
}
