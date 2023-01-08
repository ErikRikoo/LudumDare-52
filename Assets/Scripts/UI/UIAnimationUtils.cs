using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UI
{
	public static class UIAnimationUtils
	{
		public static void FadeIn(VisualElement visualElement, bool disabledWhenInvisible = true)
		{
			var fromOpacity = visualElement.style.opacity.value;
			
			visualElement.experimental.animation.Start(fromOpacity, 1, 300, (_, opacity) =>
			{
				visualElement.style.opacity = opacity;
				
				if (disabledWhenInvisible)
					visualElement.style.display = opacity > 0 ? DisplayStyle.Flex : DisplayStyle.None;
			}).Ease(Easing.OutCubic);
		}
		
		public static void FadeOut(VisualElement visualElement, bool disabledWhenInvisible = true)
		{
			var fromOpacity = visualElement.style.opacity.value;
			
			visualElement.experimental.animation.Start(fromOpacity, 0, 300, (_, opacity) =>
			{
				visualElement.style.opacity = opacity;
				
				if (disabledWhenInvisible)
					visualElement.style.display = opacity > 0 ? DisplayStyle.Flex : DisplayStyle.None;
			}).Ease(Easing.OutCubic);
		}
	}
}