using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace System.Maui
{
	public static class BindableObjectExtensions
	{
		internal static void PropagateBindingContext<T>(this BindableObject self, IList<T> children)
		{
			PropagateBindingContext(self, children, BindableObject.SetInheritedBindingContext);
		}

		internal static void PropagateBindingContext<T>(this BindableObject self, IList<T> children, Action<BindableObject, object> setChildBindingContext)
		{
			if (children == null || children.Count == 0)
				return;

			var bc = self.BindingContext;

			for (var i = 0; i < children.Count; i++)
			{
				var bo = children[i] as BindableObject;
				if (bo == null)
					continue;

				setChildBindingContext(bo, bc);
			}
		}

		public static void SetBinding(this BindableObject self, BindableProperty targetProperty, string path, BindingMode mode = BindingMode.Default, IValueConverter converter = null,
									  string stringFormat = null)
		{
			if (self == null)
				throw new ArgumentNullException("self");
			if (targetProperty == null)
				throw new ArgumentNullException("targetProperty");

			var binding = new Binding(path, mode, converter, stringFormat: stringFormat);
			self.SetBinding(targetProperty, binding);
		}

		[Obsolete]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetBinding<TSource>(this BindableObject self, BindableProperty targetProperty, Expression<Func<TSource, object>> sourceProperty, BindingMode mode = BindingMode.Default,
											   IValueConverter converter = null, string stringFormat = null)
		{
			if (self == null)
				throw new ArgumentNullException(nameof(self));
			if (targetProperty == null)
				throw new ArgumentNullException(nameof(targetProperty));
			if (sourceProperty == null)
				throw new ArgumentNullException(nameof(sourceProperty));

			Binding binding = Binding.Create(sourceProperty, mode, converter, stringFormat: stringFormat);
			self.SetBinding(targetProperty, binding);
		}

		public static T GetPropertyIfSet<T>(this BindableObject bindableObject, BindableProperty bindableProperty, T returnIfNotSet)
		{
			if (bindableObject == null)
				return returnIfNotSet;

			if (bindableObject.IsSet(bindableProperty))
				return (T)bindableObject.GetValue(bindableProperty);

			return returnIfNotSet;
		}

		public static void SetOnAppTheme<T>(this BindableObject self, BindableProperty targetProperty, T light, T dark, T defaultValue = default)
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(BindableObjectExtensions), ExperimentalFlags.AppThemeExperimental, nameof(BindableObjectExtensions), nameof(SetOnAppTheme));

			var appTheme = new OnAppTheme<T> { Light = light, Dark = dark, Default = defaultValue };
			self.SetBinding(targetProperty, appTheme);
		}

		public static void SetAppThemeColor(this BindableObject self, BindableProperty targetProperty, Color light, Color dark, Color defaultValue = default)
		{
			ExperimentalFlags.VerifyFlagEnabled(nameof(BindableObjectExtensions), ExperimentalFlags.AppThemeExperimental, nameof(BindableObjectExtensions), nameof(SetAppThemeColor));

			var appTheme = new AppThemeColor { Light = light, Dark = dark, Default = defaultValue };
			self.SetBinding(targetProperty, appTheme);
		}
	}
}