using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDisabled : MonoBehaviour
{
	void Awake() => gameObject.SetActive( false );
}
