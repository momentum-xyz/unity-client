using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace Odyssey
{

    public interface IStructureMover
    {
        public UniTask MoveStructure(Transform structureTransform, Transform structureParentTransform, Vector3 newPosition, bool lookAtParent = true);
    }


    public class StructureMover : IStructureMover
    {
        private float _moveTime = 2.0f;

        public StructureMover(float moveTime)
        {
            _moveTime = moveTime;
        }

        /// <summary>
        /// Animated movement of a structure for _moveTime time
        /// </summary>
        /// <param name="wo"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public async UniTask MoveStructure(Transform structureTransform, Transform structureParentTransform, Vector3 newPosition, bool lookAtParent = true)
        {

            Vector3 oldPosition = new Vector3(0, 0, 0);

            float elapsedTime = 0f;
            float waitTime = _moveTime;

            oldPosition = structureTransform.position;

            while (elapsedTime < waitTime)
            {
                if (structureTransform != null)
                {
                    structureTransform.position = Vector3.Lerp(oldPosition, newPosition, (elapsedTime / waitTime));

                    if (structureParentTransform != null && lookAtParent)
                    {
                        structureTransform.LookAt(new Vector3(structureParentTransform.position.x, structureTransform.position.y, structureParentTransform.position.z));
                    }
                }

                elapsedTime += Time.deltaTime;

                await UniTask.WaitForEndOfFrame();
            }

            if (structureTransform != null) structureTransform.position = newPosition;
        }
    }

}
