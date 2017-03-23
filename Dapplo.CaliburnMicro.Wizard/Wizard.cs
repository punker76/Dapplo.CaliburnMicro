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
using System.Linq;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Log;

#endregion

namespace Dapplo.CaliburnMicro.Wizard
{
    /// <summary>
    ///     This implements a Caliburn-Micro wizard
    /// </summary>
    public class Wizard<TWizardScreen> : Conductor<TWizardScreen>.Collection.OneActive, IWizard<TWizardScreen>
        where TWizardScreen : class, IWizardScreen
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly LogSource Log = new LogSource();

        private IDisposable _screenObservable;

        /// <summary>
        ///     Helper Property to get the next screen
        /// </summary>
        private TWizardScreen NextScreen
        {
            get
            {
                // Skip as long as there is a CurrentWizardScreen, and the item is not the current, skip 1 (the current) and skip as long as the item can not be shown.
                return WizardScreens.OrderBy(x => x.Order).SkipWhile(w => w != CurrentWizardScreen).Skip(1).SkipWhile(w => !w.IsEnabled || !w.IsVisible).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Helper Property to get the previous screen
        /// </summary>
        private TWizardScreen PreviousScreen
        {
            get
            {
                if (CurrentWizardScreen == null)
                {
                    return null;
                }
                return WizardScreens.OrderBy(x => x.Order).TakeWhile(w => w != CurrentWizardScreen).LastOrDefault(w => w.IsEnabled && w.IsVisible);
            }
        }

        /// <summary>
        ///     This is called when the wizard needs to initialize stuff, it will call Initialize on every screen
        /// </summary>
        public virtual void Initialize()
        {
            Log.Verbose().WriteLine("Initializing wizard");

            foreach (var wizardScreen in WizardScreens.OrderBy(x => x.Order))
            {
                wizardScreen.ParentWizard = this;
                wizardScreen.Initialize();
            }
        }

        /// <summary>
        ///     This is called when the wizard needs to cleanup things, it will call Terminate on every screen
        /// </summary>
        public virtual void Terminate()
        {
            _screenObservable?.Dispose();
            Log.Verbose().WriteLine("Terminating wizard");
            foreach (var wizardScreen in WizardScreens.OrderBy(x => x.Order))
            {
                wizardScreen.Terminate();
            }
        }

        /// <summary>
        ///     The current progress, from 0 when no screen is active until 100 when the last screen is active
        /// </summary>
        public int Progress
        {
            get
            {
                // Is there already a screen selected?
                if (CurrentWizardScreen == null)
                {
                    return 0;
                }

                int availableScreens = WizardScreens.Count(x => x.IsVisible);
                int index = WizardScreens.OrderBy(x => x.Order).TakeWhile(w => w != CurrentWizardScreen).Count(x => x.IsVisible) + 1;
                return (int) (100 * ((double) index / availableScreens));
            }
        }

        /// <summary>
        ///     Test if we are at the last screen
        /// </summary>
        public bool IsLast
        {
            get { return WizardScreens.OrderBy(x => x.Order).LastOrDefault(x => x.IsEnabled && x.IsVisible) == CurrentWizardScreen; }
        }

        /// <summary>
        ///     Test if we are at the first screen
        /// </summary>
        public bool IsFirst
        {
            get { return WizardScreens.OrderBy(x => x.Order).First() == CurrentWizardScreen; }
        }

        /// <summary>
        ///     The TWizardScreen items of the wizard
        /// </summary>
        public virtual IEnumerable<TWizardScreen> WizardScreens { get; set; }

        /// <summary>
        ///     This implements IWizard.WizardScreens via ConfigScreens
        /// </summary>
        IEnumerable<IWizardScreen> IWizard.WizardScreens
        {
            get { return WizardScreens; }
            set { WizardScreens = value as IEnumerable<TWizardScreen>; }
        }

        /// <summary>
        ///     This return or sets the current wizard screen
        /// </summary>
        public virtual TWizardScreen CurrentWizardScreen
        {
            get { return ActiveItem; }
            set { ActivateItem(value); }
        }

        /// <summary>
        ///     This implements IWizard.CurrentWizardScreen via CurrentConfigScreen
        /// </summary>
        IWizardScreen IWizard.CurrentWizardScreen
        {
            get { return CurrentWizardScreen; }
            set { CurrentWizardScreen = value as TWizardScreen; }
        }

        /// <summary>
        ///     Changes the ActiveItem of the conductor to the next IWizardScreen
        /// </summary>
        /// <returns>bool if next was possible</returns>
        public virtual bool Next()
        {
            if (CurrentWizardScreen?.IsComplete != true)
            {
                return false;
            }
            var nextWizardScreen = NextScreen;
            if (nextWizardScreen == null)
            {
                return false;
            }
            CurrentWizardScreen = nextWizardScreen;
            return true;
        }

        /// <summary>
        ///     This returns true we can move to the next screen, which is true when:
        ///     1) if the current IsComplete
        ///     2) if there is a IWizardScreen after the current
        /// </summary>
        /// <returns>true if next can be called</returns>
        public virtual bool CanNext => CurrentWizardScreen?.IsComplete == true && NextScreen != null;

        /// <summary>
        ///     Goto the previous
        /// </summary>
        /// <returns>bool if previous was possible</returns>
        public virtual bool Previous()
        {
            // Take until 
            var previousWizardScreen = PreviousScreen;
            if (previousWizardScreen == null)
            {
                return false;
            }
            CurrentWizardScreen = previousWizardScreen;
            return true;
        }


        /// <summary>
        ///     Is there a previous WizardScreen?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanPrevious => PreviousScreen != null;

        /// <summary>
        ///     This will call TryClose with false if all IWizardScreen items are okay with closing
        /// </summary>
        public virtual void Cancel()
        {
            if (CanCancel)
            {
                TryClose(false);
            }
        }

        /// <summary>
        ///     check every IWizardScreen if it can close
        /// </summary>
        public virtual bool CanCancel
        {
            get
            {
                var result = true;
                CanClose(b => result = b);
                return result;
            }
        }

        /// <summary>
        ///     This will call TryClose with true if all IWizardScreen items are okay with closing
        /// </summary>
        public virtual void Finish()
        {
            if (CanFinish)
            {
                TryClose(true);
            }
        }

        /// <summary>
        ///     check every IWizardScreen if it can close
        /// </summary>
        public virtual bool CanFinish
        {
            get
            {
                var result = true;
                CanClose(b => result = b);
                return result;
            }
        }

        /// <summary>
        ///     Activates the specified item, and sends notify property changed events.
        /// </summary>
        /// <param name="item">The TWizardScreen to activate.</param>
        public override void ActivateItem(TWizardScreen item)
        {
            _screenObservable?.Dispose();
            base.ActivateItem(item);
            _screenObservable = item.OnPropertyChanged(nameof(IWizardScreen.IsComplete)).Subscribe(args => NotifyOfPropertyChange(nameof(CanNext)));
            NotifyOfPropertyChange(nameof(CurrentWizardScreen));
            NotifyOfPropertyChange(nameof(Progress));
            NotifyOfPropertyChange(nameof(CanNext));
            NotifyOfPropertyChange(nameof(CanPrevious));
            NotifyOfPropertyChange(nameof(CanCancel));
            NotifyOfPropertyChange(nameof(CanFinish));
            NotifyOfPropertyChange(nameof(IsFirst));
            NotifyOfPropertyChange(nameof(IsLast));
        }

        /// <summary>
        ///     Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="callback">The implementor calls this action with the result of the close check.</param>
        public override void CanClose(Action<bool> callback)
        {
            var result = true;
            foreach (var wizardScreen in WizardScreens.OrderBy(x => x.Order))
            {
                wizardScreen.CanClose(canClose => result &= canClose);
            }
            callback(result);
        }

        /// <summary>Called when activating.</summary>
        protected override void OnActivate()
        {
            // Order the items by ordering on Order
            Items.AddRange(WizardScreens.OrderBy(x => x.Order));

            Initialize();

            base.OnActivate();
        }

        /// <summary>
        ///     Tries to close this instance by asking its Parent to initiate shutdown or by asking its corresponding view to
        ///     close.
        ///     Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        public override void TryClose(bool? dialogResult = null)
        {
            // Terminate needs to be called before TryClose, otherwise our items are gone.
            Terminate();
            base.TryClose(dialogResult);
        }
    }
}