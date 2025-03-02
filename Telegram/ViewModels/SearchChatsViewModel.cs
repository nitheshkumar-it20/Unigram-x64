﻿//
// Copyright Fela Ameghino 2015-2024
//
// Distributed under the GNU General Public License v3.0. (See accompanying
// file LICENSE or copy at https://www.gnu.org/licenses/gpl-3.0.txt)
//
using Rg.DiffUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Collections;
using Telegram.Collections.Handlers;
using Telegram.Common;
using Telegram.Navigation;
using Telegram.Services;
using Telegram.Td;
using Telegram.Td.Api;
using Telegram.Views.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telegram.ViewModels
{
    public partial class SearchChatsViewModel : ViewModelBase, IIncrementalCollectionOwner
    {
        private readonly KeyedCollection<SearchResult> _recent = new(Strings.Recent, new SearchResultDiffHandler());
        private readonly KeyedCollection<SearchResult> _chatsAndContacts1 = new(Strings.ChatsAndContacts, new SearchResultDiffHandler());
        private readonly KeyedCollection<SearchResult> _chatsAndContacts2 = new(null as string, new SearchResultDiffHandler());
        private readonly KeyedCollection<SearchResult> _globalSearch = new(Strings.GlobalSearch, new SearchResultDiffHandler());
        private readonly KeyedCollection<Message> _messages = new(Strings.SearchMessages, null);

        private readonly SearchChannelsViewModel _channels;
        private readonly SearchWebAppsViewModel _webApps;

        private readonly ChooseChatsTracker _tracker;

        private CancellationTokenSource _cancellation = new();

        private string _prevQuery;
        private string _nextOffset;

        public SearchChatsViewModel(IClientService clientService, ISettingsService settingsService, IEventAggregator aggregator)
            : base(clientService, settingsService, aggregator)
        {
            _channels = new SearchChannelsViewModel(clientService, settingsService, aggregator);
            _webApps = new SearchWebAppsViewModel(clientService, settingsService, aggregator);
            _tracker = new ChooseChatsTracker(clientService, true);
            _tracker.Options = ChooseChatsOptions.All;

            _query = new(Constants.TypingTimeout, UpdateQuery, CanUpdateQuery);
            _query.Value = string.Empty;

            TopChats = new DiffObservableCollection<Chat>(new ChatDiffHandler(), Constants.DiffOptions);
            Items = new FlatteningCollection(this, _recent, _chatsAndContacts1, _chatsAndContacts2, _globalSearch, _messages);
        }

        public ChooseChatsOptions Options
        {
            get => _tracker.Options;
            set => _tracker.Options = value;
        }

        public bool CanSendMessageToUser =>
            Options == ChooseChatsOptions.PostMessages
            || Options == ChooseChatsOptions.Contacts
            || Options == ChooseChatsOptions.InviteUsers;

        public DiffObservableCollection<Chat> TopChats { get; }

        public FlatteningCollection Items { get; }

        public FlatteningCollection ItemsView => SelectedTab switch
        {
            1 => _channels.Items,
            2 => _webApps.Items,
            _ => Items,
        };

        private readonly DebouncedProperty<string> _query;
        public string Query
        {
            get => _query;
            set
            {
                if (string.Equals(value, _query) && !string.IsNullOrEmpty(value))
                {
                    return;
                }

                SynchronizeQuery(value);

                if (SelectedTab == 1)
                {
                    _channels.Query = value;
                    _webApps.SynchronizeQuery(value);
                }
                else if (SelectedTab == 2)
                {
                    _webApps.Query = value;
                    _channels.SynchronizeQuery(value);
                }
                else
                {
                    _query.Set(value, _cancellation.Token);
                    _channels.SynchronizeQuery(value);
                    _webApps.SynchronizeQuery(value);
                }
            }
        }

        public void SynchronizeQuery(string query)
        {
            _cancellation.Cancel();
            _cancellation = new();

            IsEmpty = false;
        }

        private bool _isTopChatsVisible;
        public bool IsTopChatsVisible
        {
            get => _isTopChatsVisible && Options.AllowUserChats && SelectedTab == 0;
            set => Set(ref _isTopChatsVisible, value);
        }

        private int _selectedTab;
        public int SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (Set(ref _selectedTab, value))
                {
                    RaisePropertyChanged(nameof(ItemsView));
                    RaisePropertyChanged(nameof(IsTopChatsVisible));

                    if (value == 1)
                    {
                        _channels.Query = Query;
                    }
                    else if (value == 2)
                    {
                        _webApps.Query = Query;
                    }
                    else
                    {
                        Query = _channels.Query;
                    }
                }
            }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set => Set(ref _isEmpty, value);
        }

        private bool CanUpdateQuery(string value)
        {
            if (string.Equals(value, _prevQuery))
            {
                return false;
            }

            UpdateQueryOffline(_prevQuery = value);
            return value.Length > 0;
        }

        private async void UpdateQueryOffline(string value)
        {
            _nextOffset = null;

            _messages.Clear();
            _tracker.Clear();

            var query = value ?? string.Empty;
            var token = _cancellation.Token;

            if (string.IsNullOrEmpty(value))
            {
                _chatsAndContacts2.Clear();
                _globalSearch.Clear();

                IsTopChatsVisible = true;

                await LoadTopChatsAsync(token);
                await LoadRecentAsync(query, token);
            }
            else
            {
                IsTopChatsVisible = false;

                await LoadRecentAsync(query, token);
                await LoadChatsAndContactsPart1Async(query, token);
            }
        }

        public async void UpdateQuery(string value)
        {
            var query = value ?? string.Empty;
            var token = _cancellation.Token;

            _query.Value = query;

            await LoadChatsAndContactsPart2Async(query, token);
            await LoadGlobalSearchAsync(query, token);

            if (Options.ShowMessages)
            {
                await LoadMessagesAsync(query, token);
            }

            IsEmpty = Items.Empty();
        }

        private async Task LoadTopChatsAsync(CancellationToken cancellationToken)
        {
            var temp = new List<Chat>();

            var response = await ClientService.SendAsync(new GetTopChats(new TopChatCategoryUsers(), 30));
            if (response is Td.Api.Chats chats && !cancellationToken.IsCancellationRequested)
            {
                foreach (var chat in ClientService.GetChats(chats.ChatIds))
                {
                    temp.Add(chat);
                }
            }

            ReplaceDiff(TopChats, temp, cancellationToken);
        }

        private async Task LoadRecentAsync(string query, CancellationToken cancellationToken)
        {
            var temp = new List<SearchResult>();

            var response = await ClientService.SendAsync(new SearchRecentlyFoundChats(query, 50));
            if (response is Td.Api.Chats chats && !cancellationToken.IsCancellationRequested)
            {
                foreach (var chat in ClientService.GetChats(chats.ChatIds))
                {
                    if (_tracker.Filter(chat))
                    {
                        temp.Add(new SearchResult(ClientService, chat, query, SearchResultType.Recent, CanSendMessageToUser));
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            ReplaceDiff(_recent, temp, cancellationToken);
        }

        private Chat LoadSavedMessages(string query, CancellationToken cancellationToken)
        {
            if (ClientEx.SearchByPrefix(Strings.SavedMessages, _query))
            {
                if (ClientService.TryGetChat(ClientService.Options.MyId, out Chat chat) && !cancellationToken.IsCancellationRequested)
                {
                    return chat;
                }
            }

            return null;
        }

        private async Task LoadChatsAndContactsPart1Async(string query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(query))
            {
                _chatsAndContacts1.Clear();
                return;
            }

            var task2 = ClientService.SendAsync(new SearchChats(query, 100));
            var task3 = ClientService.SendAsync(new SearchContacts(query, 100));

            await Task.WhenAny(task2, task3);

            var temp = new List<SearchResult>();

            var response1 = LoadSavedMessages(query, cancellationToken);
            if (response1 is Chat savedMessages && !cancellationToken.IsCancellationRequested)
            {
                if (_tracker.Filter(savedMessages))
                {
                    temp.Add(new SearchResult(ClientService, savedMessages, query, SearchResultType.Chats, CanSendMessageToUser));
                }
            }

            var response2 = await task2;
            if (response2 is Td.Api.Chats chats && !cancellationToken.IsCancellationRequested)
            {
                foreach (var chat in ClientService.GetChats(chats.ChatIds))
                {
                    if (_tracker.Filter(chat))
                    {
                        temp.Add(new SearchResult(ClientService, chat, query, SearchResultType.Chats, CanSendMessageToUser));
                    }
                }
            }

            var response3 = await task3;
            if (response3 is Td.Api.Users users && !cancellationToken.IsCancellationRequested)
            {
                foreach (var user in ClientService.GetUsers(users.UserIds))
                {
                    if (_tracker.Filter(user))
                    {
                        temp.Add(new SearchResult(ClientService, user, query, SearchResultType.Contacts, CanSendMessageToUser));
                    }
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            ReplaceDiff(_chatsAndContacts1, temp, cancellationToken);
        }

        private async Task LoadChatsAndContactsPart2Async(string query, CancellationToken cancellationToken)
        {
            var response = await ClientService.SendAsync(new SearchChatsOnServer(query, 100));
            if (response is Td.Api.Chats chats && !cancellationToken.IsCancellationRequested)
            {
                var temp = new List<SearchResult>();

                foreach (var chat in ClientService.GetChats(chats.ChatIds))
                {
                    if (_tracker.Filter(chat))
                    {
                        temp.Add(new SearchResult(ClientService, chat, query, SearchResultType.ChatsOnServer, CanSendMessageToUser));
                    }
                }

                ReplaceDiff(_chatsAndContacts2, temp, cancellationToken);
            }
        }

        private async Task LoadGlobalSearchAsync(string query, CancellationToken cancellationToken)
        {
            var response = await ClientService.SendAsync(new SearchPublicChats(query));
            if (response is Td.Api.Chats chats && !cancellationToken.IsCancellationRequested)
            {
                var temp = new List<SearchResult>();

                foreach (var chat in ClientService.GetChats(chats.ChatIds))
                {
                    if (_tracker.Filter(chat))
                    {
                        temp.Add(new SearchResult(ClientService, chat, query, SearchResultType.PublicChats, CanSendMessageToUser));
                    }
                }

                ReplaceDiff(_globalSearch, temp, cancellationToken);
            }
        }

        private async Task LoadMessagesAsync(string query, CancellationToken cancellationToken)
        {
            var response = await ClientService.SendAsync(new SearchMessages(null, false, query, _nextOffset ?? string.Empty, 50, null, 0, 0));
            if (response is FoundMessages messages && !cancellationToken.IsCancellationRequested)
            {
                _nextOffset = string.IsNullOrEmpty(messages.NextOffset) ? null : messages.NextOffset;

                foreach (var message in messages.Messages)
                {
                    _messages.Add(message);
                }
            }
        }

        private async void ReplaceDiff<T>(DiffObservableCollection<T> destination, IEnumerable<T> source, CancellationToken cancellationToken)
        {
            var diff = await Task.Run(() => DiffUtil.CalculateDiff(destination, source, destination.DefaultDiffHandler, destination.DefaultOptions));

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            destination.ReplaceDiff(diff);
        }

        #region ISupportIncrementalLoading

        public async Task<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            await LoadMessagesAsync(_query.Value, _cancellation.Token);
            return new LoadMoreItemsResult { Count = 50 };
        }

        public bool HasMoreItems => _nextOffset != null && Options.ShowMessages;

        #endregion

        #region Commands

        public async void ClearRecentChats()
        {
            var confirm = await ShowPopupAsync(Strings.ClearSearchAlert, Strings.ClearSearchAlertTitle, Strings.ClearButton, Strings.Cancel, destructive: true);
            if (confirm != ContentDialogResult.Primary)
            {
                return;
            }

            _recent.Clear();
            ClientService.Send(new ClearRecentlyFoundChats());
        }

        public async void RemoveRecentChat(SearchResult result)
        {
            string message;
            if (ClientService.TryGetUser(result.Chat, out User user))
            {
                message = string.Format(Strings.ClearSearchSingleUserAlertText, user.FullName());
            }
            else
            {
                message = string.Format(Strings.ClearSearchSingleChatAlertText, ClientService.GetTitle(result.Chat));
            }

            var confirm = await ShowPopupAsync(message, Strings.ClearSearchSingleAlertTitle, Strings.ClearSearchRemove, Strings.Cancel, destructive: true);
            if (confirm != ContentDialogResult.Primary)
            {
                return;
            }

            _recent.Remove(result);
            ClientService.Send(new RemoveRecentlyFoundChat(result.Chat.Id));
        }

        public async void RemoveTopChat(Chat chat)
        {
            if (chat == null)
            {
                return;
            }

            var confirm = await ShowPopupAsync(string.Format(Strings.ChatHintsDeleteAlert, ClientService.GetTitle(chat)), Strings.ChatHintsDeleteAlertTitle, Strings.Remove, Strings.Cancel, destructive: true);
            if (confirm != ContentDialogResult.Primary)
            {
                return;
            }

            TopChats.Remove(chat);
            ClientService.Send(new RemoveTopChat(new TopChatCategoryUsers(), chat.Id));
        }

        #endregion
    }

    public partial class KeyedCollection<T> : DiffObservableCollection<T>, IKeyedCollection
    {
        public string Key { get; }

        public int Index { get; set; }

        public int TotalIndex => Index + (Key != null && Count > 0 ? 1 : 0);

        public int TotalCount => Count + (Key != null && Count > 0 ? 1 : 0);

        public KeyedCollection(string key, IDiffHandler<T> handler)
            : base(handler, Constants.DiffOptions)
        {
            Key = key;
        }

        public KeyedCollection(string key, IEnumerable<T> source, IDiffHandler<T> handler)
            : base(source, handler, Constants.DiffOptions)
        {
            Key = key;
        }

        public KeyedCollection(IGrouping<string, T> source, IDiffHandler<T> handler)
            : base(source, handler, Constants.DiffOptions)
        {
            Key = source.Key;
        }

        public override string ToString()
        {
            return Key ?? base.ToString();
        }
    }
}
