
using Android.App;
using Android.OS;

namespace State.SearchView
{
    public class SearchFragment : Fragment
    {
        public SearchPresenter Presenter { get; set; }
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RetainInstance = true;
        }
    }
}