using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFillerScript : MonoBehaviour
{
    public Sprite[] particlesSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpriteWriting()
    {
        ParticleSystem p = GetComponent<ParticleSystem>();
        var m = p.textureSheetAnimation;
        for (int i = m.spriteCount-1; i >= 0; i--)
        {
            m.RemoveSprite(i);
        }
        for (int i = 0; i < particlesSprite.Length; i++)
        {
            m.AddSprite(particlesSprite[i]);
            //m.SetSprite(i, particlesSprite[i]);

        }
    }
}
