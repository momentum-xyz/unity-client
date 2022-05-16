using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public float scrollSpeed = 0.4f;
    public Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed;
        rend.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: change to wisp tag
        if (other.name != "plasmaBallTotem")
        {
            Destroy(this.gameObject);
        }
    }

}
