using System;
using UnityEngine;

namespace General.TutorialData
{
    [Serializable]
    public struct TutorialItem
    {
        public TutorialEvent Event;
        public string Title;
        public string Text;
        public Sprite Sprite;
        public bool IsSequence;
        public bool EndSequence;
        
    }
    
    [CreateAssetMenu(fileName = "TutorialData", menuName = "LD52/Tutorial/Data", order = 0)]
    public class TutorialData : ScriptableObject
    {
        [SerializeField] private TutorialItem[] m_PopUpData;

        [SerializeField] public TutorialEvent[] OnWeaponHarvestedEvents;
        [SerializeField] public TutorialItem OnWeaponHarvestedData;
        
        public int Length => m_PopUpData.Length;
        public TutorialItem this[int _index] => m_PopUpData[_index];
    }
}