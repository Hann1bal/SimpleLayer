using SDL2;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UI.UIStates;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderHudWorker
{
    public void RunWorker(List<Buttons> buttons, ref IntPtr renderer,
        ref Texture textureManager,
        ref Hud hud, ref TextInput textInput, ref IntPtr monserat )
    {
        RenderHud(ref renderer, ref textureManager, ref hud);
        RenderButtons(buttons, ref renderer, ref textureManager);
        RenderTextInput(ref textInput, ref renderer, ref monserat);
    }

    private void RenderButtons(List<Buttons> buttons, ref IntPtr renderer,
        ref Texture textureManager)
    {
 
        
        foreach (var button in buttons)

            SDL_RenderCopy(renderer,
                textureManager.Dictionary[button.hudBaseObjectAttribute.CurrentTextureName], IntPtr.Zero,
                ref button.DRect);
        
    }

    private void RenderTextInput(ref TextInput textInput, ref IntPtr renderer, ref IntPtr monserat)
    {
        if (textInput.TextInputStates != TextInputStates.Focused) return;
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{textInput.Textbuffer}", new SDL_Color {a = 0, r = 255, b = 255, g = 255});
        var textureWreed = SDL_CreateTextureFromSurface(renderer, message);
        SDL_ttf.TTF_SizeText(monserat, textInput.Textbuffer, out textInput.TextInputRec.w,
            out textInput.TextInputRec.h);
        SDL_SetTextureColorMod(textureWreed, 0, 0, 0);
        SDL_SetRenderDrawColor(renderer, 192, 192, 192, 255);
        SDL_RenderFillRect(renderer, ref textInput.TextInputClipRec);
        SDL_SetRenderDrawColor(renderer, 128, 128, 128, 255);
        SDL_RenderFillRect(renderer, ref textInput.TextInputInnerClipRec);
        SDL_RenderSetClipRect(renderer, ref textInput.TextInputInnerClipRec);
        if (textInput.CurentString.Length > 0 && textInput.flag)
        {
            textInput.TextInputRec.x -= textInput.GetCurentTextLenght(monserat) -
                                        textInput.GetLastTextLenght(monserat) +
                                        (textInput.TextInputRec.x + textInput.TextInputRec.w - textInput.MaxLenght);
            Console.WriteLine(
                $"curent x: {textInput.TextInputRec.x} {textInput.TextInputRec.x + textInput.TextInputRec.w}");
            textInput.flag = false;
        }

        SDL_RenderCopy(renderer, textureWreed, IntPtr.Zero, ref textInput.TextInputRec);
        SDL_RenderSetClipRect(renderer, IntPtr.Zero);
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