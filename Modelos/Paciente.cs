using System.ComponentModel.DataAnnotations;

namespace IMCApp.Modelos
{
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Edad { get; set; }
        public double Peso { get; set; }
        public int Estatura { get; set; }
        public string Sexo { get; set; }
        public string ActividadFisica { get; set; }
        public double Imc { get; set; }
        public double GrasaCorporal { get; set; }
        public double PesoIdeal { get; set; }
        public double GastoEnergiaDiario { get; set; }
        public string CategoriaImc { get; set; }
    }
}
