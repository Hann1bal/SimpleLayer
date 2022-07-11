using SDL2;
using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public class GameBaseObject : IGameBaseObject
{
    public GameBaseObjectAttribute BaseObjectAttribute;
    public SDL.SDL_Rect DRect;
    public SDL.SDL_Rect SRect;

    protected GameBaseObject(string textureName, int xPos, int yPos,
        int healthPoint, int team, ObjectType objectType)
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
            h = 210,
            w = 210
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