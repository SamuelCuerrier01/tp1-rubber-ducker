using Android.Views;
using Android.Widget;

namespace DuckLog
{
    public class SwipeGestureListener : Java.Lang.Object
    {
        private const int SwipeThreshold = 100;

        private readonly LinearLayout _moduleCardContent;
        private readonly LinearLayout _deleteBackground;
        private readonly int _screenWidth;

        private float _initialX;
        private bool _isSwipingLeft;

        public delegate void OnSwipeToDeleteDelegate();
        public event OnSwipeToDeleteDelegate? OnSwipeToDelete;

        public SwipeGestureListener(LinearLayout moduleCardContent, LinearLayout deleteBackground, int screenWidth)
        {
            _moduleCardContent = moduleCardContent;
            _deleteBackground = deleteBackground;
            _screenWidth = screenWidth;
        }

        public bool HandleTouchEvent(MotionEvent? e)
        {
            if (e == null) return false;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _initialX = e.GetX();
                    _isSwipingLeft = false;
                    return false; // ne bloque pas le clic

                case MotionEventActions.Move:
                    float deltaX = e.GetX() - _initialX;

                    // Si on dépasse le seuil - c'est un swipe, on bloque le clic
                    if (Math.Abs(deltaX) > SwipeThreshold)
                    {
                        _isSwipingLeft = deltaX < 0;

                        float translationX = Math.Max(deltaX, -_screenWidth);
                        _moduleCardContent.TranslationX = translationX;

                        _deleteBackground.Visibility = ViewStates.Visible;

                        return true; // IMPORTANT : bloque le clic
                    }

                    return false; // petit mouvement - clic possible

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:

                    // Si ce n'était PAS un swipe - laisser passer le clic
                    if (!_isSwipingLeft)
                        return false;

                    // Si c'était un swipe - gérer la suppression
                    float finalX = _moduleCardContent.TranslationX;

                    if (finalX < -_screenWidth * 0.5f)
                        OnSwipeToDelete?.Invoke();
                    else
                        ResetSwipe();

                    return true; // IMPORTANT : empêche le clic après un swipe
            }

            return false;
        }


        private void ResetSwipe(bool animated = true)
        {
            if (animated)
            {
                _moduleCardContent.Animate()
                    .TranslationX(0)
                    .SetDuration(200)
                    .Start();
            }
            else
            {
                _moduleCardContent.TranslationX = 0;
            }

            _deleteBackground.Visibility = ViewStates.Gone;
            _isSwipingLeft = false;
        }
    }
}
