using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SignalRMyEasyChat
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatPage : ContentPage
	{
	    ChatPageViewModel vm;

	    public ChatPage (string username)
		{
			InitializeComponent ();
			vm = new ChatPageViewModel(username);
			BindingContext = vm;
		}
    }
}