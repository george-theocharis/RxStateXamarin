using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using State.SearchView;
using System.Reactive.Linq;
using static Android.Widget.SearchView;
using static State.SearchView.SearchViewState;
using State.Api;
using Java.Lang;

namespace State
{
    [Activity(Label = "State", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISearchView
    {
        Android.Widget.SearchView SearchView;
        TextView StateIndicator;
        private SearchFragment retainedFragment;

        public Button TryNewScreen { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var builder = new StrictMode.VmPolicy.Builder();
            var policy = builder.DetectActivityLeaks().PenaltyLog().Build();
            StrictMode.SetVmPolicy(policy);

            SetContentView(Resource.Layout.Main);

            SearchView = FindViewById<Android.Widget.SearchView>(Resource.Id.SearchView);
            StateIndicator = FindViewById<TextView>(Resource.Id.StateIndicator);
            TryNewScreen = FindViewById<Button>(Resource.Id.OpenNewActivity);

            TryNewScreen.Click += GoToSecondScreen;

            retainedFragment = FragmentManager.FindFragmentByTag("SearchPresenter") as SearchFragment;

            if(retainedFragment == null)
            {
                retainedFragment = new SearchFragment();
                FragmentManager.BeginTransaction().Add(retainedFragment, "SearchPresenter").Commit();
                retainedFragment.Presenter = new SearchPresenter(new SearchUseCase(new MySearchApi())); 
            }

            retainedFragment.Presenter.AttachView(this);
            retainedFragment.Presenter.BindIntents();
        }

        private void GoToSecondScreen(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(SecondActivity)));
            Finish();
        }

        protected override void OnPause()
        {
            base.OnPause();

            if(IsFinishing)
                FragmentManager.BeginTransaction().Remove(retainedFragment).Commit();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TryNewScreen.Click -= GoToSecondScreen;
            retainedFragment.Presenter.DetachView();
            retainedFragment.Presenter.UnBindIntents();

            if (IsFinishing)
            {
                retainedFragment.Presenter.OnDestroy();
                retainedFragment.Presenter = null;
                retainedFragment = null;
            }
        }

        public void Render(SearchViewState state)
        {
            switch(state)
            {
                case SearchNotStartedYet notStarted:
                    StateIndicator.Text = "Search Not Started Yet!";
                    break;

                case SearchViewState.Result result:
                    StateIndicator.Text = result.Item;
                    if (string.IsNullOrEmpty(SearchView.Query))
                    {
                        SearchView.SetQuery(result.Item, false);
                        SearchView.SetIconifiedByDefault(false);
                    }
                    break;

                case Loading loading:
                    StateIndicator.Text = "Searching...please wait!";
                    break;

                case SearchViewState.Error error:
                    StateIndicator.Text = "We got an error, sorry!";
                    break;

                case EmptyResult empty:
                    StateIndicator.Text = "Sorry! Nothing found!";
                    break;
                default:
                    break;
            }
        }

        public IObservable<string> SearchIntent()
        {
            return Observable.FromEventPattern<QueryTextChangeEventArgs>
                (handler => SearchView.QueryTextChange += handler, 
                 handler => SearchView.QueryTextChange -= handler)
                .Select(e => e.EventArgs.NewText)
                .Where(query => query.Length > 3 || query.Length == 0)
                .Throttle(TimeSpan.FromMilliseconds(500));
        }
    }
}

