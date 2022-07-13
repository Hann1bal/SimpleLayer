using SDL2;
using SimpleLayer.GameEngine.Objects;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderTextWorker
{
    private readonly SDL_Color _color = new() {a = 0, r = 255, b = 0, g = 0};
    private readonly IntPtr monserat = SDL_ttf.TTF_OpenFont("./Data/Fonts/OpenSans.ttf", 550);
    private SDL_Rect mes;
    private SDL_Rect mesh;

    /// <summary>
    ///     Запуск отрисовки текстовой информации о состоянии игрока, таких как время, количество доступного золота и т.д.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="renderer"></param>
    /// <param name="time"></param>
    public void RunWorker(ref Player player, ref IntPtr renderer, ref int time)
    {
        RenderGoldInfo(ref player, ref renderer);
        RenderTime(ref time, ref renderer);
    }

    private void RenderGoldInfo(ref Player player, ref IntPtr renderer)
    {
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"Gold: {player.PlayerAttribute.Gold}",
            _color);
        var textureWreed = SDL_CreateTextureFromSurface(renderer, message);
        mesh.x = 100; //controls the rect's x coorinate 
        mesh.y = 10; // controls the rect's y coordinte
        mesh.w = 250; // controls the width of the rect
        mesh.h = 80; // controls the height of the rect
        SDL_RenderCopy(renderer, textureWreed, IntPtr.Zero, ref mesh);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }

    private void RenderTime(ref int time, ref IntPtr renderer)
    {
        var minutes = time / 60;
        var seconds = time - minutes * 60;
        var message = SDL_ttf.TTF_RenderText_Solid(monserat,
            $"{minutes}:{seconds}", _color);
        var textureWreed = SDL_CreateTextureFromSurface(renderer, message);
        mes.x = 950; //controls the rect's x coorinate 
        mes.y = 10; // controls the rect's y coordinte
        mes.w = 100; // controls the width of the rect
        mes.h = 80; // controls the height of the rect

        SDL_RenderCopy(renderer, textureWreed, IntPtr.Zero, ref mes);
        SDL_FreeSurface(message);
        SDL_DestroyTexture(textureWreed);
    }
}