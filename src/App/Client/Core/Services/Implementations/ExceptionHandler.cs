﻿using Functionland.FxBlox.Shared.Exceptions;
using Functionland.FxBlox.Shared.Resources;
using System.Diagnostics;

namespace Functionland.FxBlox.Client.Core.Services.Implementations
{
    public partial class ExceptionHandler : IExceptionHandler
    {
        [AutoInject] IStringLocalizer<AppStrings> _localizer = default!;

        public void Handle(Exception exception, IDictionary<string, object?>? parameters = null)
        {
            if (exception is UnauthorizedException)
            {
                return;
            }

#if DEBUG
        string exceptionMessage = (exception as KnownException)?.Message ?? exception.ToString();
        _ = MessageBox.Show(exceptionMessage, _localizer[nameof(AppStrings.Error)]);
        Console.WriteLine(exceptionMessage);
        Debugger.Break();
#else
            if (exception is KnownException knownException)
            {
                _ = MessageBox.Show(knownException.Message, _localizer[nameof(AppStrings.Error)]);
            }
            else
            {
                _ = MessageBox.Show(_localizer[nameof(AppStrings.UnknownException)], _localizer[nameof(AppStrings.Error)]);
            }
#endif
        }
    }
}