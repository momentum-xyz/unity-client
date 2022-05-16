using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nextensions;


namespace HS
{  
    /// <summary> Just a basic pooler, mainly for testing in dev. 
    /// You can create a pooled instance by delivering the prefab original.
    /// The pooler itself will check and track all poolable objects.
    /// If you request a pooled object but don't deliver a poolable prefab, it will return null.
    /// </summary>
    public class Pool : MonoBehaviour
    {
        public static Pool Instance;


        // every 'poolable' prefab gets its own pool
        Dictionary<GameObject,HashSet<GameObject>> _pools = new Dictionary<GameObject, HashSet<GameObject>>();
        // per poolable, we track the in-use instances only (the not-in-use ones are the subtraction of the whole ppol minus the in-use ones)
        Dictionary<GameObject,HashSet<GameObject>> _inUse = new Dictionary<GameObject, HashSet<GameObject>>();
        // every poolable should have a Poolable component, at least denoting its maximum count, but we also track the original prefab in it.
        Dictionary<GameObject,int> _maxCounts = new Dictionary<GameObject,int>();


		
        /// <summary> Attempts to spawn a prefab from the pool and place it at the given 
		/// location. Returns null if unsuccessful (fi because the pool is empty) </summary>
		public GameObject GetSpawnFromPrefab( GameObject prefab, Transform spawnLocation )
		{
			var spawn = GetSpawnFromPrefab( prefab );
			spawn?.transform.Pose( spawnLocation );
			return spawn;
		}


        /// <summary> Attempts to spawn a prefab from the pool, or null if not allowed. </summary>
        public GameObject GetSpawnFromPrefab( GameObject prefab )
        {
            if( prefab.scene != null && prefab.scene.name != null )
            {
                Debug.LogError( $"POOL tryna spawn from a non-prefab {prefab}. This is not allowed." );
                return null;
            }
            if( prefab.GetComponent<Poolable>() == null )
            {
                Debug.LogError( $"POOL: please add a PoolableSettings to {prefab.name} before using it!" );
                return null;
            }

            if( _pools.Keys.Contains( prefab ) == false ) // first use of this specific prefab, we need to prep its indexation
            {
                _pools.Add( prefab, new HashSet<GameObject>() );
                if( _inUse.Keys.Contains( prefab ) ) _inUse.Remove( prefab ); // sanity
                _inUse.Add( prefab, new HashSet<GameObject>() );
                if( _maxCounts.Keys.Contains( prefab ) ) _maxCounts.Remove( prefab ); // sanity
                _maxCounts.Add( prefab, prefab.GetComponent<Poolable>().MaxAmount );
            }

            if( _inUse[prefab].Count >= _maxCounts[prefab] )
            // the pool is full! Abort!
            {
                // Debug.Log($"Pool limit reached for {prefab.name}");
                return null;
            }


            // we can spawn/deliver an object!
            GameObject newOp = null;

            if( _pools[prefab].Count < _maxCounts[prefab] && _inUse[prefab].Count >= _pools[prefab].Count )
            // all current instances are in use but there's room in the pool
            {
                newOp = Instantiate( prefab );
                // newOp.transform.SetParent( transform, true );
                newOp.GetComponent<Poolable>().Origin = prefab;
                _pools[prefab].Add( newOp );
            }
            else if( _inUse[prefab].Count < _pools[prefab].Count ) 
            // sanity: are we sure there's room for activating another instance?
            {
                // find the first inactive instance that isn't in use yet.
                newOp = _pools[prefab].FirstOrDefault( instance => _inUse[prefab].Contains(instance) == false );
                if( newOp == null )
                {
                    Debug.LogError( $"Weirdly, we both concluded that there are unused pool objects, AND we couldn't find one." );
                    return null;
                }
            }
            else
            {
                Debug.LogError( $"Weirdly, the pool seems to be both full AND have space for new stuff." );
                return null;
            }


            // found our new or re-activatable object!
            newOp.SetActive( true );
            _inUse[prefab].Add( newOp );
            return newOp;
        }


        /// <summary> Either return an object to the pool or destroy it (if not poolable). </summary>
        public void Return( GameObject returnObject )
        {
            if( returnObject == null ) return;

            if( returnObject.GetComponent<Poolable>() == null )
            {
                Debug.Log( $"DESTROY: No Poolable Component found on returned object {returnObject.name}." );
                Destroy( returnObject );
                return;
            }
            var prefab = returnObject.GetComponent<Poolable>().Origin;
            if( prefab == null )
            {
                Debug.Log( $"DESTROY: No prefab origin found on {returnObject.name}." );
                Destroy( returnObject );
                return;
            }


            if( _pools.Keys.Contains( prefab ) == false )
            {
                // TODO: make this so that a pool gets created (NB: this should be a very rare occurence)
                Debug.Log( $"DESTROY: We don't maintain a pool for {returnObject.name}." );
                Destroy( returnObject );
                return;
            }
            if( _inUse.Keys.Contains( prefab ) == false )
            {
                Debug.Log( $"We're not actualy using the returned object {returnObject.name}. But we do have a pool for it." );
                // return;
            }



            _inUse[prefab].Remove( returnObject );
            returnObject.SetActive( false );
            returnObject.transform.SetParent( transform, true );
        }


        /// <summary> instances of Poolable that get destroyed call this function, so that the Pool has a chance to clean
        /// its datastructures when objects accidentally disappear (fi thanks to a scene unload) </summary>
        public void Untrack( Poolable poolable )
        {
            if( poolable == null ) return;
            if( poolable.Origin == null ) return;

            _pools[poolable.Origin].Remove( poolable.gameObject );
            _inUse[poolable.Origin].Remove( poolable.gameObject );            
        }


        void Awake()
        {
            if( Instance != null && Instance != this )
            {
                Destroy( gameObject );
            }

            Instance = this;
        }


		void OnDestroy()
		{
			if ( Instance == this ) Instance = null;
		}
    }
}
