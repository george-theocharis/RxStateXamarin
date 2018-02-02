using System;
using System.Collections.Generic;

namespace State.Api
{
    public interface ISearchApi
    {
        IObservable<List<string>> SearchFor(string query);
    }
}