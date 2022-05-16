using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HS
{
    public class LODTether : LODable
    {
        public override int LOD => -1;
        [Header( "(LOD setting above is ignored, this object always changes)" )]
        // public int[] SizePerLOD =
        //     {
        //         8,
        //         5,
        //         3,
        //         2
        //     };

        PlasmaTether _line;

        void Awake()
        {
            _line = GetComponent<PlasmaTether>();
        }


        public override void SetLOD( int newLOD )
        {
            if( Application.isPlaying && _line != null )
			{
				_line.RecalculateWithLOD( newLOD );
				// _line.RecalculateWith( SizePerLOD[newLOD] );
                // _line.BezierPoints = SizePerLOD[newLOD];
			}
        }
    }
}
