using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStructureState
{
    public void SetState<T>(string label, T value);
    public T GetState<T>(string label);

}
