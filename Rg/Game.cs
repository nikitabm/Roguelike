using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RLNET;
using Rg.Core;
using RogueSharp.Random;
using System.IO;
public static class Game
{

    //private

    // The screen height and width are in number of tiles
    private static readonly int _screenWidth = 100;
    private static readonly int _screenHeight = 70;
    private static RLRootConsole _rootConsole;

    // The map console takes up most of the screen and is where the map will be drawn
    private static readonly int _mapWidth = 80;
    private static readonly int _mapHeight = 50;
    private static RLConsole _mapConsole;

    // Below the map console is the message console which displays attack rolls and other information
    private static readonly int _messageWidth = 100;
    private static readonly int _messageHeight = 10;
    private static RLConsole _messageConsole;

    // The stat console is to the right of the map and display player and monster stats
    private static readonly int _statWidth = 20;
    private static readonly int _statHeight = 60;
    private static RLConsole _statConsole;

    // Above the map is the inventory console which shows the players equipment, abilities, and items
    private static readonly int _inventoryWidth = 80;
    private static readonly int _inventoryHeight = 10;
    private static RLConsole _inventoryConsole;

    private static readonly int _textScreenWidth = 100;
    private static readonly int _textScreenHeight = 70;
    private static RLConsole _textScreenConsole;

    //vars
    private static bool _renderRequired = true;
    private static string _path = "save.txt";
    private static int _mapLevel = 1;
    private static int _generation = 1;
    private static bool _running = true;
    private static bool _showTextConsole = false;
    //public 
    public static MessageLog MessageLog { get; private set; }
    public static MessageLog TextConsoleLog { get; private set; }
    public static IRandom Random { get; private set; }
    public static Player Player { get; set; }
    public static DungeonMap DungeonMap { get; private set; }
    public static CommandSystem CommandSystem { get; private set; }
    public static SchedulingSystem SchedulingSystem { get; private set; }

    public static int Level
    {
        get
        {
            return _mapLevel;
        }
    }
    public static int Generation
    {
        get
        {
            return _generation;
        }
        set
        {
            _generation = value;
        }
    }
    public static string Path
    {
        get
        {
            return _path;
        }
    }
    public static bool Running
    {
        get
        {
            return _running;
        }
        set
        {
            _running = value;
        }
    }
    public static void Main()
    {
        int seed = (int)DateTime.UtcNow.Ticks;
        Random = new DotNetRandom(seed);

        // The title will appear at the top of the console window
        // also include the seed used to generate the level
        string consoleTitle = "Game by Nikita Balabuiev";

        CommandSystem = new CommandSystem();
        SchedulingSystem = new SchedulingSystem();
        MessageLog = new MessageLog();
        TextConsoleLog = new MessageLog(100);

        //generate map
        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, _mapLevel);
        DungeonMap = mapGenerator.CreateMap();



        if (!File.Exists(_path))
        {
            var sw = File.CreateText(_path);
            sw.WriteLine(_generation.ToString());
            sw.Close();
        }
        if (File.Exists(_path))
        {
            StreamReader reader = new StreamReader(_path);
            _generation = int.Parse(reader.ReadLine());
            reader.Close();
        }
        MessageLog.Add("As the book finally touches floor it turns into... You.");
        MessageLog.Add("I hope you like name Lucy, because your name is Lucy. Lucy was one of the authors of this book-you");
        MessageLog.Add("The  servant of Lueltein got summoned. " + Player.Name + " " + _generation + " finds herself in the library storage.");

        DungeonMap.UpdatePlayerFieldOfView();

        RLSettings settings = new RLSettings();
        settings.BitmapFile = "ascii_8x8.png";
        settings.CharWidth = 8;
        settings.CharHeight = 8;
        settings.Width = _screenWidth;
        settings.Height = _screenHeight;
        settings.Scale = 1.9f;
        settings.WindowBorder = RLWindowBorder.Resizable;
        settings.Title = consoleTitle;

        _rootConsole = new RLRootConsole(settings);
        _rootConsole.SetWindowState(RLWindowState.Fullscreen);

