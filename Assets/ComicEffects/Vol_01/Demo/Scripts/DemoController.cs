using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoController : MonoBehaviour {

    [SerializeField]
    private int _index = 0;

    [SerializeField]
    Text _effectNameText;

    [System.Serializable]
    public struct EffectData
    {
        public string ItemName;
        public GameObject ScenePrefab; 
    }

    public EffectData[] DemoList;

    private void Start()
    {
        _effectNameText.text = DemoList[_index].ItemName;
        PlayEffect();
    }

    public void PlayEffect()
    {
        DemoList[_index].ScenePrefab.SetActive(false);
        DemoList[_index].ScenePrefab.SetActive(true);
    }

    public void Next()
    {
        DemoList[_index].ScenePrefab.SetActive(false);
        _index++;
        _index = _index % DemoList.Length;
        _effectNameText.text = DemoList[_index].ItemName;
        PlayEffect();
    }

    public void Last()
    {
        DemoList[_index].ScenePrefab.SetActive(false);
        _index--;
        if (_index < 0)
            _index = DemoList.Length - 1;
        _effectNameText.text = DemoList[_index].ItemName;
        PlayEffect();
    }
}
