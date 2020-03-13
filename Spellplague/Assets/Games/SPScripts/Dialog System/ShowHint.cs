using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHint : MonoBehaviour
{
    public GameObject hint;

	//private AudioSource source;

	void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player") && hint.gameObject != null && hint.gameObject.activeSelf == false)
        {
            hint.SetActive(true);

            //if (this.gameObject.GetComponent<AudioSource>() != null)
            //source = GetComponent<AudioSource>();
            //source.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && hint.gameObject != null && hint.gameObject.activeSelf == true)
        {
            hint.SetActive(false);

            //if (this.gameObject.GetComponent<AudioSource>() != null)
            //source = GetComponent<AudioSource>();
            //source.Play();
        }
    }
}
