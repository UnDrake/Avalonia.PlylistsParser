using System;
using System.Net.Http;
using ReactiveUI;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.PlaylistsParser.Models;

namespace Avalonia.PlaylistsParser.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand StartParseCommand { get; }
    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }
    
    private Playlist? _playlist;
    public Playlist? Playlist
    {
        get => _playlist;
        private set => this.RaiseAndSetIfChanged(ref _playlist, value);
    }
    
    public MainWindowViewModel()
    {
        StartParseCommand = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                try
                {
                    Playlist = Playlist.Search(_searchText);
                    if (!string.IsNullOrEmpty(Playlist.Avatar))
                    {
                        AvatarImageBitmap = LoadImage(Playlist.Avatar);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        });
    }
    
    private Bitmap? _avatarImageBitmap;
    public Bitmap? AvatarImageBitmap
    {
        get => _avatarImageBitmap;
        set => this.RaiseAndSetIfChanged(ref _avatarImageBitmap, value);
    }
    
    private Bitmap? LoadImage(string imgUrl)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(imgUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        return new Bitmap(stream);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return null;
    }
}