using System;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Collections.Generic;

namespace State.SearchView
{
    public class SearchPresenter
    {
        ISearchView _view;
        SearchUseCase _searchUseCase;
        BehaviorSubject<SearchViewState> _viewState = new BehaviorSubject<SearchViewState>(new SearchViewState.SearchNotStartedYet());
        CompositeDisposable _disposables = new CompositeDisposable();

        public SearchPresenter(SearchUseCase searchUseCase)
        {
            _searchUseCase = searchUseCase;
        }

        public void AttachView(ISearchView view)
        {
            _view = view;
        }

        public void DetachView()
        {
            _view = null;
        }

        public void BindIntents()
        {
            _disposables.Add(_viewState.Subscribe(state => _view.Render(state)));

            _disposables.Add(
                _view.SearchIntent()
                     .SelectMany(query => _searchUseCase.Search(query))
                     .SubscribeOn(TaskPoolScheduler.Default)
                     .ObserveOn(SynchronizationContext.Current)
                     .Subscribe(state => _viewState.OnNext(state))
                     );
        }

        public void UnBindIntents()
        {
            _disposables.Clear();
        }

        public void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            _searchUseCase = null;
            _viewState.Dispose();
            _viewState = null;
        }
    }
}