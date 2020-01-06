using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using WiimoteApi;

public class C_ScrappedMain : MonoBehaviour
{
    C_Wiimote wiimoteController;

    bool wiimoteMode = false;

    Vector2 followingPosition;

    [SerializeField]
    float percentFollowFrame = .210f;
    [SerializeField]
    float distanceToMinFollow = 1;
    [SerializeField]
    float distanceToMaxFollow = 50;

    // Start is called before the first frame update
    void Start()
    {
        wiimoteController = GetComponent<C_Wiimote>();

        followingPosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            wiimoteMode = !wiimoteMode;
        }
        

        if (wiimoteMode && WiimoteManager.HasWiimote())
        {
            Vector2 vals = wiimoteController.GetIRValues();
            float distancePoints = Vector2.Distance(followingPosition, vals);

            float newPercentFollow;

            if (distancePoints < distanceToMinFollow)
            {
                newPercentFollow = .01f;
            }
            else if (distancePoints > distanceToMaxFollow)
            {
                newPercentFollow = .5f;
            }
            else
            {
                newPercentFollow = percentFollowFrame;
            }

            float x = followingPosition.x * (1 - newPercentFollow) + vals.x * newPercentFollow;
            float y = followingPosition.y * (1 - newPercentFollow) + vals.y * newPercentFollow;

            followingPosition = new Vector2(x, y);

            MouseOperations.SetCursorPosition((int)followingPosition.x * 2, (Screen.height - (int)followingPosition.y)*2);

            if (wiimoteController.isBDown)
            {
                MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
            }
        }
        else
        {
            followingPosition = Input.mousePosition;
        }

    }


}

public class MouseOperations
{
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010
    }

    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out MousePoint lpMousePoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    public static void SetCursorPosition(int x, int y)
    {
        SetCursorPos(x, y);
    }

    public static void SetCursorPosition(MousePoint point)
    {
        SetCursorPos(point.X, point.Y);
    }

    public static MousePoint GetCursorPosition()
    {
        MousePoint currentMousePoint;
        var gotPoint = GetCursorPos(out currentMousePoint);
        if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
        return currentMousePoint;
    }

    public static void MouseEvent(MouseEventFlags value)
    {
        MousePoint position = GetCursorPosition();

        mouse_event
            ((int)value,
             position.X,
             position.Y,
             0,
             0)
            ;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
