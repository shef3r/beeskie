﻿using Bluesky.NET.ApiClients;
using Bluesky.NET.Models;
using BlueskyClient.Caches;
using BlueskyClient.Constants;
using BlueskyClient.Services;
using BlueskyClient.Tools;
using BlueskyClient.Tools.Uwp;
using BlueskyClient.ViewModels;
using BlueskyClient.Views;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Extensions.DependencyInjection;
using JeniusApps.Common.Settings;
using JeniusApps.Common.Settings.Uwp;
using JeniusApps.Common.Tools;
using JeniusApps.Common.Tools.Uwp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

#nullable enable

namespace BlueskyClient;

partial class App
{
    private IServiceProvider? _serviceProvider;

    public static IServiceProvider Services
    {
        get
        {
            IServiceProvider? serviceProvider = ((App)Current)._serviceProvider;

            if (serviceProvider is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The service provider is not initialized");
            }

            return serviceProvider;
        }
    }


    /// <summary>
    /// Builds the <see cref="IServiceProvider"/> instance with the required services.
    /// </summary>
    /// <param name="appsettings">The <see cref="IAppSettings"/> instance to use, if available.</param>
    /// <returns>The resulting service provider.</returns>
    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection collection = new();
        ConfigureServices(collection);

        collection.AddKeyedSingleton<INavigator, Navigator>(NavigationConstants.RootNavigatorKey, (serviceProvider, key) =>
        {
            return new Navigator(new Dictionary<string, Type>
            {
                { NavigationConstants.ShellPage, typeof(ShellPage) },
                { NavigationConstants.SignInPage, typeof(SignInPage) },
            });
        });

        collection.AddKeyedSingleton<INavigator, Navigator>(NavigationConstants.ContentNavigatorKey, (serviceProvider, key) =>
        {
            return new Navigator(new Dictionary<string, Type>
            {
                { NavigationConstants.HomePage, typeof(HomePage) },
                { NavigationConstants.NotificationsPage, typeof(NotificationsPage) },
            });
        });

        collection.AddTransient((serviceProvider) =>
        {
            return new SignInPageViewModel(
                serviceProvider.GetRequiredService<IAuthenticationService>(),
                serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.RootNavigatorKey),
                serviceProvider.GetRequiredService<IUserSettings>());
        });

        collection.AddTransient((serviceProvider) =>
        {
            return new ShellPageViewModel(
                serviceProvider.GetRequiredKeyedService<INavigator>(NavigationConstants.ContentNavigatorKey),
                serviceProvider.GetRequiredService<IProfileService>());
        });

        collection.AddSingleton<IUserSettings>(_ => new LocalSettings(UserSettingsConstants.Defaults));

        IServiceProvider provider = collection.BuildServiceProvider();
        return provider;
    }

    /// <summary>
    /// Configures services used by the app.
    /// </summary>
    /// <param name="services">The target <see cref="IServiceCollection"/> instance to register services with.</param>
    [Singleton(typeof(BlueskyApiClient), typeof(IBlueskyApiClient))]
    [Singleton(typeof(AuthenticationService), typeof(IAuthenticationService))]
    [Singleton(typeof(TimelineService), typeof(ITimelineService))]
    [Singleton(typeof(FeedItemViewModelFactory), typeof(IFeedItemViewModelFactory))]
    [Singleton(typeof(NotificationViewModelFactory), typeof(INotificationViewModelFactory))]
    [Singleton(typeof(SecureCredentialStorage), typeof(ISecureCredentialStorage))]
    [Singleton(typeof(NotificationsService), typeof(INotificationsService))]
    [Singleton(typeof(ProfileCache), typeof(ICache<Author>))]
    [Singleton(typeof(ProfileService), typeof(IProfileService))]
    [Transient(typeof(HomePageViewModel))]
    [Transient(typeof(NotificationsPageViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}