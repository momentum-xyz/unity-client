using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HS;
using Nextensions;

public class AlphaUserPlatformDriver : UserPlatformDriver
{

    public List<Transform> postersCache;
    public List<Transform> screensCache;
    public List<Transform> badgesCache;
    public List<Transform> slotsCache;

    public PlasmaballDriver[] plasmaBallDrivers;
    public PlasmaTetherAttachShape tetherShapeCache;

    void Awake()
    {
        PopulateFromCache();
    }

    void PopulateFromCache()
    {

        FillFromCache<IPosterRenderer>(postersCache, "@poster", _posters);
        FillFromCache<CustomContentRenderer>(screensCache, "@screen", _screens);
        FillFromCache<CustomContentRenderer>(badgesCache, "@badge", _badges);
        FillFromCache<Transform>(slotsCache, "@slot", _slots);

        _plasmaBalls = new HashSet<PlasmaballDriver>(plasmaBallDrivers);
        if (_forceField) _forceField.gameObject.SetActive(false);

        // Not used anywhere
        //_tetherShape = tetherShapeCache;
    }

    void FillFromCache<T>(List<Transform> cache, string surfaceType, Dictionary<string, HashSet<T>> container)
    {
        for (var i = 0; i < cache.Count; ++i)
        {
            var type = (cache[i].gameObject.name.SplitTag(surfaceType) ?? "default").ToUpper();

            if (container.ContainsKey(type) == false) container.Add(type, new HashSet<T>());

            T comp = cache[i].GetComponent<T>();

            container[type].Add(comp);
        }
    }
}
