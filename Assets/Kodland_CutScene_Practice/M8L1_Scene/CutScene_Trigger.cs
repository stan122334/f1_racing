using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CutScene_Trigger : MonoBehaviour
{
    [SerializeField] PlayableDirector cutScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cutScene.Play();
            Destroy(gameObject);
        }
    }
}
