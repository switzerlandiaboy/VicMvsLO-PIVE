using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StartingWall : MonoBehaviour
{
    [FormerlySerializedAs("START_DELAY")] public int startDelay;
        
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(WaitAndDestroy));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(startDelay);
        Destroy(gameObject);
        Instantiate(Resources.Load("Prefabs/Particle/Explosion"), transform.position + new Vector3(2, 3, 0),
            Quaternion.identity);
        GameManager.Instance.sfx.PlayOneShot(Enums.Sounds.Enemy_Bobomb_Explode.GetClip());
    }
}