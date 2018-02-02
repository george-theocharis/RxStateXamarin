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
        private CompositeDisposable _disposables;

        BehaviorSubject<SearchViewState> _intents = new BehaviorSubject<SearchViewState>(new SearchViewState.SearchNotStartedYet());

        IObservable<SearchViewState> AllIntents
        {
            get { return _intents.AsObservable(); }
        }

        public SearchPresenter(SearchUseCase searchUseCase)
        {
            _searchUseCase = searchUseCase;
        }

        public void AttachView(ISearchView view)
        {
            _view = view;
        }

        public void BindIntents()
        {
            _disposables = new CompositeDisposable();

            var searchAction = Intent(_view.SearchIntent())
                .SelectMany(query => _searchUseCase.Search(query))
                .SubscribeOn(TaskPoolScheduler.Default)
                .ObserveOn(SynchronizationContext.Current);

      
            _disposables.Add(searchAction.Subscribe(_intents));

           _disposables.Add( AllIntents.Subscribe(state =>
            _view.Render(state)));
        }

        private IObservable<T> Intent<T>(IObservable<T> observable)
        {
            var subject = new Subject<T>();

            _disposables.Add(observable.Subscribe(subject));

            _disposables.Add(subject);

            return subject.AsObservable();
        }

        public void UnBindIntents()
        {
            _disposables.Dispose();
            _disposables = null;
        }

        public void OnDestroy()
        {
            _searchUseCase = null;
            _intents.Dispose();
            _intents = null;
        }

        public void DetachView()
        {
            _view = null;
        }

    }
}