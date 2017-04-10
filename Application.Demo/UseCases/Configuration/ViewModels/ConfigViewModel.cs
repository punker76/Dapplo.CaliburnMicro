﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.CaliburnMicro
// 
//  Dapplo.CaliburnMicro is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.CaliburnMicro is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.CaliburnMicro. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Application.Demo.Languages;
using Application.Demo.Models;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Language;

#endregion

namespace Application.Demo.UseCases.Configuration.ViewModels
{
    /// <summary>
    ///     The settings view model is, well... for the settings :)
    ///     It is a conductor where one item is active.
    /// </summary>
    [Export]
    public class ConfigViewModel : Config<IConfigScreen>, IPartImportsSatisfiedNotification
    {
        /// <summary>
        ///     Get all settings controls, these are the items that are displayed.
        /// </summary>
        [ImportMany]
        public override IEnumerable<Lazy<IConfigScreen>> ConfigScreens { get; set; }

        [Import]
        public IConfigTranslations ConfigTranslations { get; set; }

        [Import]
        public ICoreTranslations CoreTranslations { get; set; }

        [Import]
        private IDemoConfiguration DemoConfiguration { get; set; }

        /// <summary>
        ///     Used to send events
        /// </summary>
        [Import]
        private IEventAggregator EventAggregator { get; set; }

        public void OnImportsSatisfied()
        {
            // automatically update the DisplayName
            CoreTranslations.CreateDisplayNameBinding(this, nameof(ICoreTranslations.Settings));

            // Set the current language (this should update all registered OnPropertyChanged anyway, so it can run in the background
            var lang = DemoConfiguration.Language;
            Task.Run(async () => await LanguageLoader.Current.ChangeLanguageAsync(lang).ConfigureAwait(false));
        }
    }
}