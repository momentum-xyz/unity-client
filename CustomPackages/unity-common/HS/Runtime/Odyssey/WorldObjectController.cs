using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Odyssey
{

    public interface IWorldObjectController
    {
        public Guid GUID { get; set; }

        public Transform CustomCenter { get; set; }

        public Transform TeleportCenter { get; set; }
        public Vector3 TeleportOffset { get; set; }

        public bool LookAtParent { get; set; }

        public Transform ParentTransform { get; set; }

        public void InitBehaviours();
        public void UpdateBehaviours(float dt);
        public void UpdateLOD(int lod);

    }

    public class WorldObjectController : MonoBehaviour, IWorldObjectController, ITextureDataHandler, IStringsDataHandler, INumbersDataHandler
    {
        [Header("Data Handlers")]

        [SerializeField]
        private List<TextureHandlerComp> textureDataHandlers = new List<TextureHandlerComp>();

        [SerializeField]
        private List<StringsHandlerComp> stringsHandlers = new List<StringsHandlerComp>();

        [SerializeField]
        private List<NumbersHandlerComp> numbersHandlers = new List<NumbersHandlerComp>();

        [Header("Behaviours")]

        [SerializeField]
        private List<WorldBehaviour> worldBehaviours = new List<WorldBehaviour>();

        [Header("Extras")]
        [SerializeField]
        private Transform customCenter;

        [SerializeField]
        private Transform teleportCenter;

        [SerializeField]
        private Vector3 teleportOffset;

        [SerializeField]
        private bool lookAtParent = true;

        public Transform CustomCenter { get { return customCenter; } set { } }
        public Transform TeleportCenter { get { return teleportCenter; } set { } }
        public Vector3 TeleportOffset { get { return teleportOffset; } set { } }
        public bool LookAtParent { get { return lookAtParent; } set { lookAtParent = value; } }
        public Transform ParentTransform { get { return parentTransform; } set { parentTransform = value; } }
        public Guid GUID { get { return guid; } set { guid = value; } }

        private int currentLOD = -1;
        private Transform parentTransform;
        private Guid guid;

        void OnEnable()
        {
            currentLOD = -1;
        }

        public void TextureUpdate(string label, Texture2D texture, float ratio = 1.0f)
        {
            for (var i = 0; i < textureDataHandlers.Count; ++i)
            {
                if (textureDataHandlers[i].receiveAllData || textureDataHandlers[i].Label == label)
                {
                    textureDataHandlers[i].SetTexture(label, texture, ratio);
                }
            }
        }
        public void UpdateString(string label, string value)
        {
            for (var i = 0; i < stringsHandlers.Count; ++i)
            {
                if (stringsHandlers[i].receiveAllData || stringsHandlers[i].Label == label)
                {
                    stringsHandlers[i].SetString(label, value);
                }
            }
        }

        public void InitBehaviours()
        {
            if (worldBehaviours == null) worldBehaviours = new List<WorldBehaviour>();

            for (var i = 0; i < worldBehaviours.Count; ++i)
            {
                worldBehaviours[i].InitBehaviour();
            }
        }

        public void UpdateBehaviours(float dt)
        {
            for (var i = 0; i < worldBehaviours.Count; ++i)
            {
                worldBehaviours[i].UpdateBehaviour(dt);
            }
        }

        public void UpdateLOD(int lod)
        {
            if (currentLOD == lod) return;

            for (var i = 0; i < worldBehaviours.Count; ++i)
            {
                worldBehaviours[i].UpdateLOD(lod);
            }

            currentLOD = lod;
        }

        public void UpdateNumber(string label, int value)
        {
            for (var i = 0; i < numbersHandlers.Count; ++i)
            {
                if (numbersHandlers[i].receiveAllData || numbersHandlers[i].Label == label)
                {
                    numbersHandlers[i].SetNumber(label, value);
                }
            }
        }

        public void UpdateNumber(string label, float value)
        {
            for (var i = 0; i < numbersHandlers.Count; ++i)
            {
                if (numbersHandlers[i].receiveAllData || numbersHandlers[i].Label == label)
                {
                    numbersHandlers[i].SetNumber(label, value);
                }
            }
        }

        public void UpdateNumber(string label, long value)
        {
            for (var i = 0; i < numbersHandlers.Count; ++i)
            {
                if (numbersHandlers[i].receiveAllData || numbersHandlers[i].Label == label)
                {
                    numbersHandlers[i].SetNumber(label, value);
                }
            }
        }
    }
}