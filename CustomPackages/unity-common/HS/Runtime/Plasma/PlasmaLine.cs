using System.Collections.Generic;
using UnityEngine;

namespace HS
{
    public class PlasmaLine : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer myLineRenderer;

        //Objects to draw a line between
        public Transform beginPoint;
        public Transform endPoint;

        public float firstOffset;
        public float secondOffset;

        [SerializeField]
        private Transform lineBeginEffect, lineEndEffect;

        public float maxDistance;
 
        public void UpdatePositions()
        {
            Vector3 startPos = CalcRadiusPosition(endPoint.position, beginPoint.position, firstOffset);
            Vector3 endPos = CalcRadiusPosition(beginPoint.position, endPoint.position, secondOffset);

            if (lineBeginEffect) lineBeginEffect.position = startPos;
            if (lineEndEffect) lineEndEffect.position = endPos;

            myLineRenderer.SetPositions(new Vector3[2] { startPos, endPos });
        }

        void Update()
        {
            UpdatePositions();
        }

        private Vector3 CalcRadiusPosition(Vector3 point1, Vector3 point2, float radius)
        {
            return (point2 + (point1 - point2).normalized * radius);
        }
    }
}