﻿using System.Numerics;
using System.Runtime.InteropServices;
using SDL2;
using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine;

public class RenderManager
{
    private IntPtr _renderer;
    private Texture _textureManager;
    private List<Building> _buildings;
    private static RenderManager _renderManager;
    private readonly Camera _camera;
    private readonly Level _level;
    private readonly Level _level2;

    private List<Buttons> _buttonsList;

    // public Building _currentBuilding { get; set; }
    private Hud _hud;
    private int _x, _y;
    private readonly Dictionary<int, Tile> _tileList;

    private RenderManager(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList)
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
    }

    public static RenderManager GetInstance(ref IntPtr renderer, ref List<Building> buildings,
        ref Texture textureManager, ref Camera camera, ref Level level, ref List<Buttons> buttonsList, ref Hud hud,
        ref Dictionary<int, Tile> tileList)
    {
        if (_renderManager != null) return _renderManager;
        return _renderManager =
            new RenderManager(ref renderer, ref buildings, ref textureManager, ref camera, ref level, ref buttonsList,
                ref hud, ref tileList);
    }

    public void RunManager(ref bool matchState)
    {
        Render(ref matchState);
    }

    public void RunManager(ref Building currentBuilding, ref bool matchState)
    {
        Render(currentBuilding: currentBuilding, matchState: ref matchState);
    }


    private void DrawMap(Camera camera, bool flag)
    {
        for (var x = 0; x < Level.LevelWidth / 32; x++)
        {
            for (var y = 0; y < Level.LevelHeight / 32; y++)
            {
                if (flag)
                {
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
                }
                else
                {
                    SDL_SetTextureColorMod(_level._tileLevel[new Vector2(x, y)]._texture, 255, 255, 255);
                }

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
    }


    private void RenderMinimap(List<Building> buildings, Camera camera)
    {
        SDL_Rect newCameraSRect = new SDL_Rect()
        {
            h = 90, w = 192,
            x = 22 + camera.CameraRect.x / 10,
            y = 805 + camera.CameraRect.y / 12
        };
        for (var x = 0; x < Level.LevelWidth / 32; x++)
        {
            for (var y = 0; y < Level.LevelHeight / 32; y++)
            {
                _level2.DRect.x = 22 + x * 32 / 10;
                _level2.DRect.y = 805 + y * 32 / 12;
                _level2.DRect.h = _level2.SRect.h / 8;
                _level2.DRect.w = _level2.SRect.w / 8;
                SDL_RenderCopy(_renderer, _level._tileLevel[new Vector2(x, y)]._texture, ref _level2.SRect,
                    ref _level2.DRect);
            }
        }

        SDL_SetRenderDrawColor(_renderer, 0, 255, 0, 255);

        SDL_RenderDrawRect(_renderer, ref newCameraSRect);

        foreach (var build in buildings)
        {
            RenderSingleIdleObjects(build);
            foreach (var unit in build.Units)
            {
                RenderSingleIdleObjects(unit);
            }
        }
    }

    private void RenderMesh()
    {
        for (var i = 0; i < 10; i++)
        {
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
    }

    private void RenderSingleObjects(GameBaseObject gameBaseObject)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = gameBaseObject.SRect.w / 5,
            w = gameBaseObject.SRect.w / 5,
            x = gameBaseObject.XPosition - _camera.CameraRect.x,
            y = gameBaseObject.YPosition - _camera.CameraRect.y
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + _camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + _camera.CameraRect.h)
        {
            return;
        }


        if (!gameBaseObject.IsBuildng)
        {
            texture = gameBaseObject.CurrentXSpeed switch
            {
                > 0 => _textureManager.Dictionary[$"{gameBaseObject.TextureName}_right_{gameBaseObject.CurrentFrame}"],
                < 0 => _textureManager.Dictionary[$"{gameBaseObject.TextureName}_left_{gameBaseObject.CurrentFrame}"],
                _ => _textureManager.Dictionary[$"{gameBaseObject.TextureName}_right_{gameBaseObject.CurrentFrame}"]
            };
        }
        else
        {
            texture = _textureManager.Dictionary[gameBaseObject.TextureName];
        }

        SDL_RenderCopy(_renderer, texture, ref gameBaseObject.SRect,
            ref newRectangle);
    }


    private void RenderSingleIdleObjects(GameBaseObject gameBaseObject)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = gameBaseObject.SRect.w / 50,
            w = gameBaseObject.SRect.w / 50,
            x = 25 + gameBaseObject.XPosition / 10,
            y = 805 + gameBaseObject.YPosition / 12
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + _camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + _camera.CameraRect.h)
        {
            return;
        }

        if (gameBaseObject.IsBuildng)
        {
            texture = _textureManager.Dictionary[gameBaseObject.TextureName];
        }
        else
        {
            texture = gameBaseObject.CurrentFrame < 0
                ? _textureManager.Dictionary[$"{gameBaseObject.TextureName}_left_{gameBaseObject.CurrentFrame}"]
                : _textureManager.Dictionary[$"{gameBaseObject.TextureName}_right_{gameBaseObject.CurrentFrame}"];
        }

        SDL_RenderCopy(_renderer, texture, ref gameBaseObject.SRect,
            ref newRectangle);
    }

    private void RenderAllObject(Building building)
    {
        RenderSingleObjects(building);
        foreach (var unit in building.Units)
        {
            RenderSingleObjects(unit);
        }
    }


    private void RenderHud()
    {
        SDL_RenderCopy(_renderer, _textureManager.Dictionary[_hud.TextureName], ref _hud.SRect,
            ref _hud.DRect);
    }

    private void RenderButtons(ref bool matchState)
    {
        foreach (var button in _buttonsList.Where(btn => btn.IsGameObject))
        {
            SDL_RenderCopy(_renderer, _textureManager.Dictionary[button.CurrentTextureName], ref button.SRect,
                ref button.DRect);
        }

        foreach (var button in _buttonsList.Where(btn => !btn.IsGameObject))
        {
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
    }

    private void RenderSelectedObject(Building currentBuilding)
    {
        SDL_GetMouseState(out _x, out _y);
        SDL_Rect newRectangle = new()
        {
            h = 90,
            w = 90,
            x = _x,
            y = _y
        };
        SDL_RenderCopy(_renderer, _textureManager.Dictionary[currentBuilding.TextureName],
            ref currentBuilding.SRect,
            ref newRectangle);
    }

    private void Render(ref bool matchState)
    {
        SDL_RenderClear(_renderer);
        RenderHud();
        RenderButtons(ref matchState);
        SDL_RenderPresent(_renderer);
    }

    private void Render(Building currentBuilding, ref bool matchState)
    {
        SDL_RenderClear(_renderer);
        DrawMap(_camera, currentBuilding != null);
        foreach (var b in _buildings.ToArray())
        {
            RenderAllObject(b);
        }

        RenderMinimap(_buildings, _camera);
        RenderHud();
        RenderButtons(ref matchState);
        if (currentBuilding != null)
        {
            RenderSelectedObject(currentBuilding);
        }

        SDL_RenderPresent(_renderer);
    }
}