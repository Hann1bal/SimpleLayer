using SDL2;
using SimpleLayer.GameEngine.Network.Types;
using SimpleLayer.GameEngine.Objects;
using SimpleLayer.GameEngine.Objects.MatchObjects;
using SimpleLayer.GameEngine.Objects.States;
using SimpleLayer.GameEngine.Objects.Types;
using SimpleLayer.GameEngine.UI.UIElements;
using SimpleLayer.GameEngine.UtilComponents;
using EventType = SimpleLayer.GameEngine.Objects.Types.EventType;

namespace SimpleLayer.GameEngine.Managers.Workers;

public class EventWorker
{
    public void RunJob(ref EventType eventType, ref GameLogicManager gameLogicManager, ref SDL.SDL_Event e,
        ref Camera camera, ref Building? currentBuilding, ref Time timer, ref Player player, ref bool _isShiftPressed, ref List<Buttons> buttons,ref HudManager hudManager,ref GameState gameState, ref MatchState matchState)
    {
        DoAction(ref eventType, ref gameLogicManager, ref e, ref camera, ref currentBuilding, ref timer, ref player,
            ref _isShiftPressed, ref  buttons, ref hudManager, ref  gameState, ref  matchState);
    }

    private void DoAction(ref EventType eventType, ref GameLogicManager gameLogicManager, ref SDL.SDL_Event e,
        ref Camera camera, ref Building? currentBuilding, ref Time timer, ref Player player, ref bool _isShiftPressed, ref List<Buttons> buttons, ref HudManager hudManager, ref GameState gameState, ref MatchState matchState)
    {
        switch (eventType)
        {
            case EventType.SelectBuilding:
                currentBuilding =
                    gameLogicManager.BuildingWorker.SelectBuilding(e.button.x + camera.CameraRect.x,
                        e.button.y + camera.CameraRect.y);
                if (currentBuilding != null)
                {
                    currentBuilding.BuildingAttributes.BuildingPlaceState = BuildingPlaceState.Selected;
                    foreach (var btns in buttons
                                 .Where(c => c.ButtonAttribute.ButtonType == ButtonType.MatchHudButton))
                        if (currentBuilding.BuildingAttributes.BuildingType == BuildingType.Base &&
                            btns.hudBaseObjectAttribute.TextureName == "destroyTextButton")
                            btns.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                        else
                            btns.ButtonAttribute.EoDButtonState = EoDButtonState.Enabled;
                }
                break;
            case EventType.PlaseBuilding:
                gameLogicManager.BuildingWorker.PlaceBuilding(e.button.x + camera.CameraRect.x,
                    e.button.y + camera.CameraRect.y, ref currentBuilding, timer, ref player);
                if (!_isShiftPressed)
                {
                    currentBuilding.Dispose();
                    currentBuilding = null;
                }

                break;
            case EventType.PressButton:
                foreach (var button in buttons.Where(b => b.ButtonAttribute.ButtonState
                                                              is ButtonState.Focused &&
                                                          b.ButtonAttribute.EoDButtonState ==
                                                          EoDButtonState.Enabled)
                             .ToArray())
                    hudManager.PressButton(button, ref gameState, ref matchState, ref currentBuilding, ref timer);
                
               
                break;
            case EventType.UnselectTemplate:
                currentBuilding.Dispose();
                currentBuilding = null;
                break;
            case EventType.UnselectCurrentBuilding:
                gameLogicManager.BuildingWorker.UnSelectBuilding(ref currentBuilding);
                foreach (var btns in buttons
                             .Where(c => c.ButtonAttribute.ButtonType == ButtonType.MatchHudButton)
                             .ToList())
                    btns.ButtonAttribute.EoDButtonState = EoDButtonState.Disabled;
                break;
            case EventType.ReleaseButton:
                foreach (var button in buttons.Where(b =>
                             b.ButtonAttribute.ButtonPressState == ButtonPressState.Pressed))
                    hudManager.ReleaseButton(button, ref currentBuilding);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
        }
    }
}