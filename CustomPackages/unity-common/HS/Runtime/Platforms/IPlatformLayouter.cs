using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlatformLayouter
{


	void Generate( GameObject programSpacePrefab, GameObject challengeSpacePrefab, GameObject teamSpacePrefab );
	void DrawGizmos();

}
