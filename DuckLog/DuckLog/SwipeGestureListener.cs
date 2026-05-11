using Android.Views;

namespace DuckLog
{
    public class SwipeGestureListener : Java.Lang.Object
    {
        private const int SwipeThreshold = 100;
        
        private LinearLayout? _moduleCardContent;
        private LinearLayout? _deleteBackground;
        private float _initialX;
        private bool _isSwipingLeft;
        private readonly int _screenWidth;
        
        public delegate void OnSwipeToDeleteDelegate();
        public event OnSwipeToDeleteDelegate? OnSwipeToDelete;
        
        public SwipeGestureListener(LinearLayout moduleCardContent, LinearLayout deleteBackground, int screenWidth)
        {
            _moduleCardContent = moduleCardContent;
            _deleteBackground = deleteBackground;
            _screenWidth = screenWidth;
            _isSwipingLeft = false;
        }
        
        public bool HandleTouchEvent(MotionEvent? e)
        {
            if (e == null)
            {
                return false;
            }
            
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    _initialX = e.GetX();
                    _isSwipingLeft = false;
                    return true;
                    
                case MotionEventActions.Move:
                    float deltaX = e.GetX() - _initialX;
                    
                    if (deltaX < -SwipeThreshold)
                    {
                        _isSwipingLeft = true;
                        
                        // Calculer la translationX en fonction du déplacement, mais ne pas dépasser la largeur de l'écran
                        float translationX = deltaX;
                        if (translationX < -_screenWidth) translationX = -_screenWidth;
                        
                        if (_moduleCardContent != null)
                        {
                            _moduleCardContent.TranslationX = translationX;
                        }
                        
                        // Montrer le fond de suppression si on commence à swiper vers la gauche
                        if (_deleteBackground != null)
                        {
                            _deleteBackground.Visibility = ViewStates.Visible;
                        }
                    }
                    else if (_isSwipingLeft && deltaX > -SwipeThreshold / 2.0f)
                    {
                        // Réinitialiser la position si l'utilisateur revient en arrière après avoir commencé à swiper
                        ResetSwipe(animated: true);
                    }
                    return true;
                    
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    if (_isSwipingLeft && _moduleCardContent != null)
                    {
                        float translationX = _moduleCardContent.TranslationX;
                        
                        // Si le swipe dépasse la moitié de la largeur de l'écran, déclencher la suppression
                        if (translationX < -_screenWidth * 0.5f)
                        {
                            OnSwipeToDelete?.Invoke();
                        }
                        else
                        {
                            // Remet l'animation à zéro (position initale)
                            ResetSwipe();
                        }
                    }
                    _isSwipingLeft = false;
                    return true;
            }
            
            return false;
        }
        private void ResetSwipe(bool animated = true)
        {
            if (animated)
            {
                _moduleCardContent?.Animate()
                    .TranslationX(0)
                    .SetDuration(600)
                    .Start();
            }
            else if (_moduleCardContent != null)
            {
                _moduleCardContent.TranslationX = 0;
            }

            if (_deleteBackground != null)
                _deleteBackground.Visibility = ViewStates.Gone;

            _isSwipingLeft = false;
        }
    }
}


