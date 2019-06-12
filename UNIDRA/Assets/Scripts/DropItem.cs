using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    public AudioClip itemSeClip;

    public enum ItemKind
    {
        Attack,
        Heal,
    };
    public ItemKind kind;

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            CharacterStatus aStatus = other.GetComponent<CharacterStatus>();
            aStatus.GetItem(kind);
            Destroy(gameObject);

            Vector3 cameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            AudioSource.PlayClipAtPoint(itemSeClip, cameraPos, 1f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 velocity = Random.insideUnitSphere * 2.0f + Vector3.up * 8.0f;
        GetComponent<Rigidbody>().velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
