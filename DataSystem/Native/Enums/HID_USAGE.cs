using System;

namespace CMDR.Native
{
    internal enum HID_USAGE_PAGE : uint
    {
        GENERIC = 0x01,
        GAME = 0X05,
        LED = 0X08,
        BUTTON = 0X09
    }

    internal enum HID_USAGE_GENERIC
    {
        POINTER = 0X01,
        MOUSE = 0X02,
        JOYSTICK = 0X04,
        GAMEPAD = 0X05,
        KEYBOARD = 0X06,
        KEYPAD = 0X07,
        MULTI_AXIS_CONTROLLER = 0X08
    }
}