using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    /// <summary>
    /// Helper script that keeps Plasma Tethers live-updating in editor context.
    /// </summary>
    public class PlasmaTetherAlive : MonoBehaviour
    {
        PlasmaTether _tether;


        void Awake()
        {
            _tether = GetComponent<PlasmaTether>();
        }

#if UNITY_EDITOR
        void Update()
        {
            if (Application.isEditor && _tether && _tether.enabled)
                _tether.UpdateTetherGraphics();
        }
#endif
    }

}