using SDL2;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UI.UIStates;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderHudWorker
{
    private readonly IntPtr monserat = SDL_ttf.TTF_OpenFont("./Data/Fonts/OpenSans.ttf", 30);
    private int _maxLenght = 500;
    private int _curentLenght = 0;

    public void RunWorker(List<Buttons> buttons, ref IntPtr renderer,
        ref Texture textureManager,
        ref Hud hud, ref TextInput textInput)
    {
        RenderHud(ref renderer, ref textureManager, ref hud);
        RenderButtons(buttons, ref renderer, ref textureManager);
        RenderTextInput(ref textInput, ref renderer);
    }

    private void RenderButtons(List<Buttons> buttons, ref IntPtr renderer,
        ref Texture textureManager)
    {
        foreach (var button in buttons)

            SDL_RenderCopy(renderer,
                textureManager.Dictionary[button.hudBaseObjectAttribute.CurrentTextureName], IntPtr.Zero,
                ref button.DRect);
    }

    private void RenderTextInput(ref TextInput textInput, ref IntPtr renderer)
    {
        if (textInput.TextInputStates != TextInputStates.Focused) return;
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{textInput.Textbuffer}", new SDL_Color() {a = 0, r = 255, b = 255, g = 255});
        var textureWreed = SDL_CreateTextureFromSurface(renderer, message);
        textInput.TextInputRec.w = textInput.Textbuffer.Length * 20;
        if (textInput.TextInputRec.w > _maxLenght && textInput.TextInputSRec.w > _curentLenght)
        {
            _curentLenght = textInput.TextInputSRec.w;
            textInput.TextInputSRec.x += 15;
        }

        SDL_SetTextureColorMod(renderer, 255, 255, 0); //set yellow letters
        SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255); //set blue background
        SDL_SetRenderDrawBlendMode(renderer, SDL_BlendMode.SDL_BLENDMODE_ADD);
        SDL_RenderCopy(renderer, textureWreed, ref textInput.TextInputSRec, ref textInput.TextInputRec);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }

    private void RenderHud(ref IntPtr renderer, ref Texture textureManager,
        ref Hud hud)
    {
        SDL_RenderCopy(renderer, textureManager.Dictionary[hud.hudBaseObjectAttribute.CurrentTextureName],
            ref hud.SRect,
            ref hud.DRect);
    }
}