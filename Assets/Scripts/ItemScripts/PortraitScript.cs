using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitScript : MonoBehaviour {

    public Material m_paintingPrimary;
    public Material m_paintingSecondary;
    private GameObject human;
    float timer = 0f;
    bool humanInProximity = false;
    bool imageChanged = false;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (humanInProximity)
        {
            //Player looking at mannequin
            if (!imageChanged && Vector3.Dot(human.transform.forward, (transform.position - human.transform.position).normalized) > 0.5f)
            {
                timer += Time.deltaTime;
            }
            //Not looking at mannequin
            else if (timer > 3 && !imageChanged)
            {
                imageChanged = true;
                GetComponent<MeshRenderer>().material = m_paintingSecondary;
            }
            if (imageChanged && Vector3.Dot(human.transform.forward, (transform.position - human.transform.position).normalized) > 0.6f)
            {
                StartCoroutine(ChangeImageBack());
            }
        }
    }
    IEnumerator ChangeImageBack()
    {
        yield return new WaitForSeconds(0.4f);
        GetComponent<MeshRenderer>().material = m_paintingPrimary;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<HumanController>() || other.gameObject.GetComponent<HumanVRController>())
        {
            human = other.gameObject;
            humanInProximity = true;
        }
    }
}
