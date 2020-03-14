using UnityEngine;

public class ShowHint : MonoBehaviour
{
    public GameObject hint;

	void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player") 
            && hint.gameObject != null
            && !hint.gameObject.activeSelf)
        {
            hint.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") 
            && hint.gameObject != null 
            && hint.gameObject.activeSelf)
        {
            hint.SetActive(false);
        }
    }
}
