using Android.Views;
using Android.Views;
using Android.Views.InputMethods;
using Android.Runtime;
using Java.Lang;
namespace DuckLog
{
    public class SwipeTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        private SwipeGestureListener _swipeListener;
        
        public SwipeTouchListener(SwipeGestureListener swipeListener)
        {
            _swipeListener = swipeListener;
        }
        
        public bool OnTouch(View? v, MotionEvent? e)
        {
            if (e == null) return false;
            return _swipeListener.HandleTouchEvent(e);
        }
    }
}

