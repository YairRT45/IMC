using IMCApp.ViewModels;

namespace IMCApp.Views;

public partial class DatosPacientePage : ContentPage
{
	public DatosPacientePage(PacienteViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}