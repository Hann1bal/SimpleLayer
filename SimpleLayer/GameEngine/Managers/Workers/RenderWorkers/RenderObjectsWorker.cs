using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderObjectsWorker
{
    private int _x, _y;

    /// <summary>
    ///     Отрисовка всех игровые объекты в приделах видимости камеры.
    /// </summary>
    /// <param name="gameBaseObject"></param>
    /// <param name="camera"></param>
    /// <param name="textureManager"></param>
    /// <param name="renderer"></param>
    public void RunWorker(List<Building> buildings, List<Unit> units, ref Camera camera, ref Texture textureManager,
        ref IntPtr renderer, ref Building? currentBuilding)
    {
        foreach (var building in buildings) RenderSingleObjects(building, ref camera, ref textureManager, ref renderer);

        foreach (var unit in units) RenderSingleObjects(unit, ref camera, ref textureManager, ref renderer);

        if (currentBuilding != null &&
            currentBuilding.BuildingAttributes.BuildingPlaceState == BuildingPlaceState.NonPlaced)
            RenderSelectedObject(ref currentBuilding, ref renderer, ref textureManager);
    }

    private void RenderSingleObjects(GameBaseObject gameBaseObject, ref Camera camera, ref Texture textureManager,
        ref IntPtr renderer)
    {
        IntPtr texture;
        int height;
        int width;
        switch (gameBaseObject)
        {
            case Unit:
                height = gameBaseObject.SRect.h;
                width = gameBaseObject.SRect.w;
                break;
            case Building:
                height = gameBaseObject.SRect.w / 5;
                width = gameBaseObject.SRect.h / 5;
                break;
            default:
                height = gameBaseObject.SRect.w / 5;
                width = gameBaseObject.SRect.h / 5;
                break;
        }


        SDL_Rect newRectangle = new()
        {
            h = height,
            w = width,
            x = (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition) - camera.CameraRect.x,
            y = (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition) - camera.CameraRect.y
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + camera.CameraRect.h)
            return;

        texture = gameBaseObject switch
        {
            Unit unit => unit.UnitsAttributes.MoAState switch
            {
                MoAState.Moving => unit.UnitsAttributes.DeltaX switch
                {
                    > 0f => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"],
                    < 0f => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_left_{unit.UnitsAttributes.CurrentMovingFrame}"],
                    _ => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
                },
                MoAState.Attacking => unit.UnitsAttributes.DeltaX switch
                {
                    > 0f => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_right_atack_{unit.UnitsAttributes.CurrentAttackFrame}"],
                    < 0f => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_left_atack_{unit.UnitsAttributes.CurrentAttackFrame}"],
                    _ => textureManager.Dictionary[
                        $"{unit.BaseObjectAttribute.TextureName}_right_atack_{unit.UnitsAttributes.CurrentAttackFrame}"]
                },
                _ => textureManager.Dictionary[
                    $"{gameBaseObject.BaseObjectAttribute.TextureName}_right_{unit.UnitsAttributes.CurrentMovingFrame}"]
            },
            Building building => textureManager.Dictionary[building.BaseObjectAttribute.TextureName],
            _ => textureManager.Dictionary[gameBaseObject.BaseObjectAttribute.TextureName]
        };

        SDL_RenderCopy(renderer, texture, ref gameBaseObject.SRect, ref newRectangle);
    }

    private void RenderSelectedObject(ref Building? selectedObject, ref IntPtr renderer, ref Texture textureManager)
    {
        SDL_GetMouseState(out _x, out _y);
        SDL_Rect newRectangle = new()
        {
            h = 90,
            w = 90,
            x = _x,
            y = _y
        };
        SDL_RenderCopy(renderer, textureManager.Dictionary[selectedObject.BaseObjectAttribute.TextureName],
            ref selectedObject.SRect,
            ref newRectangle);
    }
}