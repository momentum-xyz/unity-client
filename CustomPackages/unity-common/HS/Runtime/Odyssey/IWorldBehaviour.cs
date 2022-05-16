using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldBehaviour 
{
    public void InitBehaviour();
    public void UpdatePrivacy(bool isPrivate, bool currentUserCanEnter);

    public void UpdateLOD(int lodLevel);
    public void UpdateBehaviour(float dt);
    public void FixedUpdateBehaviour(float dt);

}
