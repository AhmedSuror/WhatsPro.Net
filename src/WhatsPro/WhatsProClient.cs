using System;
using WhatsPro.Authentication;
using WhatsPro.Clients;
using WhatsPro.Dashboard;
using WhatsPro.Groups;
using WhatsPro.Http;
using WhatsPro.Messages;
using WhatsPro.Sessions;

namespace WhatsPro;

/// <summary>
/// The main entry point for interacting with the Whats-Pro API.
/// </summary>
public class WhatsProClient : IDisposable
{
    private readonly WhatsProOptions _options;
    private readonly WhatsProHttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// Gets the authentication and profile operations.
    /// </summary>
    public AuthOperations Auth { get; }

    /// <summary>
    /// Gets the dashboard analytics operations.
    /// </summary>
    public DashboardOperations Dashboard { get; }

    /// <summary>
    /// Gets the clients management operations.
    /// </summary>
    public ClientOperations Clients { get; }

    /// <summary>
    /// Gets the groups management operations.
    /// </summary>
    public GroupOperations Groups { get; }

    /// <summary>
    /// Gets the sessions management operations.
    /// </summary>
    public SessionOperations Sessions { get; }

    /// <summary>
    /// Gets the messages management operations.
    /// </summary>
    public MessageOperations Messages { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhatsProClient"/> class.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
    public WhatsProClient(WhatsProOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        ValidateOptions(_options);
        
        _httpClient = new WhatsProHttpClient(_options);
        Auth = new AuthOperations(_httpClient);
        Dashboard = new DashboardOperations(_httpClient);
        Clients = new ClientOperations(_httpClient);
        Groups = new GroupOperations(_httpClient);
        Sessions = new SessionOperations(_httpClient);
        Messages = new MessageOperations(_httpClient);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WhatsProClient"/> class using an action to configure options.
    /// </summary>
    /// <param name="configure">The action to configure options.</param>
    public WhatsProClient(Action<WhatsProOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        _options = new WhatsProOptions();
        configure(_options);
        ValidateOptions(_options);

        _httpClient = new WhatsProHttpClient(_options);
        Auth = new AuthOperations(_httpClient);
        Dashboard = new DashboardOperations(_httpClient);
        Clients = new ClientOperations(_httpClient);
        Groups = new GroupOperations(_httpClient);
        Sessions = new SessionOperations(_httpClient);
        Messages = new MessageOperations(_httpClient);
    }

    private static void ValidateOptions(WhatsProOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new ArgumentException("BaseUrl must be provided in WhatsProOptions.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.Email))
            throw new ArgumentException("Email must be provided in WhatsProOptions.", nameof(options));
        if (string.IsNullOrWhiteSpace(options.Password))
            throw new ArgumentException("Password must be provided in WhatsProOptions.", nameof(options));
    }

    /// <summary>
    /// Disposes the underlying resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the underlying resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
            _disposed = true;
        }
    }
}
