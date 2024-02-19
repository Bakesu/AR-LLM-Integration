using Microsoft.MixedReality.GraphicsTools;
using MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDisabler : MonoBehaviour
{
    public void DisableButton()
    {
        this.gameObject.SetActive(false);
        //this.gameObject.GetComponent<PressableButton>().enabled = false;
        //this.gameObject.transform.GetChild(0).GetComponent<RawImage>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void EnableButton()
    {
        this.gameObject.SetActive(true);
        //this.gameObject.GetComponent<PressableButton>().enabled = true;
        //this.gameObject.transform.GetChild(0).GetComponent<RawImage>().color = new Color(6f, 101f, 183f, 1f);
    }

}
