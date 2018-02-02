using System;
using System.Collections.Generic;
using System.Linq;

namespace State.SearchView
{
    public abstract class SearchViewState
    {
        public sealed class SearchNotStartedYet : SearchViewState { }
        public sealed class Loading : SearchViewState { }
        public sealed class Error : SearchViewState
        {
            public readonly Exception _exception;

            public  Exception Exception { get {
                    return _exception;
                } }

            public Error(Exception error)
            {
               _exception = error;
            }
        }
        public sealed class Result : SearchViewState
        {
            private readonly List<string> queries;

            public Result(List<string> queries)
            {
                this.queries = queries;
            }

            public string Item { get { return queries?.FirstOrDefault(); } }
        }
        public sealed class EmptyResult : SearchViewState { }
       
    }
}