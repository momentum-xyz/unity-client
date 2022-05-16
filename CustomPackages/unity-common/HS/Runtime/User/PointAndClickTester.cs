using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace HS
{
    public class PointAndClickTester : MonoBehaviour
    {
        [SerializeField] bool _onlyWithInactiveControls;
        [SerializeField] Texture2D _hoverCursor;
        [SerializeField] Vector2 _hoverCursorOffset;
        [SerializeField] LayerMask _mask;
        [SerializeField] float _clickDistance = 50;
        [SerializeField] ThirdPersonController thirdPersonController;

        Camera _cam;

        private void Awake()
        {
            thirdPersonController = GetComponent<ThirdPersonController>();
        }

        void Update()
        {
            // in paused state we want to deactivate all rollovers
            if (Time.timeScale == 0 || (_onlyWithInactiveControls && thirdPersonController.IsControlling))
            {
                Clickable.UpdateHover(null);
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }

            if (!_cam) _cam = Camera.main;
            if (!_cam) return;

            RaycastHit info;

            var ray = _cam.ScreenPointToRay(Input.mousePosition);

            Clickable winner = null;

            if (Physics.Raycast(ray, out info, _clickDistance, _mask, QueryTriggerInteraction.Collide))
            {
                winner = info.collider.GetComponent<Clickable>();
            }

            UpdateCursor(winner != null);

            Clickable.UpdateHover(winner);

        }

        void UpdateCursor(bool hover)
        {
            if (hover)
                Cursor.SetCursor(_hoverCursor, _hoverCursorOffset, CursorMode.Auto);
            else
                Cursor.SetCursor(null, Vector3.zero, CursorMode.Auto);

        }
    }
}