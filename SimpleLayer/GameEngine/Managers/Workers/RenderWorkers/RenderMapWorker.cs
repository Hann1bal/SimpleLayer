using System.Numerics;
using SimpleLayer.GameEngine.UtilComponents;
using static SDL2.SDL;

namespace SimpleLayer.GameEngine.Managers.Workers.RenderWorkers;

public class RenderMapWorker
{
    public void RunWorker(bool flag, ref Level level, ref Camera camera, ref IntPtr renderer)
    {
        RenderMap(flag, ref level, ref camera, ref renderer);
    }

    private void RenderMap(bool flag, ref Level level, ref Camera camera, ref IntPtr renderer)
    {
        for (var x = 0; x < Level.LevelEndX / 32; x++)
        for (var y = 0; y < Level.LevelEndY / 32; y++)
        {
            switch (flag)
            {
                case true:
                {
                    switch (level._tileLevel[new Vector2(x, y)].TileAttribute.isPlacibleTile)
                    {
                        case true:
                            switch (level._tileLevel[new Vector2(x, y)].TileAttribute.ContainBuilding)
                            {
                                case false:
                                    SDL_SetTextureColorMod(level._tileLevel[new Vector2(x, y)].TileAttribute._texture, 0, 100, 0);
                                    break;
                                default:
                                    SDL_SetTextureColorMod(level._tileLevel[new Vector2(x, y)].TileAttribute._texture, 100, 0, 0);
                                    break;
                            }

                            break;
                        default:
                            SDL_SetTextureColorMod(level._tileLevel[new Vector2(x, y)].TileAttribute._texture, 100, 0, 0);
                            break;
                    }

                    break;
                }
                case false:
                {
                    SDL_SetTextureColorMod(level._tileLevel[new Vector2(x, y)].TileAttribute._texture, 255, 255, 255);
                    break;
                }
            }

            var tmpDRect = new SDL_Rect
            {
                h = level._tileLevel[new Vector2(x, y)].SdlDRect.h,
                w = level._tileLevel[new Vector2(x, y)].SdlDRect.w,
                x = level._tileLevel[new Vector2(x, y)].SdlDRect.x - camera.CameraRect.x,
                y = level._tileLevel[new Vector2(x, y)].SdlDRect.y - camera.CameraRect.y
            };
            if (tmpDRect.x + tmpDRect.w < 0 || tmpDRect.x > 0 + camera.CameraRect.w ||
                tmpDRect.y + tmpDRect.h < 0 || tmpDRect.y > 0 + camera.CameraRect.h) continue;


            SDL_RenderCopy(renderer, level._tileLevel[new Vector2(x, y)].TileAttribute._texture,
                ref level._tileLevel[new Vector2(x, y)].SdlSRect,
                ref tmpDRect);
        }
    }
}