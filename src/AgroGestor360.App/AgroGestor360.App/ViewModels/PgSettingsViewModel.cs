using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class PgSettingsViewModel : ObservableObject
{
    [RelayCommand]
    async Task GoToSettings() => await Shell.Current.GoToAsync("..", true);
}
