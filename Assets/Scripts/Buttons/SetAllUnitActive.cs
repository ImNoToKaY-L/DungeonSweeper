using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetAllUnitActive : MonoBehaviour
{

    private bool setActive = true;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnClick()
    {
        Dictionary<Vector3, GameObject> UnitMap = GameManager.instance.boardScript.UnitMap;
        foreach (var item in UnitMap)
        {
            if (item.Value.tag!="Enemy")
            {
                item.Value.SetActive(setActive);
            }
        }

        if (setActive)
            Debug.Log("All units are now revealed");
        else
            Debug.Log("All unrevealed units are hidden");

        

        setActive = !setActive;
    }
}
