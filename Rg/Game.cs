using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;

public class Game
{
    private static int playerX = 25;
    private static int playerY = 25;
    private static RLRootConsole rootConsole;

    public static void Main()
    {
        RLSettings settings = new RLSettings();
        settings.BitmapFile = "ascii_8x8.png";
        settings.CharWidth = 8;
        settings.CharHeight = 8;
        settings.Width = 60;
        settings.Height = 40;
        settings.Scale = 1f;
        settings.Title = "RLNET Test";
        settings.WindowBorder = RLWindowBorder.Resizable;
        settings.ResizeType = RLResizeType.ResizeCells;
        settings.StartWindowState = RLWindowState.Normal;

        rootConsole = new RLRootConsole(settings);
        rootConsole.Update += rootConsole_Update;
        rootConsole.Render += rootConsole_Render;
        rootConsole.OnLoad += rootConsole_OnLoad;
        rootConsole.Run();

    }

    static void rootConsole_OnLoad(object sender, EventArgs e)
    {
        //rootConsole.SetWindowState(RLWindowState.Fullscreen);
    }

    static void rootConsole_Update(object sender, UpdateEventArgs e)
    {

        RLKeyPress keyPress = rootConsole.Keyboard.GetKeyPress();
        if (keyPress != null)
        {
            if (keyPress.Key == RLKey.Up)
                playerY--;
            else if (keyPress.Key == RLKey.Down)
                playerY++;
            else if (keyPress.Key == RLKey.Left)
                playerX--;
            else if (keyPress.Key == RLKey.Right)
                playerX++;
            if (keyPress.Key == RLKey.Escape)
                rootConsole.Close();
        }
    }

    static void rootConsole_Render(object sender, UpdateEventArgs e)
    {
        rootConsole.Clear();


        rootConsole.SetChar(playerX, playerY, '@');

        int color = 1;
        if (rootConsole.Mouse.LeftPressed)
        {
            color = 4;
        }
        rootConsole.SetBackColor(rootConsole.Mouse.X, rootConsole.Mouse.Y, RLColor.CGA[color]);

        rootConsole.Draw();
    }
}