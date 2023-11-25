using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDE.GenericSelectionUI
{
    public class SelectionUI<T> : MonoBehaviour where  T : ISelectableItem
    {
        List<T> items;
        float selectedItem = 0;
        
        float selectionTimer = 0;

        const float selectionSpeed = 5;

        public void SetItems(List<T> items)
        {
            this.items = items;
            UpdateSelectionInUI();
        }

        public virtual void HandleUpdate()
        {
            UpdateSelectionTimer();
            float prevSelection = selectedItem;

            HandleListSelection();

            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);
        
            if(selectedItem != prevSelection)
                UpdateSelectionInUI();
        }

        void HandleListSelection()
        {
            float v = Input.GetAxis("Vertical");

            if(selectionTimer == 0 && Mathf.Abs(v) > 0.2f) // check if it's greater than a small value like 0.2f, just in case we're using a joystick. We don't want to change selection for small movements
            {
                selectedItem += -(int)Mathf.Sign(v); // when we press down we want to increase the selected item, but the value of the negative axis will be negative, so we negate it
            
                selectionTimer = 1 / selectionSpeed;
            }    
        }

        void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }

        void UpdateSelectionTimer()
        {
            if(selectionTimer > 0)
                selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }
    }
}


