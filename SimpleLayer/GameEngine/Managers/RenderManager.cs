using SimpleLayer.GameEngine.Managers.Workers;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.Hud;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
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
    private Camera _camera;
    private Hud _hud;
    private Level _level;
    private Level _level2;
    private IntPtr _renderer;
    private Texture _textureManager;
    private Dictionary<int, Tile> _tileList;
    private string _time = "";

    private RenderManager(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList, ref List<Unit> playersUnits)
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
    }

    public static RenderManager GetInstance(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList, ref List<Unit> playersUnits)
    {
        if (_renderManager != null) return _renderManager;
        return _renderManager =
            new RenderManager(ref renderer, ref buildings, ref textureManager, ref camera, ref level, ref buttonsList,
                ref hud, ref tileList, ref playersUnits);
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
    public void RunManager(ref Building currentBuilding, ref int time, ref Player player)
    {
        Render(currentBuilding, ref time, ref player);
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
            ref _hud);
        SDL_RenderPresent(_renderer);
    }

    /// <summary>
    ///     Перегрузка для отрисовки только игрового поля.
    /// </summary>
    /// <param name="currentBuilding"></param>
    /// <param name="matchState"></param>
    /// <param name="time"></param>
    /// <param name="player"></param>
    private void Render(Building currentBuilding, ref int time, ref Player player)
    {
        SDL_RenderClear(_renderer);
        RenderMapWorker.RunWorker(currentBuilding != null, ref _level, ref _camera, ref _renderer);
        RenderObjectsWorker.RunWorker(_buildings.ToList(), _units.ToList(), ref _camera, ref _textureManager,
            ref _renderer, ref currentBuilding);
        RenderMiniMapWorker.RunWorker(_buildings, _units, ref _camera, ref _level2, ref _level, ref _renderer,
            ref _textureManager);
        RenderHudWorker.RunWorker(_buttonsList.Where(b =>
                b.ButtonAttribute.ButtonType == ButtonType.MatchHudButton ||
                (b.ButtonAttribute.ButtonType == ButtonType.Blank &&
                 b.ButtonAttribute.EoDButtonState == EoDButtonState.Enabled)).ToList(), ref _renderer,
            ref _textureManager,
            ref _hud);
        RenderTextWorker.RunWorker(ref player, ref _renderer, ref time);
        SDL_RenderPresent(_renderer);
    }
}