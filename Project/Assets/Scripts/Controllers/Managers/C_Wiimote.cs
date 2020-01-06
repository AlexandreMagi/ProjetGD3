using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteApi;

public class C_Wiimote : MonoBehaviour
{

    private Wiimote wiimote;

    public bool isBDown;
    public bool isB;
    public bool isADown;
    public bool isA;
    public bool isZDown;
    public bool isZ;

    float[] ir2;

    NunchuckData dataNunchuk;

    /// <summary>
    /// Tries to find an active Wiimote
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        WiimoteManager.FindWiimotes();
    }

    /// <summary>
    /// Gets the IR values of the current detected Wiimote
    /// </summary>
    void Update()
    {
        if (!WiimoteManager.HasWiimote()) { WiimoteManager.FindWiimotes(); return; }
        else
        {
            wiimote = WiimoteManager.Wiimotes[0];
            wiimote.SetupIRCamera(IRDataType.BASIC);
            wiimote.SendPlayerLED(true, false, false, false);

            dataNunchuk = wiimote.Nunchuck;
        }

        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();


        } while (ret > 0);

        ir2 = wiimote.Ir.GetPointingPosition();


        if (wiimote.Button.b)
        {
            if (isB)
            {
                isBDown = false;
            }
            else
            {
                isBDown = true;
            }
            isB = true;
        }
        else
        {
            isB = false;
            isBDown = false;
        }

        if (wiimote.Button.a)
        {
            if (isA)
            {
                isADown = false;
            }
            else
            {
                isADown = true;
            }
            isA = true;
        }
        else
        {
            isA = false;
            isADown = false;
        }

        if (dataNunchuk != null && dataNunchuk.z)
        {
            if (isZ)
            {
                isZDown = false;
            }
            else
            {
                isZDown = true;
            }
            isZ = true;
        }
        else
        {
            isZ = false;
            isZDown = false;
        }

    }

    /// <summary>
    /// Returns the IR values of the Wiimote
    /// </summary>
    /// <returns></returns>
    public Vector2 GetIRValues()
    {
        return new Vector2(ir2[0]*Screen.width*1.2f, ir2[1]*Screen.height*1.2f);
    }

    /// <summary>
    /// Forces to detect an active Wiimote
    /// </summary>
    void DetectWiimotes()
    {
        WiimoteManager.FindWiimotes();
    }

    /*
    void OnApplicationQuit()
    {
        if (wiimote != null)
        {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }
    }
    */
}
