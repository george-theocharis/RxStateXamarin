using System;

namespace State.SearchView
{
    public interface ISearchView 
    {
        IObservable<string> SearchIntent();
        void Render(SearchViewState state);
    }
}