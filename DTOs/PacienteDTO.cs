using CommunityToolkit.Mvvm.ComponentModel;

namespace IMCApp.DTOs
{
    public partial class PacienteDTO : ObservableObject
    {
        [ObservableProperty]
        public int idPaciente;
        [ObservableProperty]
        public string nombre;
        [ObservableProperty]
        public string apellido;
        [ObservableProperty]
        public int edad;
        [ObservableProperty]
        public double peso;
        [ObservableProperty]
        public int estatura;
        [ObservableProperty]
        public string sexo;
        [ObservableProperty]
        public string actividadFisica;
        [ObservableProperty]
        public double imc;
        [ObservableProperty]
        public double grasaCorporal;
        [ObservableProperty]
        public double pesoIdeal;
        [ObservableProperty]
        public double gastoEnergiaDiario;
        [ObservableProperty]
        public string categoriaImc;
    }
}
