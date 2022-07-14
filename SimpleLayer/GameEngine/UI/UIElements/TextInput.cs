using SDL2;
using SimpleLayer.GameEngine.UI.UIAttributes;
using SimpleLayer.GameEngine.UI.UIStates;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class TextInput : IDisposable
{
    public TextInputStates TextInputStates = TextInputStates.Unfocused;
    public readonly TextInputAttribute TextInputAttribute;
    public SDL.SDL_Rect TextInputRec;
    public SDL.SDL_Rect TextInputClipRec;
    public string Textbuffer = "";
    public readonly int MaxLenght = 500;
    public int CurentLenght = 0;

    /// <summary>
    /// Создает поле ввода текста.
    /// Пока нет синхронизации для разной длины символов.
    /// Нет каретки.
    /// </summary>
    /// <param name="textInputAttribute"></param>
    public TextInput(TextInputAttribute textInputAttribute)
    {
        TextInputAttribute = textInputAttribute;
        TextInputRec = new()
        {
            h = TextInputAttribute.SizeAxisY, w = TextInputAttribute.SizeAxisX, x = TextInputAttribute.XStartPos,
            y = TextInputAttribute.YStartPos
        };
        TextInputClipRec = new SDL.SDL_Rect
            {h = TextInputRec.h, w = 500, x = 0, y = TextInputRec.y};
    }

    public void ClearBufferString()
    {
        Textbuffer = Textbuffer.Remove(0);
        TextInputRec.x = 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect(GC.MaxGeneration);
    }
}