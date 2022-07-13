using SDL2;
using SimpleLayer.GameEngine.UI.UIAttributes;
using SimpleLayer.GameEngine.UI.UIStates;

namespace SimpleLayer.GameEngine.UI.UIElements;

public class TextInput : IDisposable
{
    public TextInputStates TextInputStates = TextInputStates.Unfocused;
    public readonly TextInputAttribute TextInputAttribute;
    public string Textbuffer = "";

    public TextInput(TextInputAttribute textInputAttribute)
    {
        TextInputAttribute = textInputAttribute;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        GC.Collect(GC.MaxGeneration);
    }
}