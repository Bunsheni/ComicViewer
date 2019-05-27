using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.CtModels;
using ComicViewer.ViewModels;

namespace ComicViewer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SearchPage : ContentPage
    {
        SearchOptionViewModel viewModel;

        public SearchPage ()
		{
			InitializeComponent ();
            BindingContext = viewModel = new SearchOptionViewModel();
        }

        protected override void OnDisappearing()
        {
            viewModel.UpdateRequest();
            base.OnDisappearing();
        }
    }

    public class ToggleButton : Button
    {
        public event EventHandler<ToggledEventArgs> Toggled;

        public static BindableProperty IsToggledProperty =
            BindableProperty.Create("IsToggled", typeof(bool), typeof(ToggleButton), false,
                                    propertyChanged: OnIsToggledChanged);

        public ToggleButton()
        {
            Clicked += (sender, args) => IsToggled ^= true;
        }

        public bool IsToggled
        {
            set { SetValue(IsToggledProperty, value); }
            get { return (bool)GetValue(IsToggledProperty); }
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            VisualStateManager.GoToState(this, "ToggledOff");
        }

        static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ToggleButton toggleButton = (ToggleButton)bindable;
            bool isToggled = (bool)newValue;

            // Fire event
            toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

            // Set the visual state
            VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");
        }
    }
}