﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Unigram.Logs;
using Unigram.Navigation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Unigram.Services.ViewService
{
    public interface IViewService
    {
        ///<summary>
        /// Creates and opens new secondary view        
        /// </summary>
        /// <param name="page">Type of page to automatically navigate</param>
        /// <param name="parameter">Parameter that will be passed to NavigationService with the page</param>
        /// <param name="title">Title that will be displayed for new view. If <code>null</code> - current view's title will be used</param>
        /// <param name="size">Anchor size for newly created view</param>        
        /// <returns><see cref="ViewLifetimeControl"/> object that is associated to newly created view. Use it to subscribe to <code>Released</code> event to close window manually.
        /// It won't not be called before all previously started async operations on <see cref="CoreDispatcher"/> complete. <remarks>DO NOT call operations on Dispatcher after this</remarks></returns>
        Task<ViewLifetimeControl> OpenAsync(Type page, object parameter = null, string title = null, ViewSizePreference size = ViewSizePreference.UseHalf, int session = 0, string id = "0");

        Task<ViewLifetimeControl> OpenAsync(ViewServiceParams parameters);
    }

    public class ViewServiceParams
    {
        public ApplicationViewMode ViewMode { get; set; } = ApplicationViewMode.Default;

        public double Width { get; set; }
        public double Height { get; set; }

        public Func<ViewLifetimeControl, UIElement> Content { get; set; }

        public string PersistentId { get; set; }
    }

    public sealed class ViewService : IViewService
    {
        internal static void OnWindowCreated()
        {
            var view = CoreApplication.GetCurrentView();
            if (!view.IsMain && !view.IsHosted)
            {
                var control = ViewLifetimeControl.GetForCurrentView();
                //This one time it should be made manually, as after Consolidate event fires the inner reference number should become zero
                control.StartViewInUse();
                //This is necessary to not make control.StartViewInUse()/control.StopViewInUse() manually on each and every async call. Facade will do it for you
                SynchronizationContext.SetSynchronizationContext(new SecondaryViewSynchronizationContextDecorator(control,
                    SynchronizationContext.Current));
            }
        }

        public async Task<ViewLifetimeControl> OpenAsync(ViewServiceParams parameters)
        {
            if (_windows.TryGetValue(parameters.PersistentId, out IDispatcherContext value))
            {
                var newControl = await value.DispatchAsync(async () =>
                {
                    var control = ViewLifetimeControl.GetForCurrentView();
                    var newAppView = ApplicationView.GetForCurrentView();

                    var preferences = ViewModePreferences.CreateDefault(parameters.ViewMode);
                    preferences.CustomSize = new Size(parameters.Width, parameters.Height);

                    await ApplicationViewSwitcher.TryShowAsViewModeAsync(newAppView.Id, parameters.ViewMode, preferences);

                    return control;
                }).ConfigureAwait(false);
                return newControl;
            }
            else
            {
                //if (ApiInformation.IsPropertyPresent("Windows.UI.ViewManagement.ApplicationView", "PersistedStateId"))
                //{
                //    try
                //    {
                //        ApplicationView.ClearPersistedState("Calls");
                //    }
                //    catch { }
                //}

                var newView = CoreApplication.CreateNewView();
                var dispatcher = new DispatcherContext(newView.Dispatcher);
                _windows[parameters.PersistentId] = dispatcher;

                var newControl = await dispatcher.DispatchAsync(async () =>
                {
                    var newWindow = Window.Current;
                    newWindow.Closed += (s, args) =>
                    {
                        _windows.TryRemove(parameters.PersistentId, out _);
                    };
                    newWindow.CoreWindow.Closed += (s, args) =>
                    {
                        _windows.TryRemove(parameters.PersistentId, out _);
                    };

                    var newAppView = ApplicationView.GetForCurrentView();
                    newAppView.Consolidated += (s, args) =>
                    {
                        _windows.TryRemove(parameters.PersistentId, out _);
                        newWindow.Close();
                    };

                    if (ApiInformation.IsPropertyPresent("Windows.UI.ViewManagement.ApplicationView", "PersistedStateId"))
                    {
                        newAppView.PersistedStateId = "Calls";
                    }

                    var control = ViewLifetimeControl.GetForCurrentView();
                    control.Released += (s, args) =>
                    {
                        newWindow.Close();
                        _windows.TryRemove(parameters.PersistentId, out _);
                    };

                    newWindow.Content = parameters.Content(control);
                    newWindow.Activate();

                    var preferences = ViewModePreferences.CreateDefault(parameters.ViewMode);
                    preferences.CustomSize = new Size(parameters.Width, parameters.Height);

                    await ApplicationViewSwitcher.TryShowAsViewModeAsync(newAppView.Id, parameters.ViewMode, preferences);
                    newAppView.TryResizeView(new Size(parameters.Width, parameters.Height));

                    return control;
                }).ConfigureAwait(false);
                return newControl;
            }
        }

        public async Task<ViewLifetimeControl> OpenAsync(Type page, object parameter = null, string title = null,
            ViewSizePreference size = ViewSizePreference.UseHalf, int session = 0, string id = "0")
        {
            Logger.Info($"Page: {page}, Parameter: {parameter}, Title: {title}, Size: {size}");

            var currentView = ApplicationView.GetForCurrentView();
            title = title ?? currentView.Title;









            if (parameter != null && _windows.TryGetValue(parameter, out IDispatcherContext value))
            {
                var newControl = await value.DispatchAsync(async () =>
                {
                    var control = ViewLifetimeControl.GetForCurrentView();
                    var newAppView = ApplicationView.GetForCurrentView();

                    await ApplicationViewSwitcher
                        .SwitchAsync(newAppView.Id, currentView.Id, ApplicationViewSwitchingOptions.Default);

                    return control;
                }).ConfigureAwait(false);
                return newControl;
            }
            else
            {
                //if (ApiInformation.IsPropertyPresent("Windows.UI.ViewManagement.ApplicationView", "PersistedStateId"))
                //{
                //    try
                //    {
                //        ApplicationView.ClearPersistedState("Calls");
                //    }
                //    catch { }
                //}

                var newView = CoreApplication.CreateNewView();
                var dispatcher = new DispatcherContext(newView.Dispatcher);

                if (parameter != null)
                {
                    _windows[parameter] = dispatcher;
                }

                var bounds = Window.Current.Bounds;

                var newControl = await dispatcher.DispatchAsync(async () =>
                {
                    var newWindow = Window.Current;
                    var newAppView = ApplicationView.GetForCurrentView();
                    newAppView.Title = title;

                    if (ApiInformation.IsPropertyPresent("Windows.UI.ViewManagement.ApplicationView", "PersistedStateId"))
                    {
                        newAppView.PersistedStateId = "Floating";
                    }

                    var control = ViewLifetimeControl.GetForCurrentView();
                    control.Released += (s, args) =>
                    {
                        if (parameter != null)
                        {
                            _windows.TryRemove(parameter, out IDispatcherContext _);
                        }

                        newWindow.Close();
                    };

                    var nav = BootStrapper.Current.NavigationServiceFactory(BootStrapper.BackButton.Ignore, BootStrapper.ExistingContent.Exclude, session, id, false);
                    control.NavigationService = nav;
                    nav.Navigate(page, parameter);
                    newWindow.Content = BootStrapper.Current.CreateRootElement(nav);
                    newWindow.Activate();

                    await ApplicationViewSwitcher
                        .TryShowAsStandaloneAsync(newAppView.Id, ViewSizePreference.Default, currentView.Id, size);
                    //newAppView.TryResizeView(new Windows.Foundation.Size(360, bounds.Height));
                    newAppView.TryResizeView(new Windows.Foundation.Size(360, 640));

                    return control;
                }).ConfigureAwait(false);
                return newControl;
            }
        }

        private readonly ConcurrentDictionary<object, IDispatcherContext> _windows = new ConcurrentDictionary<object, IDispatcherContext>();
    }
}
