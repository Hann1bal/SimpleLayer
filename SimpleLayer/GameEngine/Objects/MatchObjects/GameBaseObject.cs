using SDL2;
using SimpleLayer.GameEngine.Objects.Attributes;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;

namespace SimpleLayer.GameEngine.Objects;

public class GameBaseObject : IGameBaseObject
{
    public GameBaseObjectAttribute BaseObjectAttribute;
    public SDL.SDL_Rect DRect;
    public SDL.SDL_Rect SRect;

    protected GameBaseObject(string textureName, int xPos, int yPos,
        int healthPoint, int team, ObjectType objectType, int heightSprite, int widthSprite)
    {
        BaseObjectAttribute = new GameBaseObjectAttribute
        {
            TextureName = textureName,
            HealthPoint = healthPoint,
            Team = team,
            DoAState = DoAState.Alive,
            ObjectType = objectType
        };
        SRect = new SDL.SDL_Rect
        {
            h = heightSprite,
            w = widthSprite
        };
        DRect = new SDL.SDL_Rect
        {
            x = (int) Math.Round(BaseObjectAttribute.XPosition),
            y = (int) Math.Round(BaseObjectAttribute.YPosition),
            h = SRect.h / 10,
            w = SRect.w / 10
        };
        BaseObjectAttribute.XPosition = xPos;
        BaseObjectAttribute.YPosition = yPos;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.SuppressFinalize(DRect);
        GC.SuppressFinalize(SRect);
        GC.SuppressFinalize(BaseObjectAttribute);
        GC.Collect(GC.MaxGeneration);
    }


    ~GameBaseObject()
    {
        Dispose();
    }
}