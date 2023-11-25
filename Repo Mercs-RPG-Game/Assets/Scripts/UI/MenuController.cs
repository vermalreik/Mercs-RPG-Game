using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GDE.GenericSelectionUI;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }
}
