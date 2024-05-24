using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore;
using IMCApp.DataAccess;
using IMCApp.DTOs;
using IMCApp.Utilidades;
using IMCApp.Modelos;
using IMCApp.Views;

namespace IMCApp.ViewModels
{
    public partial class PacienteViewModel : ObservableObject, IQueryAttributable
    {
        private readonly PacienteDBContext _dbContext;

        [ObservableProperty]
        private PacienteDTO pacienteDto = new PacienteDTO();
        
        [ObservableProperty]
        private string tituloPagina;

        private int IdPaciente;

        [ObservableProperty]
        private bool loadingEsVisible = false;

        [ObservableProperty]
        private string errorNombre;
        [ObservableProperty]
        private string errorApellido;
        [ObservableProperty]
        private string errorEdad;
        [ObservableProperty]
        private string errorPeso;
        [ObservableProperty]
        private string errorEstatura;
        [ObservableProperty]
        private string errorSexo;
        [ObservableProperty]
        private string errorActFisica;


        public PacienteViewModel(PacienteDBContext context)
        {
            _dbContext = context;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var id = int.Parse(query["id"].ToString());
            IdPaciente = id;

            if (IdPaciente == 0)
            {
                TituloPagina = "Nuevo Paciente";
            } 
            else
            {
                TituloPagina = "Paciente";
                LoadingEsVisible = true;
                await Task.Run(async () =>
                {
                    var encontrado = await _dbContext.Pacientes.FirstAsync(p => p.IdPaciente == IdPaciente);
                    PacienteDto.IdPaciente = encontrado.IdPaciente;
                    PacienteDto.Nombre = encontrado.Nombre;
                    PacienteDto.Apellido = encontrado.Apellido;
                    PacienteDto.Edad = encontrado.Edad;
                    PacienteDto.Peso = encontrado.Peso;
                    PacienteDto.Estatura = encontrado.Estatura;
                    PacienteDto.Sexo = encontrado.Sexo;
                    PacienteDto.ActividadFisica = encontrado.ActividadFisica;
                    PacienteDto.Imc = encontrado.Imc;
                    PacienteDto.GrasaCorporal = encontrado.GrasaCorporal;
                    PacienteDto.PesoIdeal = encontrado.PesoIdeal;
                    PacienteDto.GastoEnergiaDiario = encontrado.GastoEnergiaDiario;
                    PacienteDto.CategoriaImc = encontrado.CategoriaImc;

                    MainThread.BeginInvokeOnMainThread(() => { LoadingEsVisible = false; });
                });
            }
        }

        [RelayCommand]
        private async Task Guardar()
        {
            if (!ValidarEntradas())
            {
                return;
            }

            LoadingEsVisible = true;
            PacienteMensaje mensaje = new PacienteMensaje();

            await Task.Run(async () =>
            {
                if (IdPaciente == 0)
                {
                    var tbPaciente = new Paciente
                    {
                        Nombre = PacienteDto.Nombre,
                        Apellido = PacienteDto.Apellido,
                        Edad = PacienteDto.Edad,
                        Estatura = PacienteDto.Estatura,
                        Peso = PacienteDto.Peso,
                        Sexo = PacienteDto.Sexo,
                        ActividadFisica = PacienteDto.ActividadFisica,
                        Imc = CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura),
                        GrasaCorporal = CalcularGrasaCorporal(CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura), PacienteDto.Edad, PacienteDto.Sexo),
                        PesoIdeal = CalcularPesoIdeal(PacienteDto.Estatura, PacienteDto.Sexo),
                        GastoEnergiaDiario = CalcularTDEE(PacienteDto.Estatura, PacienteDto.Sexo, PacienteDto.Peso, PacienteDto.Edad, ObtenerValorActFisica(PacienteDto.ActividadFisica)),
                        CategoriaImc = ObtenerClasificacionImc(CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura))
                    };

                    _dbContext.Pacientes.Add(tbPaciente);
                    await _dbContext.SaveChangesAsync();
                    PacienteDto.IdPaciente = tbPaciente.IdPaciente;
                    mensaje = new PacienteMensaje()
                    {
                        EsCrear = true,
                        PacienteDto = PacienteDto
                    };
                }
                else
                {
                    var encontrado = await _dbContext.Pacientes.FirstAsync(p => p.IdPaciente == IdPaciente);
                    encontrado.Nombre = PacienteDto.Nombre;
                    encontrado.Apellido = PacienteDto.Apellido;
                    encontrado.Edad = PacienteDto.Edad;
                    encontrado.Peso = PacienteDto.Peso;
                    encontrado.Estatura = PacienteDto.Estatura;
                    encontrado.Sexo = PacienteDto.Nombre;
                    encontrado.ActividadFisica = PacienteDto.ActividadFisica;
                    encontrado.Imc = CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura);
                    encontrado.GrasaCorporal = CalcularGrasaCorporal(CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura), PacienteDto.Edad, PacienteDto.Sexo);
                    encontrado.PesoIdeal = CalcularPesoIdeal(PacienteDto.Estatura, PacienteDto.Sexo);
                    encontrado.GastoEnergiaDiario = CalcularTDEE(PacienteDto.Estatura, PacienteDto.Sexo, PacienteDto.Peso, PacienteDto.Edad, ObtenerValorActFisica(PacienteDto.ActividadFisica));
                    encontrado.CategoriaImc = ObtenerClasificacionImc(CalcularIMC(PacienteDto.Peso, PacienteDto.Estatura));

