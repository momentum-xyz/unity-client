using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestructEffect : MonoBehaviour
{
    private ParticleSystem[] particles;
    private AudioSource[] audioSource;
    private Animator[] animators;

    [SerializeField] [Range(0, 3)] private float PollingDuration = .5f;

    // OnEnable
    void OnEnable()
    {
        particles = GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponentsInChildren<AudioSource>();
        animators = GetComponentsInChildren<Animator>();
        StartCoroutine(CheckIfComponentsAreFinished());
    }

    private IEnumerator CheckIfComponentsAreFinished()
    {
        while (true)
        {
            yield return new WaitForSeconds(PollingDuration);

            // bool breakWhileLoop = false;
            bool stillAlive = false;
 
            foreach( var prt in particles )
                stillAlive |= prt.IsAlive();
            foreach( var au in audioSource )
                stillAlive |= au.isPlaying;
            foreach( var anm in animators )
                stillAlive |= anm.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.99f;

            if( !stillAlive ) break;

            // if (particles.Length > 0)
            //     for (int i = 0; i < particles.Length; i++)
            //     {
            //         // Debug.Log(particles[i].isEmitting);
            //         if (!particles[i].IsAlive() )
            //             breakWhileLoop = true;
            //         else
            //             breakWhileLoop = false;
            //     }

            // if (audioSource.Length > 0 && breakWhileLoop)
            //     for (int i = 0; i < audioSource.Length; i++)
            //     {
            //         if (!audioSource[i].isPlaying)
            //             breakWhileLoop = true;
            //         else
            //             breakWhileLoop = false;
            //     }

            // if (breakWhileLoop)
            //     break;
        }

        HS.Pool.Instance.Return(gameObject);
    }
}