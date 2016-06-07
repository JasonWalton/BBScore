using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BBScore
{
	[ContentProperty(nameof(Bindings))]
	public class MultiBinding : IMarkupExtension<Binding>
	{
		public IList<Binding> Bindings { get; } = new List<Binding>();

		public string StringFormat { get; set; }

		public Binding ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}

		object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
		{
			return ProvideValue(serviceProvider);
		}
	}
}