                    await _dbContext.SaveChangesAsync();

                    mensaje = new PacienteMensaje()
                    {
                        EsCrear = false,
                        PacienteDto = PacienteDto
                    };
                }
                MainThread.BeginInvokeOnMainThread(async() =>
                {
                    LoadingEsVisible = false;
                    WeakReferenceMessenger.Default.Send(new PacienteMensajeria(mensaje));
                    await Shell.Current.Navigation.PopAsync();
                });
            });
        }

        private double CalcularIMC(double peso, int estatura)
        {
            double estaturaEnMetros = estatura / 100.0;
            double imc = peso / Math.Pow(estaturaEnMetros, 2);
            return Math.Round(imc, 1);
        }

        private double CalcularGrasaCorporal(double imc, int edad, string sexo)
        {
            int valorSexo = 0;
            if (sexo.Equals("Masculino"))
            {
                valorSexo = 1;
            }
            double porcentaje = (1.2 * imc) + (0.23 * edad) - (10.8 * valorSexo) - 5.4;
            return Math.Round(porcentaje, 1);
        }

        private double CalcularPesoIdeal(int estatura, string sexo)
        {
            if (sexo.Equals("Masculino"))
            {
                double pesoIdeal = estatura - 100 - ((estatura - 150) / 4);
                return Math.Round(pesoIdeal, 1);
            }
            else
            {
                double pesoIdeal = estatura - 100 - ((estatura - 150) / 2.5);
                return Math.Round(pesoIdeal, 1);
            }
        }

        private double CalcularTDEE(int estatura, string sexo, double peso, int edad, double actividadFisica)
        {
            double bmr = 0;
            if (sexo.Equals("Masculino"))
            {
                bmr = (estatura * 6.25) + (peso * 9.99) - (edad * 4.92) + 5;
            }
            else
            {
                bmr = (estatura * 6.25) + (peso * 9.99) - (edad * 4.92) - 161;
            }

            double tdee = bmr * actividadFisica;
            return Math.Round(tdee, 1);
        }

        private double ObtenerValorActFisica(string actividadFisica)
        {
            if (actividadFisica.Equals("Rara vez"))
            {
                return 1.2;
            } 
            else if (actividadFisica.Equals("1 a 3 días por semana"))
            {
                return 1.375;
            }
            else if (actividadFisica.Equals("3 a 5 días por semana"))
            {
                return 1.55;
            }
            else if (actividadFisica.Equals("6 a 7 días por semana"))
            {
                return 1.725;
            }
            else if (actividadFisica.Equals("Diario"))
            {
                return 1.9;
            }
            else
            {
                return 0;
            }
        }
    
        private bool ValidarEntradas()
        {
            ErrorNombre = string.IsNullOrWhiteSpace(PacienteDto.Nombre) ? "El nombre es obligatorio" : null;
            ErrorApellido = string.IsNullOrWhiteSpace(PacienteDto.Apellido) ? "El apellido es obligatorio" : null;

            ErrorEdad = PacienteDto.Edad <= 0 ? "Ingresa una edad válida" : null;
            ErrorPeso = PacienteDto.Peso <= 0 ? "Ingresa un peso valido" : null;
            ErrorEstatura = PacienteDto.Estatura <= 0 ? "Ingresa una estatura valida" : null;
            ErrorSexo = string.IsNullOrWhiteSpace(PacienteDto.Sexo) ? "Selecciona una opción" : null;

            ErrorActFisica = string.IsNullOrWhiteSpace(PacienteDto.ActividadFisica) ? "Selecciona una opción" : null;

            return ErrorNombre == null && ErrorApellido == null && ErrorEdad == null &&
                   ErrorPeso == null && ErrorEstatura == null && ErrorSexo == null && ErrorActFisica == null;
        }

        private string ObtenerClasificacionImc(double imc)
        {
            if (imc < 16)
                return "Delgadez Severa";
            else if (imc >= 16 && imc < 17)
                return "Delgadez Moderada";
            else if (imc >= 17 && imc < 18.5)
                return "Delgadez Aceptable";
            else if (imc >= 18.5 && imc < 25)
                return "Peso Normal";
            else if (imc >= 25 && imc < 30)
                return "Sobrepeso";
            else if (imc >= 30 && imc < 35)
                return "Obesidad Tipo I";
            else if (imc >= 35 && imc < 40)
                return "Obesidad Tipo II";
            else
                return "Obesidad Tipo III";
        }
    }
}
