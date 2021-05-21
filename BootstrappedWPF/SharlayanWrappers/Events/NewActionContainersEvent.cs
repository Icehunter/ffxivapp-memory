﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewActionContainersEvent.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   NewActionContainersEvent.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BootstrappedWPF.SharlayanWrappers.Events {
    using System.Collections.Concurrent;

    using Sharlayan;
    using Sharlayan.Core;

    public class NewActionContainersEvent : SharlayanDataEvent<ConcurrentBag<ActionContainer>> {
        public NewActionContainersEvent(object sender, MemoryHandler memoryHandler, ConcurrentBag<ActionContainer> eventData) : base(sender, memoryHandler, eventData) { }
    }
}