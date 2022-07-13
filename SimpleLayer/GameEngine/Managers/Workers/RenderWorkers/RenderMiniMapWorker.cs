using System.Numerics;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderMiniMapWorker
{
    public void RunWorker(List<Building> buildings, List<Unit> units, ref Camera camera, ref Level level2,
        ref Level level, ref IntPtr renderer, ref Texture textureManager)
    {
        RenderMiniMap(buildings, units, ref camera, ref level2, ref level, ref renderer, ref textureManager);
    }

    private void RenderMiniMap(List<Building> buildings, List<Unit> units, ref Camera camera, ref Level level2,
        ref Level level, ref IntPtr renderer, ref Texture textureManager)
    {
        var newCameraSRect = new SDL_Rect
        {
            h = 90, w = 192,
            x = 22 + camera.CameraRect.x / 10,
            y = 805 + camera.CameraRect.y / 12
        };
        for (var x = 0; x < Level.LevelEndX / 32; x++)
        for (var y = 0; y < Level.LevelEndY / 32; y++)
        {
            level2.DRect.x = 22 + x * 32 / 10;
            level2.DRect.y = 805 + y * 32 / 12;
            level2.DRect.h = level2.SRect.h / 8;
            level2.DRect.w = level2.SRect.w / 8;
            SDL_RenderCopy(renderer, level._tileLevel[new Vector2(x, y)].TileAttribute._texture, ref level2.SRect,
                ref level2.DRect);
        }

        SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
        SDL_RenderDrawRect(renderer, ref newCameraSRect);
        foreach (var build in buildings) RenderSingleIdleObjects(build, ref camera, ref textureManager, ref renderer);
        foreach (var unit in units.ToArray())
            RenderSingleIdleObjects(unit, ref camera, ref textureManager, ref renderer);
    }

    private void RenderSingleIdleObjects(GameBaseObject gameBaseObject, ref Camera camera, ref Texture textureManager,
        ref IntPtr renderer)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = gameBaseObject.SRect.w / 50,
            w = gameBaseObject.SRect.w / 50,
            x = 25 + (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition) / 10,
            y = 805 + (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition) / 12
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + camera.CameraRect.h)
            return;

        texture = gameBaseObject.BaseObjectAttribute.ObjectType switch
        {
            ObjectType.Building => textureManager.Dictionary[gameBaseObject.BaseObjectAttribute.TextureName],
            ObjectType.Unit when gameBaseObject is Unit unit => unit.UnitsAttributes.DeltaX switch
            {
                > 0 => textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"],
                < 0 => textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_left_{unit.UnitsAttributes.CurrentMovingFrame}"],
                _ => textureManager.Dictionary[
                    $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
            }
        };


        SDL_RenderCopy(renderer, texture, ref gameBaseObject.SRect,
            ref newRectangle);
    }
}