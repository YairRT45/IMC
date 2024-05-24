using IMCApp.ViewModels;

namespace IMCApp.Views;

public partial class PacientePage : ContentPage
{
	public PacientePage(PacienteViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}