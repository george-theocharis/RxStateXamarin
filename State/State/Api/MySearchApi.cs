using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace State.Api
{
    public class MySearchApi : ISearchApi
    {
        public IObservable<List<string>> SearchFor(string query)
        {
            return DelayThisChain().ToObservable()
                .Select(x => new Random().Next())
                .SelectMany(x =>
                    {
                        if (x % 2 == 0)
                            return Observable.Return(new List<string> { query });
                        else
                            return Observable.Return(new List<string>());
                    }
                );
                
        }

        Task DelayThisChain()
        {
            return Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}