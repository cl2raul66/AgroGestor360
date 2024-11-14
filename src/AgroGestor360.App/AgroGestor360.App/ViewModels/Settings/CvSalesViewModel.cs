using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class CvSalesViewModel : ObservableValidator
{
    readonly IReconciliationService reconciliationServ;

    public CvSalesViewModel(IReconciliationService reconciliationService)
    {
        reconciliationServ = reconciliationService;
    }

    [ObservableProperty]
    List<string> frecuency;
}
