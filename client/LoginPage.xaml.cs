// UI side does not interact with clientConnect AT ALL

using MessageLisko.Networking;

namespace MessageLisko;

public partial class LoginPage : ContentPage
{

	private readonly MessageHandler _messages;

	public LoginPage(MessageHandler messages)	
	{
		InitializeComponent();
		_messages = messages;
	}

	private async void OnEntryCompleted(object sender, EventArgs e)
	{
        if (sender is Entry entry && !string.IsNullOrWhiteSpace(entry.Text))
        {
            _messages.HandleClientLogin(entry.Text ?? "anonymous");
        }

		await Shell.Current.GoToAsync(nameof(MainPage));
        
    }
}