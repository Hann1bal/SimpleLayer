using SDL2;
using SimpleLayer.GameEngine.Managers.Workers;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using SimpleLayer.Objects.States;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class RenderManager
{
    private static RenderManager _renderManager;
    private readonly IntPtr monserat = SDL_ttf.TTF_OpenFont("./Data/Fonts/OpenSans.ttf", 10);
    private readonly List<Building> _buildings;

    private readonly List<Buttons> _buttonsList;
    private Camera _camera;
    private readonly SDL_Color _color = new() {a = 0, r = 255, b = 0, g = 0};

    // public Building _currentBuilding { get; set; }
    private readonly Hud _hud;
    private Level _level;
    private Level _level2;
    private IntPtr _renderer;
    private Texture _textureManager;
    private Dictionary<int, Tile> _tileList;
    private string _time = "";
    private readonly List<Unit> _units;
    private int _x, _y;
    private SDL_Rect mes;
    private SDL_Rect mesh;
    private readonly RenderMapWorker RenderMapWorker = new();
    private readonly RenderMiniMapWorker RenderMiniMapWorker = new();
    private readonly RenderObjectsWorker RenderObjectsWorker = new();

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

    public void RunManager(ref bool matchState)
    {
        Render(ref matchState);
    }

    public void RunManager(ref Building currentBuilding, ref bool matchState, ref int time, ref Player player)
    {
        Render(currentBuilding, ref matchState, ref time, ref player);
    }


    private void RenderMesh()
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
        {
            SDL_RenderDrawLine(_renderer, i * 320 - _camera.CameraRect.x, j * 320 - _camera.CameraRect.y,
                i * 320 - 320 - _camera.CameraRect.x, j * 320 - _camera.CameraRect.y);
            SDL_RenderDrawLine(_renderer, i * 320 - _camera.CameraRect.x, j * 320 - _camera.CameraRect.y,
                i * 320 - _camera.CameraRect.x, j * 320 + 320 - _camera.CameraRect.y);
            SDL_RenderDrawLine(_renderer, i * 320 + 320 - _camera.CameraRect.x, j * 320 + _camera.CameraRect.y,
                i * 320 + 320 - _camera.CameraRect.x, j * 320 + 320 - _camera.CameraRect.y);
            SDL_RenderDrawLine(_renderer, i * 320, j * 320 + 320 - _camera.CameraRect.y,
                i * 320 + 320 - _camera.CameraRect.x, j * 320 + 320 - _camera.CameraRect.y);
        }
    }


    private void RenderHud()
    {
        SDL_RenderCopy(_renderer, _textureManager.Dictionary[_hud.hudBaseObjectAttribute.CurrentTextureName],
            ref _hud.SRect,
            ref _hud.DRect);
    }

    private void RenderButtons(ref bool matchState, List<Buttons> buttons)
    {
        foreach (var button in buttons)
            switch (matchState)
            {
                case true when button.hudBaseObjectAttribute.TextureName == "playTextButton":
                case false when button.hudBaseObjectAttribute.TextureName == "resumeTextButton":
                    continue;
                default:
                    if (button.hudBaseObjectAttribute.TextureName == "playTextButton" &&
                        button.ButtonAttribute.ButtonState == ButtonState.Focused)
                        Console.WriteLine(button.hudBaseObjectAttribute.CurrentTextureName);
                    SDL_RenderCopy(_renderer,
                        _textureManager.Dictionary[button.hudBaseObjectAttribute.CurrentTextureName], ref button.SRect,
                        ref button.DRect);
                    break;
            }
    }

    private void RenderUnitInfo(ref Player player)
    {
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{player.PlayerAttribute.Gold},{player.PlayerAttribute.Nickname}",
            _color);
        var textureWreed = SDL_CreateTextureFromSurface(_renderer, message);
        mesh.x = 100; //controls the rect's x coorinate 
        mesh.y = 10; // controls the rect's y coordinte
        mesh.w = 300; // controls the width of the rect
        mesh.h = 50; // controls the height of the rect
        SDL_RenderCopy(_renderer, textureWreed, IntPtr.Zero, ref mesh);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }

    private void RenderTime(ref int time)
    {
        var minutes = time / 60;
        var seconds = time - minutes * 60;
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{minutes}:{seconds}", _color);
        var textureWreed = SDL_CreateTextureFromSurface(_renderer, message);
        mes.x = 950; //controls the rect's x coorinate 
        mes.y = 10; // controls the rect's y coordinte
        mes.w = 100; // controls the width of the rect
        mes.h = 100; // controls the height of the rect

        SDL_RenderCopy(_renderer, textureWreed, IntPtr.Zero, ref mes);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }

    private void RenderSelectedObject(GameBaseObject selectedObject)
    {
        SDL_GetMouseState(out _x, out _y);
        SDL_Rect newRectangle = new()
        {
            h = 90,
            w = 90,
            x = _x,
            y = _y
        };
        SDL_RenderCopy(_renderer, _textureManager.Dictionary[selectedObject.BaseObjectAttribute.TextureName],
            ref selectedObject.SRect,
            ref newRectangle);
    }

    /// <summary>
    ///     Перегрузка только для меню игры.
    /// </summary>
    /// <param name="matchState"></param>
    private void Render(ref bool matchState)
    {
        SDL_RenderClear(_renderer);
        RenderHud();
        RenderButtons(ref matchState,
            _buttonsList.Where(d => d.ButtonAttribute.ButtonType == ButtonType.MenuButton).ToList());
        SDL_RenderPresent(_renderer);
    }

    /// <summary>
    ///     Перегрузка только для матча.
    /// </summary>
    /// <param name="matchState"></param>
    private void Render(Building currentBuilding, ref bool matchState, ref int time, ref Player player)
    {
        SDL_RenderClear(_renderer);
        RenderMapWorker.RunWorker(currentBuilding != null, ref _level, ref _camera, ref _renderer);
        foreach (var building in _buildings.ToArray())
            RenderObjectsWorker.RunWorker(building, ref _camera, ref _textureManager, ref _renderer);
        foreach (var unit in _units.ToArray())
            RenderObjectsWorker.RunWorker(unit, ref _camera, ref _textureManager, ref _renderer);
        RenderMiniMapWorker.RunWorker(_buildings, _units, ref _camera, ref _level2, ref _level, ref _renderer,
            ref _textureManager);
        RenderHud();
        RenderButtons(ref matchState,
            _buttonsList.Where(b => b.ButtonAttribute.ButtonType == ButtonType.MatchHudButton).ToList());
        RenderButtons(ref matchState,
            _buttonsList.Where(b => b.ButtonAttribute.ButtonType == ButtonType.Blank).ToList());
        if (currentBuilding != null) RenderSelectedObject(currentBuilding);
        RenderTime(ref time);
        RenderUnitInfo(ref player);
        SDL_RenderPresent(_renderer);
    }
}