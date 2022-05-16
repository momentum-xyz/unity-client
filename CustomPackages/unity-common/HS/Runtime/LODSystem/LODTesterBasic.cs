using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODTesterBasic : MonoBehaviour
{
    public List<GameObject> LODS;
    public List<float> Distances;


    Camera _cam;

    int _lodCount;


    void Start()
    {
        _lodCount = LODS.Count;
        if( _lodCount != Distances.Count )
        {
            Debug.LogError( "Different LOD count and LOD distances!" );
            Destroy( this );
            return;
        }

        var pos = LODS[0].transform.position;
        var rot = LODS[0].transform.rotation;

        foreach( var op in LODS )
        {
            op.transform.position = pos;
            op.transform.rotation = rot;
            op.SetActive( op == LODS[0] );
        }
    }



    void Update()
    {
        if( !_cam ) _cam = Camera.main;

        var dist = (LODS[0].transform.position - _cam.transform.position ).sqrMagnitude;

        bool foundLOD = false;

        for( int i = 0; i < _lodCount; i++ )
        {
            var inRange = dist<Distances[i]*Distances[i];
            LODS[i].SetActive( inRange && !foundLOD );
            foundLOD |= inRange;
        }
    }
}
