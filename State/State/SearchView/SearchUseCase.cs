using State.Api;
using System;
using System.Reactive.Linq;

namespace State.SearchView
{
    public class SearchUseCase
    {
        ISearchApi _searchApi;

        public SearchUseCase(ISearchApi searchApi)
        {
            _searchApi = searchApi;
        }

        public IObservable<SearchViewState> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
                return Observable.Return(new SearchViewState.SearchNotStartedYet());

            return _searchApi.SearchFor(query)
                      .SelectMany(queries =>
                      {
                          if (queries.Count == 0)
                              return Observable.Return<SearchViewState>(new SearchViewState.EmptyResult());
                      
                          else return Observable.Return<SearchViewState>(new SearchViewState.Result(queries));
                      })
                      .StartWith(new SearchViewState.Loading())
                      .Catch<SearchViewState, Exception>(error => Observable.Return(new SearchViewState.Error(error)));
        }
    }
}