﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using General.TutorialData;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace General
{

    
    public class TutorialHandler : MonoBehaviour
    {
        [SerializeField] private TutorialData.TutorialData m_PopUpData;
        [SerializeField] private UIHUD m_HUD;

        private bool m_WeaponInfoAlreadyDisplayed;
        
        private void OnEnable()
        {
            GameEvents.OnTutoAsked += DisplayTutorial;
            
            // TODO: Bind events
        }
        
        private void OnDisable()
        {
            GameEvents.OnTutoAsked -= DisplayTutorial;
        }

        public void DisplayTutorial(TutorialEvent _event)
        {
            int indexOfFirstMatching = -1;
            for (int i = 0; i < m_PopUpData.Length; ++i)
            {
                if (m_PopUpData[i].Event == _event)
                {
                    indexOfFirstMatching = i;
                    break;
                }
            }

            if (indexOfFirstMatching == -1)
            {
                Debug.Log("TutorialHandler: Can't find data for the given tutorial event: " + _event);
                return;
            }

            DisplayTutorialItems(GetChainFrom(indexOfFirstMatching));
        }

        private void DisplayTutorialItems(IEnumerator<TutorialItem> _items)
        {
            if (_items.MoveNext())
            {
                string buttonLabel = _items.Current.EndSequence ? "OK" : "Next";
                DisplayPopUp(_items.Current, buttonLabel, 
                    () =>
                    {
                        DisplayTutorialItems(_items);
                    });
            }
            else
            {
               m_HUD.HidePopup(); 
            }
        }

        private IEnumerator<TutorialItem> GetChainFrom(int _index)
        {
            var data = m_PopUpData[_index];
            if (m_PopUpData.OnWeaponHarvestedEvents.Contains(data.Event) && !m_WeaponInfoAlreadyDisplayed)
            {
                m_WeaponInfoAlreadyDisplayed = true;
                yield return m_PopUpData.OnWeaponHarvestedData;
            }
            
            for (; _index < m_PopUpData.Length; ++_index)
            {

                yield return m_PopUpData[_index];
                if (m_PopUpData[_index].EndSequence)
                {
                    yield break;
                }
            }
        }

        private void DisplayPopUp(TutorialItem _data, string _closeLabel, Action _onClose)
        {
            m_HUD.ShowPopup(_data.Title, _data.Sprite, _data.Text, false, _closeLabel, _onClose);
        }
        
        #if UNITY_EDITOR
        [SerializeField] private TutorialEvent m_DebugEvent;

        [Button]
        public void DrawTutorialPopUp()
        {
            DisplayTutorial(m_DebugEvent);
        }
        #endif
    }
}