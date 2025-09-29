using MessageLisko.Networking;

namespace MessageLisko;

public partial class MainPage : ContentPage
{
    MessageHandler _handler;

    public MainPage(MessageHandler handler)
    {
        InitializeComponent();
        _handler = handler;
        BindingContext = handler;
    }

    private void OnEntryCompleted(object sender, EventArgs e)
    {

        if (sender is Entry entry && !string.IsNullOrWhiteSpace(entry.Text))
        {
            _handler.sendMessage(entry.Text);
            entry.Text = string.Empty;
        }
    }
}
