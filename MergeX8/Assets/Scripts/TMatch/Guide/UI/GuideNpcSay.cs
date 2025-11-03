using DragonPlus;
using UnityEngine;

namespace OutsideGuide
{
    public class GuideNpcSay : GuideGraphicBase
    {
        public LocalizeTextMeshProUGUI m_DialogText;
        public Transform m_DialogGroup;
        public Transform m_DialogNoNpc;
        private GameObject selfGO;
        private Transform _npcIcon;

        private Transform leftPos;
        private Transform rightPos;
        private bool isInit;

        protected override void Init()
        {
            if (isInit) return;
            selfGO = this.gameObject;
            m_DialogNoNpc = transform.Find("TextGroup");
            m_DialogGroup = transform.Find("Dialog");
            _npcIcon = transform.Find("CharacterGroup");
            leftPos = transform.Find("Left");
            rightPos = transform.Find("Right");
            isInit = true;
        }

        public override void Show()
        {
            if (!IsShow()) selfGO.SetActive(true);
        }

        public void Show(string key)
        {
            Show();
            m_DialogText.SetText(LocalizationManager.Instance.GetLocalizedString(key));
        }

        public override void Hide()
        {
            if (IsShow()) selfGO.SetActive(false);
        }

        public void NpcSay(string key, Vector2 canvasPoint, int whoSay, DialogType dialogType = DialogType.None,NpcPos npcPos = NpcPos.Left, Vector2? characterOffset = null, float scale = 1.0f)
        {
            Init();
            for (int i = 0; i < _npcIcon.childCount; i++)
            {
                _npcIcon.GetChild(i).gameObject.SetActive(false);
            }
            
            for (int i = 0; i < m_DialogGroup.childCount; i++)
            {
                m_DialogGroup.GetChild(i).gameObject.SetActive(false);
            }
            transform.localScale = new Vector3(scale, scale, 1);
            m_DialogNoNpc.gameObject.SetActive(dialogType == DialogType.None);
            m_DialogGroup.gameObject.SetActive(dialogType != DialogType.None);
            switch (dialogType)
            {
                case DialogType.None:
                    m_DialogText = m_DialogNoNpc.Find($"Text").GetComponent<LocalizeTextMeshProUGUI>();
                    break;
                case DialogType.Left:
                case DialogType.Right:
                    var child = m_DialogGroup.GetChild((int) dialogType - 1);
                    child.gameObject.SetActive(true);
                    m_DialogText = child.Find($"Text").GetComponent<LocalizeTextMeshProUGUI>();
                    break;
            }
            if (whoSay < _npcIcon.childCount && whoSay >= 0)
            {
                Transform npc = _npcIcon.GetChild(whoSay);
                switch (npcPos)
                {
                    case NpcPos.Left:
                        npc.position = leftPos.position + (characterOffset == null ? Vector3.zero : new Vector3(characterOffset.Value.x, characterOffset.Value.y));
                        npc.localScale=Vector3.one;
                        break;
                    case NpcPos.Right:
                        npc.position = rightPos.position + (characterOffset == null ? Vector3.zero : new Vector3(characterOffset.Value.x, characterOffset.Value.y));
                        npc.localScale = new Vector3(-1, 1, 1);
                        break;
                }
                npc.gameObject.SetActive(true);
            }

            m_DialogText.SetText(LocalizationManager.Instance.GetLocalizedString(key));
            m_DialogText.transform.parent.localPosition = canvasPoint;
            Show(key);
        }

        public override bool IsShow()
        {
            return selfGO.activeSelf;
        }

    }
}