        _mapConsole = new RLConsole(_mapWidth, _mapHeight);
        _messageConsole = new RLConsole(_messageWidth, _messageHeight);
        _statConsole = new RLConsole(_statWidth, _statHeight);
        _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);
        _textScreenConsole = new RLConsole(_textScreenWidth, _textScreenHeight);

        _textScreenConsole.SetBackColor(0, 0, Colors.Gold);


        _rootConsole.Update += OnUpdate;
        _rootConsole.Render += OnRender;
        _rootConsole.OnLoad += OnLoad;


        _mapConsole.SetBackColor(0, 0, _mapWidth, _mapHeight, Colors.FloorFov);



        _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
        _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);


        TextConsoleLog.Add($"Visible frustration on the Lueltein's face grew more and more frustrative.");
        TextConsoleLog.Add($"");

        TextConsoleLog.Add($"He've been conducting an important research lately (for the past 100 years).");
        TextConsoleLog.Add($"");

        TextConsoleLog.Add($"Walking along numberless bookcases he seems to be unable to find what he is looking for.");
        TextConsoleLog.Add($"");

        TextConsoleLog.Add($"-I have to get some books from the library's storage but I'm too lazy...");
        TextConsoleLog.Add($"");

        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"Randomly reaching out for a book, Lueltein picks one from the bookcase.");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"Mumbling some sort of spell Lueltein comes to the staircase leading to the storage rooms.");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"Go get books I am looking for.Your sight will only see those books so it should be easy.");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"I guess there are some creatures living there, but you are able to defend yourself Im sure.");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"With these words he throws the book into the darkness.");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"The Book had title: 'Forgotten art of being productive during corona quarantine 2020'");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"Maybe it was not the best book to get rid of....");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"");
        TextConsoleLog.Add($"");

        TextConsoleLog.Add("Press SPACE TO CONTINUE");

        _showTextConsole = true;

        _rootConsole.Run();
    }

    static void OnLoad(object sender, EventArgs e)
    {
    }
    public static void SetTextConsole(bool visible, string text = "")
    {
        TextConsoleLog.Clear();
        TextConsoleLog.Add(text);
        _showTextConsole = visible;
    }
    public static void End()
    {
        TextConsoleLog.Clear();
        TextConsoleLog.Add($"=========================================");
        TextConsoleLog.Add($"          {Player.Name} {_generation} died.             ");
        TextConsoleLog.Add($"=========================================");
        SaveData();
        _showTextConsole = true;
        _renderRequired = true;
    }
    public static void SaveData()
    {
        var sw = new StreamWriter(_path);
        _generation++;
        sw.WriteLine(_generation.ToString());
        sw.Close();
    }
    static void OnUpdate(object sender, UpdateEventArgs e)
    {
        RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();

        bool didPlayerAct = false;

        switch (CommandSystem.GameState)
        {
            case CommandSystem.EGameState.GameStart:
                if (keyPress == null) return;
                if (keyPress.Key == RLKey.Space)
                {
                    _showTextConsole = false;
                    CommandSystem.SetGameState(CommandSystem.EGameState.PlayerTurn);
                    _renderRequired = true;
                }
                break;

            case CommandSystem.EGameState.PlayerTurn:
                if (keyPress == null) return;
                if ((keyPress.Key == RLKey.W) || (keyPress.Key == RLKey.Up))
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.S || (keyPress.Key == RLKey.Down))
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.A || (keyPress.Key == RLKey.Left))
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.D || (keyPress.Key == RLKey.Right))
                {
                    didPlayerAct = CommandSystem.MovePlayer(Direction.Right);
                }
                else if (keyPress.Key == RLKey.Period)
                {
                    if (DungeonMap.CanMoveDownToNextLevel())
                    {
                        MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight, 20, 13, 7, ++_mapLevel);
                        DungeonMap = mapGenerator.CreateMap();
                        MessageLog = new MessageLog();
                        CommandSystem = new CommandSystem();
                        _rootConsole.Title = "Game by Nikita Balabuiev";
                        didPlayerAct = true;
                    }
                }
                else if (keyPress.Key == RLKey.Escape)
                {
                    SaveData();
                    _rootConsole.Close();
                }
                if (didPlayerAct)
                {
                    _renderRequired = true;
                    CommandSystem.SetGameState(CommandSystem.EGameState.EnemyTurn);
                }
                break;

            case CommandSystem.EGameState.EnemyTurn:
                CommandSystem.ActivateActors();
                _renderRequired = true;
                break;
            case CommandSystem.EGameState.GameEnd:
                if (keyPress == null) return;
                else if ((keyPress.Key == RLKey.Escape) || (keyPress.Key == RLKey.Space))
                {
                    _rootConsole.Close();
                }
                break;
        }
    }

    static void OnRender(object sender, UpdateEventArgs e)
    {
        if (_renderRequired)
        {
            _mapConsole.Clear();
            _statConsole.Clear();
            _messageConsole.Clear();
            _textScreenConsole.Clear();

            DungeonMap.Draw(_mapConsole, _statConsole);

            MessageLog.Draw(_messageConsole);
            Player.Draw(_mapConsole, DungeonMap);
            Player.DrawStats(_statConsole);


            // Blit the sub consoles to the root console in the correct locations
            RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _inventoryHeight);
            RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, _mapWidth, 0);
            RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, _mapHeight + _inventoryHeight);
            RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, 0, 0);

            if (_showTextConsole)
            {
                TextConsoleLog.Draw(_textScreenConsole);
                RLConsole.Blit(_textScreenConsole, 0, 0, _textScreenWidth, _textScreenHeight, _rootConsole, 0, 0);
            }
            // Tell RLNET to draw the console that we set
            _rootConsole.Draw();

            _renderRequired = false;
        }
    }
}