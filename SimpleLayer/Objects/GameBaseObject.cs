using System.Numerics;
using SDL2;
using SimpleLayer.GameEngine;

namespace SimpleLayer.Objects;

public class GameBaseObject : IGameBaseObject
{
    public int xPosition, yPosition;
    public SDL.SDL_Rect _sRect;
    public SDL.SDL_Rect _dRect;
    private readonly IntPtr _rendererObject;
    public int _healtPpoint;
    public string _textureName;
    private bool _disposedValue;
    public Vector2 _lastQuadrant;
    public GameBaseObject _target;
    public int _targetDistance;
    public int _team;
    public bool isDead;

    public GameBaseObject(ref IntPtr renderer, string textureName, int xPos, int yPos,
        int healtPpoint, int team)
    {
        _team = team;
        _healtPpoint = healtPpoint;
        _rendererObject = renderer;

        xPosition = xPos;
        yPosition = yPos;
        _textureName = textureName;
        _sRect.h = 300;
        _sRect.w = 300;
        _dRect.x = xPosition;
        _dRect.y = yPosition;
        _dRect.w = _sRect.w / 10;
        _dRect.h = _sRect.h / 10;
    }

    public void Render(ref Camera camera, ref Texture textureManager)
    {
        SDL.SDL_Rect newRectangle = new()
        {
            h = _sRect.w / 10, w = _sRect.w / 10, x = xPosition - camera._cameraRect.x,
            y = yPosition - camera._cameraRect.y
        };

        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + camera._cameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + camera._cameraRect.h)
        {
            return;
        }

        SDL.SDL_RenderCopy(_rendererObject, textureManager.Dictionary[_textureName], ref _sRect, ref newRectangle);
    }
    public void Update()
    {
    }

    public void Move()
    {
    }


    ~GameBaseObject()
    {
        Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(_textureName);

        GC.SuppressFinalize(_dRect);
        GC.SuppressFinalize(_sRect);
        GC.Collect(GC.MaxGeneration);
    }
}