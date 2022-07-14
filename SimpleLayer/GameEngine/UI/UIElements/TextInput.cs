using SDL2;
using SimpleLayer.GameEngine.UI.UIAttributes;
using SimpleLayer.GameEngine.UI.UIStates;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class TextInput : IDisposable
{
    public TextInputStates TextInputStates = TextInputStates.Unfocused;
    public readonly TextInputAttribute TextInputAttribute;
    public SDL.SDL_Rect TextInputRec;
    public SDL.SDL_Rect TextInputSRec;
    public string Textbuffer = "";

    public TextInput(TextInputAttribute textInputAttribute)
    {
        TextInputAttribute = textInputAttribute;
        TextInputRec = new()
        {
            h = TextInputAttribute.SizeAxisY, w = TextInputAttribute.SizeAxisX, x = TextInputAttribute.XStartPos,
            y = TextInputAttribute.YStartPos
        };
        TextInputSRec = new SDL.SDL_Rect
            {h = TextInputRec.h, w = 500, x = 0, y = 0};
    }

    public void BufferString()
    {
        Textbuffer = Textbuffer.Remove(0, Textbuffer.Length);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect(GC.MaxGeneration);
    }
}