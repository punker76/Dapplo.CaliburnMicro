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

using System.ComponentModel;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Config.Language;

#endregion

namespace Dapplo.CaliburnMicro.Demo.Languages
{
	[Language("Config")]
	public interface IConfigTranslations : Translations.IConfigTranslations, ILanguage, INotifyPropertyChanged
	{
		string Ui { get; }
		string Addons { get; }
		string RestoreDefaults { get; }
		string Apply { get; }
		string Theme { get; }
		string Hotkey { get; }
	}
}