using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }

    private GameObject _playerHP;
    private Transform _bloodParent;
    private Transform _mpParent;
    private Transform _monsterHpPanel;

    private List<GameObject> bloods = new List<GameObject>();
    private List<GameObject> playerMPs = new List<GameObject>();

    public Transform MonsterHpPanel
    {
        get { return _monsterHpPanel; }
    }

    private void Awake()
    {
        _instance = this;
        _playerHP = GameObject.Find("UI/bgHp_bar").gameObject;

        _bloodParent = GameObject.Find("UI/Bloods").transform;
        _mpParent = GameObject.Find("UI/MP").transform;
        _monsterHpPanel = GameObject.Find("UI/HpBarPanel").transform;

        bloods.Clear();
        playerMPs.Clear();

        for (int i = 0; i < _bloodParent.childCount; i++)
        {
            bloods.Add(_bloodParent.GetChild(i).gameObject);
        }

        for(int i = 0; i < _mpParent.childCount; i++)
        {
            playerMPs.Add(_mpParent.GetChild(i).gameObject);
        }
       
    }
    public void PlayerHPValue(float min, float max)
    {
        _playerHP.GetComponent<Slider>().value = min / max;
    }

    public void PlayerSmoke(int index)
    {
        print(bloods.Count);
        bloods[index].transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PlayerReSet()
    {
        foreach(GameObject blood in bloods)
        {
            if(!blood.transform.GetChild(0).gameObject.activeSelf)
            {
                blood.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        foreach(GameObject mp in playerMPs)
        {
            if(mp.transform.GetChild(0).gameObject.activeSelf)
            {
                mp.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    

    public void PlayerMP(int index, bool _mp)
    {
        playerMPs[index].transform.GetChild(0).gameObject.SetActive(_mp);
    }

}
