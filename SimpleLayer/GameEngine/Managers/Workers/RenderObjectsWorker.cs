using SimpleLayer.GameEngine.UtilComponents;
using SimpleLayer.Objects;
using SimpleLayer.Objects.States;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class RenderObjectsWorker
{
    public void RunWorker(GameBaseObject gameBaseObject, ref Camera camera, ref Texture textureManager,
        ref IntPtr renderer)
    {
        RenderSingleObjects(gameBaseObject, ref camera, ref textureManager, ref renderer);
    }

    private void RenderSingleObjects(GameBaseObject gameBaseObject, ref Camera camera, ref Texture textureManager,
        ref IntPtr renderer)
    {
        IntPtr texture;
        SDL_Rect newRectangle = new()
        {
            h = gameBaseObject.SRect.w / 5,
            w = gameBaseObject.SRect.w / 5,
            x = (int) Math.Round(gameBaseObject.BaseObjectAttribute.XPosition) - camera.CameraRect.x,
            y = (int) Math.Round(gameBaseObject.BaseObjectAttribute.YPosition) - camera.CameraRect.y
        };
        if (newRectangle.x + newRectangle.w < 0 || newRectangle.x > 0 + camera.CameraRect.w ||
            newRectangle.y + newRectangle.h < 0 || newRectangle.y > 0 + camera.CameraRect.h)
            return;

        switch (gameBaseObject)
        {
            case Unit unit:
            {
                texture = unit.UnitsAttributes.MoAState switch
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
                };
                break;
            }
            case Building building:
            {
                texture = textureManager.Dictionary[building.BaseObjectAttribute.TextureName];
                break;
            }
            default:
                texture = textureManager.Dictionary[gameBaseObject.BaseObjectAttribute.TextureName];
                break;
        }

        SDL_RenderCopy(renderer, texture, ref gameBaseObject.SRect, ref newRectangle);
    }
}