﻿using Org.BouncyCastle.Crypto.Operators;
using WalletConnectSharp.Sign.Models;

namespace Functionland.FxBlox.Client.Core.Pages;

public partial class SettingsPage
{
    private bool _applyAnimation = false;

    [AutoInject] private ThemeInterop ThemeInterop = default!;
    [AutoInject] private IJavaScriptWalletService WalletService = default!;

    private FxTheme DesiredTheme { get; set; }

    private string? CurrentTheme { get; set; }
    private string? CurrentVersion { get; set; }

    private string transaction;
    private bool SucessFullTransfer = false;
    private int _counter = 0;
    private const int MaxCount = 7;

    protected override async Task OnInitAsync()
    {
        AppStateStore.CurrentPagePath = "settings";
        GoBackService.SetState(Task () =>
        {
            UpdateBackButtonDeviceBehavior();
            return Task.CompletedTask;
        }, true, false);

        DesiredTheme = await ThemeInterop.GetThemeAsync();

        if (DesiredTheme == FxTheme.Dark)
            CurrentTheme = Localizer.GetString(nameof(AppStrings.Night));
        else if (DesiredTheme == FxTheme.Light)
            CurrentTheme = Localizer.GetString(nameof(AppStrings.Day));
        else
            CurrentTheme = Localizer.GetString(nameof(AppStrings.System));

        GetAppVersion();
    }

    private async Task ConnectToWallet()
    {
        await WalletService.ConnectAsync(BlockchainNetwork.EthereumTestnet);
    }

    private async Task TransferMoney()
    {
        transaction = await WalletService.TransferSomeMoneyAsync();
        if (!string.IsNullOrEmpty(transaction))
        {
            SucessFullTransfer = true;
        }
    }

    private async Task TestSignMessage()
    {
        var sign = await WalletService.SignMessage("Sign this message");
    }

    private async Task OpenTransferTransaction()
    {
        if (string.IsNullOrEmpty(transaction)) return;

        var session = Preferences.Get("sessionStruct", string.Empty);
        var sessionStruct = Newtonsoft.Json.JsonConvert.DeserializeObject<SessionStruct>(session);
        var currentWallet = WalletService.GetCurrentAddress(sessionStruct, "eip155");

        var chainId = currentWallet.ChainId;
        Uri uri = null;

        if (chainId.EndsWith("1"))
        {
            uri = new Uri($"https://etherscan.io/tx/{transaction}");
        }
        else if (chainId.EndsWith("5"))
        {
            uri = new Uri($"https://goerli.etherscan.io/tx/{transaction}");
        }
       
        if(uri == null) return;

        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

    }

    private void UpdateBackButtonDeviceBehavior()
    {
        NavigationManager.NavigateTo("mydevice", false, true);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _applyAnimation = true;

#if BlazorHybrid
            var preferredCultureCookie = Preferences.Get(".AspNetCore.Culture", null);
#else
        var preferredCultureCookie = await JSRuntime.InvokeAsync<string?>("window.App.getCookie", ".AspNetCore.Culture");
#endif
            SelectedCulture = CultureInfoManager.GetCurrentCulture(preferredCultureCookie);

            StateHasChanged();
        }
    }


    public void HandleTitleClick()
    {
        if (_counter >= MaxCount && AppStateStore.IsAvailableForTest) return;

        _counter++;

        if (_counter >= MaxCount)
        {
            AppStateStore.IsAvailableForTest = true;
        }
    }

    private void GetAppVersion()
    {
#if BlazorHybrid
        CurrentVersion = AppInfo.Current.VersionString;
#endif
    }

    private string? SelectedCulture;

    private async Task OnCultureChanged()
    {
        var cultureCookie = $"c={SelectedCulture}|uic={SelectedCulture}";

#if BlazorHybrid
        Preferences.Set(".AspNetCore.Culture", cultureCookie);
#else
        await JSRuntime.InvokeVoidAsync("window.App.setCookie", ".AspNetCore.Culture", cultureCookie, 30 * 24 * 3600);
#endif

        NavigationManager.ForceReload();
    }

    private static List<BitDropdownItem> GetCultures() =>
        CultureInfoManager.SupportedCultures.Select(sc => new BitDropdownItem { Value = sc.code, Text = sc.name }).ToList();

}