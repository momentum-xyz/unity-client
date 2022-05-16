using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace HS
{
    /// <summary> 
    /// Automatically add child avatars to a plasmaball on start. Used for animations/renders.
    /// </summary>
    public class AddObjectGroupToPlasmaBall : MonoBehaviour
    {
        public PlasmaballDriver PlasmaBall;

        public float InitialDelay = 6;
        public float Delay = 3;

        HashSet<AvatarDriver> _members = new HashSet<AvatarDriver>();


        void Awake()
        {
            foreach (var elm in GetComponentsInChildren<AvatarDriver>()) _members.Add(elm);
        }


        IEnumerator Start()
        {
            yield return new WaitForSeconds(InitialDelay);

            foreach (var elm in _members)
            {
                PlasmaBall.SubscribeAvatar(elm);
                yield return new WaitForSeconds(Delay);
            }
        }


    }
}