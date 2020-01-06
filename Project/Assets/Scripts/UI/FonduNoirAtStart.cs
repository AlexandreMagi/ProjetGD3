using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FonduNoirAtStart : MonoBehaviour
{
    float fCurrentAlpha = 1;

    // Update is called once per frame
    void Update()
    {
        fCurrentAlpha -= Time.deltaTime / 1;
        if (fCurrentAlpha < 0)
        {
            fCurrentAlpha = 0;
            this.enabled = false;
            //Destroy(this.gameObject);
        }
        GetComponent<Image>().color = new Color(0, 0, 0, fCurrentAlpha);
    }
}
