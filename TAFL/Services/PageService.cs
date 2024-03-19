using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

using TAFL.Contracts.Services;
using TAFL.ViewModels;
using TAFL.Views;

namespace TAFL.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<Lab1ViewModel, Lab1Page>();
        Configure<Lab2ViewModel, Lab2Page>();
        Configure<Lab5ViewModel, Lab5Page>();
        Configure<Lab6ViewModel, Lab6Page>();
        Configure<Lab7ViewModel, Lab7Page>();
        Configure<Lab8ViewModel, Lab8Page>();
        Configure<Lab9ViewModel, Lab9Page>();
        Configure<DebugViewModel, DebugPage>();
        Configure<SettingsViewModel, SettingsPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
