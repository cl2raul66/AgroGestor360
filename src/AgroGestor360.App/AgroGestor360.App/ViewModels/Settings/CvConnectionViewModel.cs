using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings;

public partial class CvConnectionViewModel : ObservableObject
{
    [RelayCommand]
    async Task ShowSetURL()
    {
        await Shell.Current.GoToAsync("", true);
    }
}
