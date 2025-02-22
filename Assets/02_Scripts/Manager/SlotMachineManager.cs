using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SlotMachineManager : MonoSingleton<SlotMachineManager>
{
    [System.Serializable]
    public class DisplayItemSlot
    {
        public List<Image> SlotSprite = new List<Image> ( );
    }
    public SORarityEffect SOEffects;
    
    [SerializeField] private GameObject[] slotSkillObject;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;
    [SerializeField] private float spriteSize = 250f;
    [SerializeField] private float spinSpeed = 0.02f; // Lower HighSpeed
    [SerializeField] private DisplayItemSlot[] displayItemSlots;
    [SerializeField] private GameObject parentPanel;

    private List<int> _startList = new List<int> ( );
    private List<int> _resultIndexList = new List<int> ( );
    private List<Effect> _effectList = new List<Effect> ( );
    private Entity _playerEntity;
    private int _itemCnt => displayItemSlots[0].SlotSprite.Count - 1; // 마지막을 제외한 Slot 갯수
    private int[] _answer = { 2, 1, 1 }; //


    void Start ( )
    {
        _playerEntity = GameObject.FindGameObjectWithTag ( "Player" ).GetComponent<Entity>();
        parentPanel.SetActive(false);
    }

    public void MachineStart()
    {
        parentPanel.SetActive(true);
        Time.timeScale = 0f;
        Setting();
    }

    private void Setting()
    {
        for ( int i = 0 ; i < _itemCnt * slotButtons.Length; i++ )
        {
            _startList.Add ( i );
        }
        
        for ( int i = 0 ; i < slotButtons.Length ; i++ )
        {
            slotButtons[i].interactable = false;
        }

        for ( int i = 0 ; i < slotButtons.Length; i++ )
        {
            for ( int j = 0 ; j < _itemCnt; j++ )
            {
                int randomIndex = Random.Range ( 0, _startList.Count );
                if ( i == 0 && j == 1 || i == 1 && j == 2 || i == 2 && j == 2 )
                {
                    _resultIndexList.Add ( _startList[randomIndex] );
                    var effect = SOEffects.effects[_startList[randomIndex]].Clone() as Effect;
                    effect.Setup(_playerEntity.gameObject,_playerEntity, 1);
                    _effectList.Add(effect);
                }
                displayItemSlots[i].SlotSprite[j].sprite = SOEffects.effects[_startList[randomIndex]].Icon;
                
                //처음과 마지막 같게
                if ( j == 0 )
                {
                    displayItemSlots[i].SlotSprite[_itemCnt].sprite = SOEffects.effects[_startList[randomIndex]].Icon;
                }
                _startList.RemoveAt ( randomIndex );
            }
        }

        for ( int i = 0 ; i < slotButtons.Length; i++ )
        {
            float yValue = slotSkillObject[i].transform.localPosition.y - spriteSize/2;
            int i1 = i;
            slotSkillObject[i].transform.DOLocalMoveY(yValue, 0.5f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
                StartCoroutine(SpinningSlot(i1)));
        }
    }
    
    IEnumerator SpinningSlot(int SlotIndex)
    {
        int index = SlotIndex;
        for (int i = 0; i < (_itemCnt * (6 + index * _itemCnt * 2) + _answer[index]) * 2 - 2; i++)
        {
            slotSkillObject[index].transform.localPosition -= new Vector3(0, spriteSize / 2, 0);
            if (slotSkillObject[index].transform.localPosition.y <= spriteSize / 2)
            {
                slotSkillObject[index].transform.localPosition += new Vector3(0, spriteSize * _itemCnt, 0);
            }
            yield return new WaitForSecondsRealtime(spinSpeed); 
        }

        for (int i = 0; i < slotButtons.Length; i++)
        {
            slotButtons[i].interactable = true;
        }

        float yValue = slotSkillObject[index].transform.localPosition.y - spriteSize / 2;
        slotSkillObject[index].transform
            .DOLocalMoveY(yValue, 1f)
            .SetEase(Ease.OutElastic)
            .SetUpdate(true)
            .OnUpdate(() =>
            {
                if (slotSkillObject[SlotIndex].transform.localPosition.y <= spriteSize / 2)
                {
                    slotSkillObject[index].transform.localPosition += new Vector3(0, spriteSize * _itemCnt, 0);
                }
            })
            .OnComplete(() => ShowDescription(index));
    }

    public void ClickBtn ( int index )
    {
        _playerEntity.SkillSystem.Apply(_effectList[index]);
        this.gameObject.SetActive ( false );
        Time.timeScale = 1f;
    }

    private void ShowDescription(int index)
    {
        Debug.Log( index );
        descriptionTexts[index].text = _effectList[index].Description;
    }
}