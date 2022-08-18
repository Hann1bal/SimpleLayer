using SDL2;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderMatchMenuHudWorker
{
    public void RunWorker(ref IntPtr renderer,
        ref Texture textureManager, ref Building? current, ref IntPtr monserat)
    {
        if (current != null && current.BuildingAttributes.BuildingPlaceState == BuildingPlaceState.Selected)
        {
            DrawHudMenuRect(ref renderer);
            RenderBuildingInfo(ref renderer, ref current, ref textureManager, ref monserat);
        }
    }

    private void DrawHudMenuRect(ref IntPtr renderer)
    {
        var SDLDResct = new SDL_Rect() {x = 400, y = 815, h = 265, w = 890};
        SDL_SetRenderDrawBlendMode(renderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);
        SDL_SetRenderDrawColor(renderer, 0, 0, 0, 150);
        SDL_RenderFillRect(renderer, ref SDLDResct);
    }

    private void RenderBuildingInfo(ref IntPtr renderer, ref Building curent, ref Texture textureManager,
        ref IntPtr monserat)
    {
        var newrect = new SDL_Rect() {x = 1150, y = 850, h = 50, w = 50};
        var textRect = new SDL_Rect() {x = 1150, y = 950, w = 100, h = 30};
        var textRect2 = new SDL_Rect() {x = 1150, y = 980, w = 100, h = 30};
        var textRect3 = new SDL_Rect() {x = 1150, y = 1010, w = 100, h = 30};
        var textRect4 = new SDL_Rect() {x = 1150, y = 1040, w = 100, h = 30};
        var _color = new SDL_Color() {r = 192, b = 192, g = 192, a = 255};

        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"Name: {curent.BaseObjectAttribute.TextureName}", _color);
        var message2 = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"HelthPoint: {curent.BaseObjectAttribute.HealthPoint}", _color);
        var message3 = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"Type: {curent.BaseObjectAttribute.ObjectType}", _color);
        var message4 = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"Tier: {curent.BaseObjectAttribute.Tier}", _color);
        var textureWreed = SDL_CreateTextureFromSurface(renderer, message);
        var textureWreed2 = SDL_CreateTextureFromSurface(renderer, message2);
        var textureWreed3 = SDL_CreateTextureFromSurface(renderer, message3);
        var textureWreed4 = SDL_CreateTextureFromSurface(renderer, message4);

        SDL_RenderCopy(renderer, textureWreed, IntPtr.Zero, ref textRect);
        SDL_RenderCopy(renderer, textureWreed2, IntPtr.Zero, ref textRect2);
        SDL_RenderCopy(renderer, textureWreed3, IntPtr.Zero, ref textRect3);
        SDL_RenderCopy(renderer, textureWreed4, IntPtr.Zero, ref textRect4);
        SDL_FreeSurface(message);
        SDL_FreeSurface(message2);
        SDL_FreeSurface(message3);
        SDL_FreeSurface(message4);
        SDL_DestroyTexture(textureWreed);
        SDL_DestroyTexture(textureWreed2);
        SDL_DestroyTexture(textureWreed3);
        SDL_DestroyTexture(textureWreed4);
        SDL_RenderCopy(renderer, textureManager.Dictionary[curent.BaseObjectAttribute.TextureName], ref curent.SRect,
            ref newrect);
    }
}