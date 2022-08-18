using SDL2;
using SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers;

public class RenderManager
{
    //System objects
    private static RenderManager _renderManager;

    //Game objects
    private readonly List<Building> _buildings;
    private readonly List<Buttons> _buttonsList;
    private readonly List<Unit> _units;
    private readonly RenderHudWorker RenderHudWorker = new();
    private readonly RenderMapWorker RenderMapWorker = new();
    private readonly RenderMiniMapWorker RenderMiniMapWorker = new();
    private readonly RenderObjectsWorker RenderObjectsWorker = new();
    private readonly RenderTextWorker RenderTextWorker = new();
    private readonly RenderMatchMenuHudWorker RenderMatchMenuHudWorker = new();
    private Camera _camera;
    private Hud _hud;
    private Level _level;
    private Level _level2;
    private IntPtr _renderer;
    private TextInput _textInput;
    private Texture _textureManager;
    private Dictionary<int, Tile> _tileList;
    private string _time = "";
    private IntPtr monserat = SDL_ttf.TTF_OpenFont("./Data/Fonts/OpenSans.ttf", 25);


    private RenderManager(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList, ref List<Unit> playersUnits, ref TextInput textInput)
    {
        _renderer = renderer;
        _buttonsList = buttonsList;
        _buildings = buildings;
        _textureManager = textureManager;
        _camera = camera;
        _level = level;
        _level2 = level.GetCopy();
        _hud = hud;
        _tileList = tileList;
        _units = playersUnits;
        _textInput = textInput;
    }

    public static RenderManager GetInstance(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList, ref List<Unit> playersUnits, ref TextInput textInput)
    {
        if (_renderManager != null) return _renderManager;
        return _renderManager =
            new RenderManager(ref renderer, ref buildings, ref textureManager, ref camera, ref level, ref buttonsList,
                ref hud, ref tileList, ref playersUnits, ref textInput);
    }

    /// <summary>
    ///     Перегрузка только для меню игры.
    /// </summary>
    /// <param name="matchState"></param>
    public void RunManager()
    {
        Render();
    }

    /// <summary>
    ///     Запуск работы цикла отрисовки для игрового поля
    /// </summary>
    /// <param name="currentBuilding"></param>
    /// <param name="matchState"></param>
    /// <param name="time"></param>
    /// <param name="player"></param>
    public void RunManager(ref Building? currentBuilding, ref int time, ref Player player)
    {
        Render(ref currentBuilding, ref time, ref player);
    }


    /// <summary>
    ///     Перегрузка только для меню игры.
    /// </summary>
    /// <param name="matchState"></param>
    private void Render()
    {
        SDL_RenderClear(_renderer);
        RenderHudWorker.RunWorker(_buttonsList.Where(b =>
                b.ButtonAttribute.ButtonType == ButtonType.MenuButton &&
                b.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled).ToList(), ref _renderer,
            ref _textureManager,
            ref _hud, ref _textInput, ref monserat);
        SDL_RenderPresent(_renderer);
    }

    /// <summary>
    ///     Перегрузка для отрисовки только игрового поля.
    /// </summary>
    /// <param name="currentBuilding"></param>
    /// <param name="matchState"></param>
    /// <param name="time"></param>
    /// <param name="player"></param>
    private void Render(ref Building? currentBuilding, ref int time, ref Player player)
    {
        SDL_RenderClear(_renderer);
        RenderMapWorker.RunWorker(
            currentBuilding != null &&
            currentBuilding.BuildingAttributes.BuildingPlaceState == BuildingPlaceState.NonPlaced, ref _level,
            ref _camera, ref _renderer);
        RenderObjectsWorker.RunWorker(_buildings.ToList(), _units.ToList(), ref _camera, ref _textureManager,
            ref _renderer, ref currentBuilding);
        RenderMiniMapWorker.RunWorker(_buildings, _units, ref _camera, ref _level2, ref _level, ref _renderer,
            ref _textureManager);
        RenderMatchMenuHudWorker.RunWorker(ref _renderer, ref _textureManager, ref currentBuilding, ref monserat);
        RenderHudWorker.RunWorker(_buttonsList.Where(b =>
                b.ButtonAttribute.ButtonType == ButtonType.MatchBuildingButton ||
                (b.ButtonAttribute.ButtonType == ButtonType.Blank &&
                 b.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled) ||
                (b.ButtonAttribute.ButtonType == ButtonType.MatchHudButton &&
                 b.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled)).ToList(), ref _renderer,
            ref _textureManager,
            ref _hud, ref _textInput, ref monserat);
        RenderTextWorker.RunWorker(ref player, ref _renderer, ref time);
        SDL_RenderPresent(_renderer);
    }
}