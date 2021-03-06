﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.FieldCompleteMainMenu
{
    public class FriendsPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Animator panelAnimator;

        void Start()
        {
            panelAnimator = this.GetComponent<Animator>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            panelAnimator.Play("Friends Panel In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            panelAnimator.Play("Friends Panel Out");
        }
    }
}