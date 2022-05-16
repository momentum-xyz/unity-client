using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITextSlot
{
    public string GetLabel();
    public void SetText(string label, string text);
}
