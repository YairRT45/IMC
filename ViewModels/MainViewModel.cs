using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore;
using IMCApp.DataAccess;
using IMCApp.DTOs;
using IMCApp.Utilidades;
using IMCApp.Modelos;
using IMCApp.Views;
using System.Collections.ObjectModel;

namespace IMCApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PacienteDBContext _dbContext;

        [ObservableProperty]
        private ObservableCollection<PacienteDTO> listaPaciente = new ObservableCollection<PacienteDTO>();

        public MainViewModel(PacienteDBContext context)
        {
            _dbContext = context;

            MainThread.BeginInvokeOnMainThread(new Action(async() => await Obtener()));

            WeakReferenceMessenger.Default.Register<PacienteMensajeria>(this, (r, m) =>
            {
                PacienteMensajeRecibido(m.Value);
            });
        }

        public async Task Obtener()
        {
            var lista = await _dbContext.Pacientes.ToListAsync();
            if (lista.Any())
            {
                foreach(var item in lista)
                {
                    ListaPaciente.Add(new PacienteDTO
                    {
                        IdPaciente = item.IdPaciente,
                        Nombre = item.Nombre,
                        Apellido = item.Apellido,
                        Edad = item.Edad,
                        Peso = item.Peso,
                        Estatura = item.Estatura,
                        Sexo = item.Sexo,
                        ActividadFisica = item.ActividadFisica,
                        Imc = item.Imc,
                        GrasaCorporal = item.GrasaCorporal,
                        PesoIdeal = item.PesoIdeal,
                        GastoEnergiaDiario = item.GastoEnergiaDiario,
                        CategoriaImc = item.CategoriaImc
                    });
                }
            }
        }

        private void PacienteMensajeRecibido(PacienteMensaje pacienteMensaje)
        {
            var pacienteDto = pacienteMensaje.PacienteDto;

            if (pacienteMensaje.EsCrear)
            {
                ListaPaciente.Add(pacienteDto);
            }
            else
            {
                var encontrado = ListaPaciente.First(p => p.IdPaciente == pacienteDto.IdPaciente);

                encontrado.Nombre = pacienteDto.Nombre;
                encontrado.Apellido = pacienteDto.Apellido;
                encontrado.Edad = pacienteDto.Edad;
                encontrado.Peso = pacienteDto.Peso;
                encontrado.Estatura = pacienteDto.Estatura;
                encontrado.Sexo = pacienteDto.Sexo;
                encontrado.ActividadFisica = pacienteDto.ActividadFisica;
                encontrado.Imc = pacienteDto.Imc;
                encontrado.GrasaCorporal = pacienteDto.GrasaCorporal;
                encontrado.PesoIdeal = pacienteDto.PesoIdeal;
                encontrado.GastoEnergiaDiario = pacienteDto.GastoEnergiaDiario;
            }
        }

        [RelayCommand]
        private async Task Crear()
        {
            var uri = $"{nameof(PacientePage)}?id=0";
            await Shell.Current.GoToAsync(uri);
        }

        [RelayCommand]
        private async Task Editar(PacienteDTO pacienteDto)
        {
            var uri = $"{nameof(PacientePage)}?id={pacienteDto.IdPaciente}";
            await Shell.Current.GoToAsync(uri);
        }

        [RelayCommand]
        private async Task VerPaciente(PacienteDTO pacienteDto)
        {
            var uri = $"{nameof(DatosPacientePage)}?id={pacienteDto.IdPaciente}";
            await Shell.Current.GoToAsync(uri);
        }

        [RelayCommand]
        private async Task Eliminar(PacienteDTO pacienteDto)
        {
            bool answer = await Shell.Current.DisplayAlert("Mensaje", "¿Desea eliminar el paciente?", "Sí", "No");

            if (answer)
            {
                var encontrado = await _dbContext.Pacientes.FirstAsync(p => p.IdPaciente == pacienteDto.IdPaciente);
                _dbContext.Pacientes.Remove(encontrado);
                await _dbContext.SaveChangesAsync();
                ListaPaciente.Remove(pacienteDto);
            }
        }
    }
}
