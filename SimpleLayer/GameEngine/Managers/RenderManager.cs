using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using SimpleLayer.Objects.States;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class RenderManager
{
    private static RenderManager _renderManager;
    private readonly List<Building> _buildings;

    private readonly List<Buttons> _buttonsList;
    private readonly Camera _camera;

    // public Building _currentBuilding { get; set; }
    private readonly Hud _hud;
    private readonly Level _level;
    private readonly Level _level2;
    private readonly IntPtr _renderer;
    private readonly Texture _textureManager;
    private readonly Dictionary<int, Tile> _tileList;
    private readonly List<Unit> _units;
    private int _x, _y;
    private IntPtr monserat = SDL_ttf.TTF_OpenFont($"./Data/Fonts/OpenSans.ttf", 10);
    private SDL_Color _color = new SDL_Color() {a = 0, r = 255, b = 0, g = 0};
    private string _time = "";
    private SDL_Rect mes = new SDL_Rect();
    private SDL_Rect mesh = new SDL_Rect();

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

    public void RunManager(ref Building currentBuilding, ref bool matchState, ref int time)
    {
        Render(currentBuilding, ref matchState, ref time);
    }

    private void DrawMap(bool flag)
    {
        for (var x = 0; x < Level.LevelWidth / 32; x++)
        for (var y = 0; y < Level.LevelHeight / 32; y++)
        {
            if (flag)
                switch (_level._tileLevel[new Vector2(x, y)].isPlacibleTile)
                {
                    case true:
                        switch (_level._tileLevel[new Vector2(x, y)].ContainBuilding)
                        {
                            case false:
                                SDL_SetTextureColorMod(_level._tileLevel[new Vector2(x, y)]._texture, 0, 100, 0);
                                break;
                            default:
                                SDL_SetTextureColorMod(_level._tileLevel[new Vector2(x, y)]._texture, 100, 0, 0);
                                break;
                        }

                        break;
                    default:
                        SDL_SetTextureColorMod(_level._tileLevel[new Vector2(x, y)]._texture, 100, 0, 0);
                        break;
                }
            else
                SDL_SetTextureColorMod(_level._tileLevel[new Vector2(x, y)]._texture, 255, 255, 255);

            var tmpDRect = new SDL_Rect
            {
                h = _level._tileLevel[new Vector2(x, y)]._sdlDRect.h,
                w = _level._tileLevel[new Vector2(x, y)]._sdlDRect.w,
                x = _level._tileLevel[new Vector2(x, y)]._sdlDRect.x - _camera.CameraRect.x,
                y = _level._tileLevel[new Vector2(x, y)]._sdlDRect.y - _camera.CameraRect.y
            };
            // SDL_SetRenderTarget(_renderer, _level._tileLevel[new Vector2(x, y)]._texture);
            SDL_RenderCopy(_renderer, _level._tileLevel[new Vector2(x, y)]._texture,
                ref _level._tileLevel[new Vector2(x, y)]._sdlSRect,
                ref tmpDRect);
        }
    }


    private void RenderMinimap(List<Building> buildings, Camera camera)
    {
        var newCameraSRect = new SDL_Rect
        {
            h = 90, w = 192,
            x = 22 + camera.CameraRect.x / 10,
            y = 805 + camera.CameraRect.y / 12
        };
        for (var x = 0; x < Level.LevelWidth / 32; x++)
        for (var y = 0; y < Level.LevelHeight / 32; y++)
        {
            _level2.DRect.x = 22 + x * 32 / 10;
            _level2.DRect.y = 805 + y * 32 / 12;
            _level2.DRect.h = _level2.SRect.h / 8;
            _level2.DRect.w = _level2.SRect.w / 8;
            SDL_RenderCopy(_renderer, _level._tileLevel[new Vector2(x, y)]._texture, ref _level2.SRect,
                ref _level2.DRect);
        }

        SDL_SetRenderDrawColor(_renderer, 0, 255, 0, 255);
        SDL_RenderDrawRect(_renderer, ref newCameraSRect);
        foreach (var build in buildings) RenderSingleIdleObjects(build);
        foreach (var unit in _units.ToArray()) RenderSingleIdleObjects(unit);
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

    private void RenderSingleObjects(Building building)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = building.SRect.w / 5,
            w = building.SRect.w / 5,
            x = (int) Math.Round(building.BaseObjectAttribute.XPosition) - _camera.CameraRect.x,
            y = (int) Math.Round(building.BaseObjectAttribute.YPosition) - _camera.CameraRect.y
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + _camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + _camera.CameraRect.h)
            return;

        texture = _textureManager.Dictionary[building.BaseObjectAttribute.TextureName];
        SDL_RenderCopy(_renderer, texture, ref building.SRect, ref newRectangle);
    }

    private void RenderSingleObjects(Unit unit)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = unit.SRect.w / 5,
            w = unit.SRect.w / 5,
            x = (int) Math.Round(unit.BaseObjectAttribute.XPosition) - _camera.CameraRect.x,
            y = (int) Math.Round(unit.BaseObjectAttribute.YPosition) - _camera.CameraRect.y
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + _camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + _camera.CameraRect.h)
            return;

        texture = unit.UnitsAttributes.MoAState switch
        {
            MoAState.Moving => unit.UnitsAttributes.DeltaX switch
            {
                > 0f => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"],
                < 0f => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_left_{unit.UnitsAttributes.CurrentMovingFrame}"],
                _ => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
            },
            MoAState.Attacking => unit.UnitsAttributes.DeltaX switch
            {
                > 0f => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_atack_{unit.UnitsAttributes.CurrentAttackFrame}"],
                < 0f => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_left_atack_{unit.UnitsAttributes.CurrentAttackFrame}"],
                _ => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_atack_{unit.UnitsAttributes.CurrentAttackFrame}"]
            },
            _ => _textureManager.Dictionary[
                $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
        };

        SDL_RenderCopy(_renderer, texture, ref unit.SRect,
            ref newRectangle);
    }

    private void RenderSingleIdleObjects(GameBaseObject gameBaseObject)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = gameBaseObject.SRect.w / 50,
            w = gameBaseObject.SRect.w / 50,
            x = 25 + (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition) / 10,
            y = 805 + (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition) / 12
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + _camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + _camera.CameraRect.h)
            return;

        texture = gameBaseObject.BaseObjectAttribute.ObjectType switch
        {
            ObjectType.Building => _textureManager.Dictionary[gameBaseObject.BaseObjectAttribute.TextureName],
            ObjectType.Unit when gameBaseObject is Unit unit => unit.UnitsAttributes.DeltaX switch
            {
                > 0 => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"],
                < 0 => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_left_{unit.UnitsAttributes.CurrentMovingFrame}"],
                _ => _textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
            }
        };


        SDL_RenderCopy(_renderer, texture, ref gameBaseObject.SRect,
            ref newRectangle);
    }

    private void RenderHud()
    {
        SDL_RenderCopy(_renderer, _textureManager.Dictionary[_hud.TextureName], ref _hud.SRect,
            ref _hud.DRect);
    }

    private void RenderButtons(ref bool matchState, List<Buttons> buttons)
    {
        foreach (var button in buttons)
            switch (matchState)
            {
                case true when button.TextureName == "playTextButton":
                case false when button.TextureName == "resumeTextButton":
                    continue;
                default:
                    SDL_RenderCopy(_renderer, _textureManager.Dictionary[button.CurrentTextureName], ref button.SRect,
                        ref button.DRect);
                    break;
            }
    }

    private void RenderUnitInfo(Unit unit, int x, int y)
    {
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{unit.UnitsAttributes.Accelaration}, {unit.UnitsAttributes.DeltaX}, {unit.UnitsAttributes.DeltaY}, {unit.BaseObjectAttribute.XPosition}, {unit.BaseObjectAttribute.YPosition},  {unit.UnitsAttributes.TargetDistance}",
            _color);
        var textureWreed = SDL_CreateTextureFromSurface(_renderer, message);
        mesh.x = x; //controls the rect's x coorinate 
        mesh.y = y; // controls the rect's y coordinte
        mesh.w = 300; // controls the width of the rect
        mesh.h = 50; // controls the height of the rect
        SDL_RenderCopy(_renderer, textureWreed, IntPtr.Zero, ref mesh);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }

    private void RenderTime(ref int time)
    {
        var minutes = time / 60;
        var seconds = time - (minutes * 60);
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

    private void Render(ref bool matchState)
    {
        SDL_RenderClear(_renderer);
        RenderHud();
        RenderButtons(ref matchState, _buttonsList.Where(d => !d.IsGameObject && d.IsMenuObject).ToList());
        SDL_RenderPresent(_renderer);
    }

    private void Render(Building currentBuilding, ref bool matchState, ref int time)
    {
        SDL_RenderClear(_renderer);
        DrawMap(currentBuilding != null);
        foreach (var building in _buildings.ToArray()) RenderSingleObjects(building);
        foreach (var unit in _units.ToArray()) RenderSingleObjects(unit);
        var cnt = 1;
        foreach (var unit in _units.ToArray())
        {
            RenderUnitInfo(unit, 50, 100 * cnt);
            cnt++;
        }

        RenderMinimap(_buildings, _camera);
        RenderHud();
        RenderButtons(ref matchState, _buttonsList.Where(b => b.IsGameObject && !b.IsMenuObject).ToList());
        RenderButtons(ref matchState, _buttonsList.Where(b => !b.IsGameObject && !b.IsMenuObject).ToList());
        if (currentBuilding != null) RenderSelectedObject(currentBuilding);
        RenderTime(ref time);
        SDL_RenderPresent(_renderer);
    }
}