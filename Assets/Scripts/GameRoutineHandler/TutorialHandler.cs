using System;
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

        private void OnEnable()
        {
            GameEvents.OnTutoAsked += DisplayTutorial;
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

            var data = m_PopUpData[indexOfFirstMatching];
            if (data.IsSequence)
            {
                DisplayChain(indexOfFirstMatching);
            }
            else
            {
                DisplayPopUp(data);
            }
        }

        private void DisplayChain(int _index)
        {
            var data = m_PopUpData[_index];

            if (!data.EndSequence)
            {
                DisplayPopUp(data, "Next", () => {DisplayChain(_index + 1);});
            } else {
                DisplayPopUp(data);
            }
        }

        private void DisplayPopUp(TutorialItem _data)
        {
            DisplayPopUp(_data, "OK", () => m_HUD.HidePopup());
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