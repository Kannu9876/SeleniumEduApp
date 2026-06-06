namespace SeleniumEduApp.Services;

public class PageNavigationService
{
    public string CurrentPage { get; private set; } = "home";

    public event Action? OnChange;

    public void NavigateTo(string page)
    {
        CurrentPage = page;
        OnChange?.Invoke();
    }
}
