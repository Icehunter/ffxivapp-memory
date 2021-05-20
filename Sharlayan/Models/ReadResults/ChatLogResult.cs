﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatLogResult.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   ChatLogResult.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan.Models.ReadResults {
    using System.Collections.Generic;

    using Sharlayan.Core;

    public class ChatLogResult {
        public List<ChatLogItem> ChatLogItems { get; internal set; } = new List<ChatLogItem>();

        public int PreviousArrayIndex { get; internal set; }

        public int PreviousOffset { get; internal set; }
    }
}