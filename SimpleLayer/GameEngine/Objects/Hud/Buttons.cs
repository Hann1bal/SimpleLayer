﻿using SDL2;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.Objects.States;

namespace SimpleLayer.Objects;

public class Buttons : HudBaseObject
{
    public ButtonAttribute ButtonAttribute;

    public Buttons(string textureName, SDL.SDL_Rect sRect, SDL.SDL_Rect dRect, ButtonType buttonType) :
        base(textureName,
            sRect, dRect)
    {
        ButtonAttribute = new ButtonAttribute
        {
            ButtonState = ButtonState.Unfocused, ButtonType = buttonType, ButtonPressState = ButtonPressState.Released
        };
    }

    public void UpdateTextureName(ButtonState buttonState, ButtonPressState buttonPressState)
    {
        switch (buttonState)
        {
            case ButtonState.Focused:
                switch (buttonPressState)
                {
                    case ButtonPressState.Pressed:
                        hudBaseObjectAttribute.CurrentTextureName = hudBaseObjectAttribute.TextureName + "_pressed";
                        ButtonAttribute.ButtonPressState = buttonPressState;
                        break;
                    case ButtonPressState.Released:
                        hudBaseObjectAttribute.CurrentTextureName = hudBaseObjectAttribute.TextureName + "_focused";
                        ButtonAttribute.ButtonState = buttonState;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(buttonPressState), buttonPressState, null);
                }

                break;
            case ButtonState.Unfocused:
                switch (buttonPressState)
                {
                    case ButtonPressState.Pressed:
                        hudBaseObjectAttribute.CurrentTextureName = hudBaseObjectAttribute.TextureName + "_pressed";
                        ButtonAttribute.ButtonPressState = buttonPressState;
                        break;
                    case ButtonPressState.Released:
                        hudBaseObjectAttribute.CurrentTextureName = hudBaseObjectAttribute.TextureName;
                        ButtonAttribute.ButtonState = buttonState;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(buttonPressState), buttonPressState, null);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buttonState), buttonState, null);
        }
    }
}