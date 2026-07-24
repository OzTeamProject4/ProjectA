using System;

public class ScreenStateModel
{
    public ScreenType CurrentScreen { get; private set; }

    public event Action<ScreenType> OnScreenChanged;

    public ScreenStateModel(ScreenType initialScreen)
    {
        CurrentScreen = initialScreen;
    }

    public void ChangeScreen(ScreenType screen)
    {
        if (CurrentScreen == screen)
        {
            return;
        }

        CurrentScreen = screen;

        OnScreenChanged?.Invoke(screen);
    }
}