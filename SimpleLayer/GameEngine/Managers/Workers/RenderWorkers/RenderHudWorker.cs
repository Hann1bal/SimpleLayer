using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderHudWorker
{
    public void RunWorker(List<Buttons> buttons, ref IntPtr renderer, ref Texture textureManager,
        ref Hud hud)
    {
        RenderHud(ref renderer, ref textureManager, ref hud);
        RenderButtons(buttons, ref renderer, ref textureManager);
    }

    private void RenderButtons(List<Buttons> buttons, ref IntPtr renderer,
        ref Texture textureManager)
    {
        foreach (var button in buttons)

            SDL_RenderCopy(renderer,
                textureManager.Dictionary[button.hudBaseObjectAttribute.CurrentTextureName], ref button.SRect,
                ref button.DRect);
    }

    private void RenderHud(ref IntPtr renderer, ref Texture textureManager,
        ref Hud hud)
    {
        SDL_RenderCopy(renderer, textureManager.Dictionary[hud.hudBaseObjectAttribute.CurrentTextureName],
            ref hud.SRect,
            ref hud.DRect);
    }
}