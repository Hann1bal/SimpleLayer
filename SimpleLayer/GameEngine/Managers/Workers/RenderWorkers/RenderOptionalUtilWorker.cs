using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class RenderOptionalUtilWorker
{
    /// <summary>
    ///     Пока что мусорный класс, для хранения методов которые могут опционально подключатся в цикл рисования.
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="camera"></param>
    public void RunWorker(ref IntPtr renderer, ref Camera camera)
    {
        RenderMesh(ref renderer, ref camera);
    }

    private void RenderMesh(ref IntPtr renderer, ref Camera camera)
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
        {
            SDL_RenderDrawLine(renderer, i * 320 - camera.CameraRect.x, j * 320 - camera.CameraRect.y,
                i * 320 - 320 - camera.CameraRect.x, j * 320 - camera.CameraRect.y);
            SDL_RenderDrawLine(renderer, i * 320 - camera.CameraRect.x, j * 320 - camera.CameraRect.y,
                i * 320 - camera.CameraRect.x, j * 320 + 320 - camera.CameraRect.y);
            SDL_RenderDrawLine(renderer, i * 320 + 320 - camera.CameraRect.x, j * 320 + camera.CameraRect.y,
                i * 320 + 320 - camera.CameraRect.x, j * 320 + 320 - camera.CameraRect.y);
            SDL_RenderDrawLine(renderer, i * 320, j * 320 + 320 - camera.CameraRect.y,
                i * 320 + 320 - camera.CameraRect.x, j * 320 + 320 - camera.CameraRect.y);
        }
    }
}