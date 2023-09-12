﻿using Functionland.FxBlox.Client.Core.Models;

namespace Functionland.FxBlox.Client.Core.Services.Contracts;

public interface IBloxLibp2pClient : IDisposable
{
    BloxDevice Device { get; }
    void Initialize(BloxDevice device);
    Task<bool> CheckConnectionAsync(CancellationToken cancellationToken);

}