using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using Rg.Core;

public static class Game
{


    //public 

    public static Player Player { get; private set; }
    public static DungeonMap DungeonMap { get; private set; }
    public static CommandSystem CommandSystem { get; private set; }

    private static bool _renderRequired = true;


    //private 


    // The screen height and width are in number of tiles
    private static readonly int _screenWidth = 100;
    private static readonly int _screenHeight = 70;
    private static RLRootConsole _rootConsole;

    // The map console takes up most of the screen and is where the map will be drawn
    private static readonly int _mapWidth = 80;
    private static readonly int _mapHeight = 48;
    private static RLConsole _mapConsole;

    // Below the map console is the message console which displays attack rolls and other information
    private static readonly int _messageWidth = 80;
    private static readonly int _messageHeight = 11;
    private static RLConsole _messageConsole;

    // The stat console is to the right of the map and display player and monster stats
    private static readonly int _statWidth = 20;
    private static readonly int _statHeight = 70;
    private static RLConsole _statConsole;

    // Above the map is the inventory console which shows the players equipment, abilities, and items
    private static readonly int _inventoryWidth = 80;
    private static readonly int _inventoryHeight = 11;
    private static RLConsole _inventoryConsole;



    public static void Main()
    {
        CommandSystem = new CommandSystem();
        Player = new Player();

        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight);
        DungeonMap = mapGenerator.CreateMap();

        DungeonMap.UpdatePlayerFieldOfView();

        RLSettings settings = new RLSettings();
        settings.BitmapFile = "ascii_8x8.png";
        settings.CharWidth = 8;
        settings.CharHeight = 8;
        settings.Width = _screenWidth;
        settings.Height = _screenHeight;
        settings.Scale = 1f;
        settings.ResizeType = RLResizeType.ResizeCells;
        settings.WindowBorder = RLWindowBorder.Fixed;
        settings.StartWindowState = RLWindowState.Normal;
        settings.Title = "RougeSharp V3 Tutorial - Level 1";

        _rootConsole = new RLRootConsole(settings);

        _mapConsole = new RLConsole(_mapWidth, _mapHeight);
        _messageConsole = new RLConsole(_messageWidth, _messageHeight);
        _statConsole = new RLConsole(_statWidth, _statHeight);
        _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

        _rootConsole.Update += OnUpdate;
        _rootConsole.Render += OnRender;
        _rootConsole.OnLoad += OnLoad;


        _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorFov);

        _messageConsole.SetBackColor(0, 0, _messageWidth, _messageHeight, Swatch.DbDeepWater);
        _messageConsole.Print(1, 1, "Messages", Colors.TextHeading);

        _statConsole.SetBackColor(0, 0, _statWidth, _statHeight, Swatch.DbOldStone);
        _statConsole.Print(1, 1, "Stats", Colors.TextHeading);

        _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
        _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

        _rootConsole.Run();
    }

    static void OnLoad(object sender, EventArgs e)
    {

        //_rootConsole.SetWindowState(RLWindowState.Fullscreen);
    }

    static void OnUpdate(object sender, UpdateEventArgs e)
    {


        bool didPlayerAct = false;
        RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

        if (keyPress != null)
        {
            if (keyPress.Key == RLKey.Up)
            {
                didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
            }
            else if (keyPress.Key == RLKey.Down)
            {
                didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
            }
            else if (keyPress.Key == RLKey.Left)
            {
                didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
            }
            else if (keyPress.Key == RLKey.Right)
            {
                didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
            }
            else if (keyPress.Key == RLKey.Escape)
            {
                _rootConsole.Close();
            }
        }

        if (didPlayerAct)
        {
            _renderRequired = true;
        }
    }

    static void OnRender(object sender, UpdateEventArgs e)
    {
        if (_renderRequired)
        {
            DungeonMap.Draw(_mapConsole);
            Player.Draw(_mapConsole, DungeonMap);
            //_rootConsole.Clear();

            // Blit the sub consoles to the root console in the correct locations

            RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight,
              _rootConsole, 0, _inventoryHeight);
            RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight,
              _rootConsole, _mapWidth, 0);
            RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight,
              _rootConsole, 0, _screenHeight - _messageHeight);
            RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight,
              _rootConsole, 0, 0);

            // Tell RLNET to draw the console that we set
            _rootConsole.Draw();

            _renderRequired = false;
        }
    }
}