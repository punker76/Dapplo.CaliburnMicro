﻿#region Dapplo 2016 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
// Copyright (C) 2016 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.CaliburnMicro
// 
// Dapplo.CaliburnMicro is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.CaliburnMicro is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.CaliburnMicro. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#endregion

#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Tree;

#endregion

namespace Dapplo.CaliburnMicro.Menu
{
	/// <summary>
	///     Extend this to make your IMenuItem
	/// </summary>
	public class MenuItem : PropertyChangedBase, IMenuItem
	{
		private string _displayName;
		private Control _icon;
		private string _id;
		private bool _isEnabled = true;
		private bool _isVisible = true;

		/// <summary>
		/// Default constructor take the name of the type for the Id
		/// </summary>
		public MenuItem()
		{
			_id = GetType().Name;
		}

		/// <summary>
		///     This make it NOT shown as a separator
		/// </summary>
		public bool IsSeparator { get; set; } = false;

		/// <summary>
		///     Returns if the IMenuItem is enabled
		/// </summary>
		public virtual bool IsEnabled
		{
			get { return _isEnabled; }
			protected set
			{
				_isEnabled = value;
				NotifyOfPropertyChange(nameof(IsEnabled));
			}
		}

		/// <summary>
		///     Returns if the IMenuItem is visible
		/// </summary>
		public virtual bool IsVisible
		{
			get { return _isVisible; }
			protected set
			{
				_isVisible = value;
				NotifyOfPropertyChange(nameof(IsVisible));
			}
		}

		/// <summary>
		///     Returns the icon of the IMenuItem
		/// </summary>
		public virtual Control Icon
		{
			get { return _icon; }
			protected set
			{
				_icon = value;
				NotifyOfPropertyChange(nameof(Icon));
			}
		}

		/// <summary>
		///     Returns the DisplayName of the IMenuItem
		/// </summary>
		public virtual string DisplayName
		{
			get { return _displayName ?? GetType().Name; }
			set
			{
				_displayName = value;
				NotifyOfPropertyChange(nameof(DisplayName));
			}
		}

		/// <summary>
		///     Is called when the IMenuItem is clicked
		/// </summary>
		public virtual void Click(IMenuItem clickedItem)
		{
			ClickAction?.Invoke(clickedItem);
		}

		/// <summary>
		/// This action is called when Click is invoked
		/// </summary>
		public System.Action<IMenuItem> ClickAction { get; set; }

		#region ITreeNode

		/// <summary>
		///     Used to showing this inside a tree
		/// </summary>
		public virtual ITreeNode<IMenuItem> ParentNode { get; set; }

		/// <summary>
		///     Used to showing this inside a tree
		/// </summary>
		public virtual ICollection<ITreeNode<IMenuItem>> ChildNodes { get; set; } = new ObservableCollection<ITreeNode<IMenuItem>>();

		/// <summary>
		///     The parent under which the IMenuItem is shown, null is root
		/// </summary>
		public virtual string ParentId { get; set; } = null;

		/// <summary>
		///     The Id of this IMenuItem, is also used to order children of a parent
		///     Default the Id is the name of the Type
		/// </summary>
		public virtual string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		#endregion
	}
}