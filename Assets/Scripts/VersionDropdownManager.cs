using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionDropdownManager : MonoBehaviour
{

    public Dropdown dropdown;
    public RectTransform arrowTransform;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(dropdown.transform.childCount > 3)
        {
            arrowTransform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(arrowTransform.eulerAngles.z, 90f, 720 * Time.deltaTime));
        }
        else
        {
            arrowTransform.eulerAngles = new Vector3(0, 0, Mathf.MoveTowardsAngle(arrowTransform.eulerAngles.z, -89.99f, 720 * Time.deltaTime));
        }
    }
    

}
