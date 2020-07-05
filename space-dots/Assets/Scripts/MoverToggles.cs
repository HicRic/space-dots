using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoverToggles : MonoBehaviour
{
    [SerializeField]
    private Toggle barToggle;

    [SerializeField]
    private GameObject barObj;

    [SerializeField]
    private Toggle planetToggle;

    [SerializeField]
    private GameObject planetObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (barToggle.isOn != barObj.gameObject.activeSelf)
        {
            barObj.gameObject.SetActive(barToggle.isOn);
        }

        if (planetToggle.isOn != planetObj.gameObject.activeSelf)
        {
            planetObj.gameObject.SetActive(planetToggle.isOn);
        }
    }
}
