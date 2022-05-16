using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Odyssey
{
    public interface ILODSystem
    {
        public void Init(IMomentumContext context);

        public int GetLODLevelForDistance(float distance);

        public void StartRunning();
        public void Stop();
        public void RunLOD(Vector3 cameraPosition);

        public void AddToLODCalculation(WorldObject wo);
        public void RemoveFromLODCalculation(WorldObject wo);

        public List<WorldObject> GetNearby();


        public float LODDistance1 { get; set; }
        public float LODDistance2 { get; set; }
        public float LODDistance3 { get; set; }
    }


    public class OctreeLODSystem : ILODSystem, IRequiresContext
    {
        public float LODDistance1 { get; set; }
        public float LODDistance2 { get; set; }
        public float LODDistance3 { get; set; }

        IMomentumContext _c;
        bool IsRunning = false;
        PointOctree<WorldObject> octree = null;
        List<WorldObject> nearby = new List<WorldObject>();

        public OctreeLODSystem()
        {
            octree = new PointOctree<WorldObject>(10000.0f, Vector3.zero, 10);
        }

        public void Init(IMomentumContext context)
        {
            _c = context;
        }

        public void AddToLODCalculation(WorldObject wo)
        {
            octree.Add(wo, wo.position);
        }
        public void RemoveFromLODCalculation(WorldObject wo)
        {
            octree.Remove(wo);
        }

        public void StartRunning()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;

        }

        public void RunLOD(Vector3 cameraPosition)
        {
            if (!IsRunning) return;

            // Get a little bit more than the last LOD distance, so calculations of the object's LOD is more accurate
            bool hasNearBy = octree.GetNearbyNonAlloc(cameraPosition, Mathf.Sqrt(LODDistance3 + 2500.0f), nearby);

            if (!hasNearBy) return;

            for (var i = 0; i < nearby.Count; ++i)
            {
                float distance = (nearby[i].position - cameraPosition).sqrMagnitude;

                int lod = GetLODLevelForDistance(distance);

                // object lod
                nearby[i].LOD = lod;

                // textures lod (load different size of textures based on distance)
                if (nearby[i].texturesLOD != lod) nearby[i].texturesDirty = true;

                nearby[i].texturesLOD = GetTextureLODForDistance(distance);

                AlphaStructureDriver structureDriver = nearby[i].GetStructureDriver();

                if (!structureDriver) continue;

                structureDriver.SetLOD(nearby[i].LOD);

            }
        }

        public List<WorldObject> GetNearby()
        {
            return nearby;
        }


        public int GetLODLevelForDistance(float distance)
        {

            if (distance < LODDistance1)
            {
                return 0;
            }
            else if (distance < LODDistance2)
            {
                return 1;
            }
            else if (distance < LODDistance3)
            {
                return 2;
            }

            return 3;
        }

        public int GetTextureLODForDistance(float distance)
        {

            if (distance < 10000.0f)
            {
                return 0;
            }

            return 1;
        }


    }
}