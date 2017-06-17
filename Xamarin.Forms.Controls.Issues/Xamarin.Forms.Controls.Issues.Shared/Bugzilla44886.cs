﻿using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 9944886, "UWP Listview ItemSelected event triggered twice for each selection", PlatformAffected.UWP)]
	public class Bugzilla44886 : TestContentPage
	{
		[Preserve(AllMembers = true)]
		class MyViewModel : INotifyPropertyChanged
		{
			int _count;
			public int Count
			{
				get { return _count; }
				set
				{
					if (value != _count)
					{
						_count = value;
						RaisePropertyChanged();
					}
				}
			}

			void RaisePropertyChanged([CallerMemberName] string propertyName = null)
			{
				PropertyChangedEventHandler handler = PropertyChanged;

				handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			#region INotifyPropertyChanged implementation

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion
		}

		const string Instructions = "Select one of the items in the list. The text in blue should show 1, indicating that the ItemSelected event fired once. If it shows 2, this test has failed. Be sure to also test Keyboard selection and Narrator selection.";
		const string CountId = "countId";
		Label _CountLabel = new Label { AutomationId = CountId, TextColor = Color.Blue };
		MyViewModel _vm = new MyViewModel();

		protected override void Init()
		{
			BindingContext = _vm;

			_CountLabel.SetBinding(Label.TextProperty, nameof(MyViewModel.Count));

				var listView = new ListView
			{
				ItemsSource = new List<string> { "Item 1", "Item 2", "Item 3", "Item 4", "Item 5" }
			};
			listView.ItemSelected += ListView_ItemSelected;

			var stack = new StackLayout { Children = { new Label { Text = Instructions }, _CountLabel, listView } };
			Content = stack;
		}

		void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			_vm.Count++;
		}

#if UITEST
		[Test]
		public void Bugzilla44886Test()
		{
			RunningApp.Screenshot("I am at Issue 1");
			RunningApp.WaitForElement(q => q.Marked("Item 1"));

			int count = int.Parse(RunningApp.Query(q => q.Marked(CountId))[0].Text);

			Assert.IsTrue(count == 1);
		}
#endif
	}
}