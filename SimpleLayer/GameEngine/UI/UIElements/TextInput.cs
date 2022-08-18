using SDL2;
using SimpleLayer.GameEngine.UI.UIAttributes;
using SimpleLayer.GameEngine.UI.UIStates;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class TextInput : IDisposable
{
    public readonly int MaxLenght = 490;
    private readonly IntPtr monserat = SDL_ttf.TTF_OpenFont("./Data/Fonts/OpenSans.ttf", 25);
    public readonly TextInputAttribute TextInputAttribute;
    public int Buffer;
    public int Buffer2;
    public string CurentString = "";
    public bool flag = false;
    public string LastString = "";
    public string Textbuffer = "";
    public SDL.SDL_Rect TextInputClipRec;
    public SDL.SDL_Rect TextInputInnerClipRec;
    public SDL.SDL_Rect TextInputRec;
    public TextInputStates TextInputStates = TextInputStates.Unfocused;


    /// <summary>
    ///     Создает поле ввода текста.
    ///     Пока нет синхронизации для разной длины символов.
    ///     Нет каретки.
    /// </summary>
    /// <param name="textInputAttribute"></param>
    public TextInput(TextInputAttribute textInputAttribute)
    {
        TextInputAttribute = textInputAttribute;
        TextInputRec = new SDL.SDL_Rect
        {
            h = TextInputAttribute.SizeAxisY, w = TextInputAttribute.SizeAxisX, x = TextInputAttribute.XStartPos,
            y = TextInputAttribute.YStartPos
        };
        TextInputClipRec = new SDL.SDL_Rect
            {h = TextInputRec.h, w = 500, x = 0, y = TextInputRec.y};
        TextInputInnerClipRec = new SDL.SDL_Rect
            {h = TextInputClipRec.h - 10, w = 490, x = 5, y = TextInputClipRec.y + 5};
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect(GC.MaxGeneration);
    }

    public int GetCurentTextLenght(IntPtr monseratos)
    {
        SDL_ttf.TTF_SizeText(monseratos, CurentString, out Buffer2, out TextInputRec.h);
        return Buffer2;
    }

    public int GetLastTextLenght(IntPtr monseratos)
    {
        SDL_ttf.TTF_SizeText(monseratos, LastString, out Buffer, out TextInputRec.h);
        return Buffer;
    }

    public void ClearBufferString()
    {
        Textbuffer = Textbuffer.Remove(0);
        TextInputRec.x = 5;
    }
}