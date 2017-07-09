﻿// FFXIVAPP.Memory ~ Reader.Inventory.cs
// 
// Copyright © 2007 - 2017 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVAPP.Memory.Core;
using FFXIVAPP.Memory.Core.Enums;
using FFXIVAPP.Memory.Models;
using BitConverter = FFXIVAPP.Memory.Helpers.BitConverter;

namespace FFXIVAPP.Memory
{
    public static partial class Reader
    {
        public static IntPtr HotBarMap { get; set; }

        public static HotBarReadResult GetHotBars()
        {
            var result = new HotBarReadResult();

            if (!Scanner.Instance.Locations.ContainsKey("HOTBAR"))
            {
                return result;
            }

            try
            {
                HotBarMap = Scanner.Instance.Locations["HOTBAR"];

                result.HotBarEntities = new List<HotBarEntity>
                {
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_1),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_2),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_3),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_4),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_5),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_6),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_7),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_8),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_9),
                    GetHotBar(HotBarMap, HotBar.Container.HOTBAR_10),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_1),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_2),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_3),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_4),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_5),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_6),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_7),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_HOTBAR_8),
                    GetHotBar(HotBarMap, HotBar.Container.PETBAR),
                    GetHotBar(HotBarMap, HotBar.Container.CROSS_PETBAR),
                };
            }
            catch (Exception ex)
            {
                MemoryHandler.Instance.RaiseException(Logger, ex, true);
            }

            return result;
        }

        private static HotBarEntity GetHotBar(IntPtr address, HotBar.Container type)
        {
            var containerAddress = IntPtr.Add(address, (int) type * 0xD00);

            var container = new HotBarEntity
            {
                HotBarItems = new List<HotBarItem>(),
                Type = type
            };

            var canUseKeyBinds = false;
            var itemSize = 0xD0;
            int limit;
            switch (type)
            {
                case HotBar.Container.CROSS_HOTBAR_1:
                case HotBar.Container.CROSS_HOTBAR_2:
                case HotBar.Container.CROSS_HOTBAR_3:
                case HotBar.Container.CROSS_HOTBAR_4:
                case HotBar.Container.CROSS_HOTBAR_5:
                case HotBar.Container.CROSS_HOTBAR_6:
                case HotBar.Container.CROSS_HOTBAR_7:
                case HotBar.Container.CROSS_HOTBAR_8:
                case HotBar.Container.CROSS_PETBAR:
                    limit = 16 * itemSize;
                    break;
                default:
                    limit = 12 * itemSize;
                    canUseKeyBinds = true;
                    break;
            }
            
            for (var i = 0; i < limit; i += itemSize)
            {
                var itemOffset = IntPtr.Add(containerAddress, i);
                var source = MemoryHandler.Instance.GetByteArray(itemOffset, itemSize);

                var name = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.HotBarEntity.Name);
                var slot = i / itemSize;
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }
                var item = new HotBarItem
                {
                    Name = name,
                    ID = BitConverter.TryToInt16(source, MemoryHandler.Instance.Structures.HotBarEntity.ID),
                    KeyBinds = MemoryHandler.Instance.GetStringFromBytes(source, MemoryHandler.Instance.Structures.HotBarEntity.KeyBinds),
                    Slot = slot
                };
                if (canUseKeyBinds)
                {
                    if (!string.IsNullOrWhiteSpace(item.KeyBinds))
                    {
                        item.Name = item.Name.Replace($" {item.KeyBinds}", "");
                        item.KeyBinds = Regex.Replace(item.KeyBinds, @"[\[\]]", "");
                        var buttons = item.KeyBinds.Split(new[]
                        {
                            '+'
                        }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (buttons.Count <= 0)
                        {
                            continue;
                        }
                        item.ActionKey = buttons.Last();
                        if (buttons.Count <= 1)
                        {
                            continue;
                        }
                        for (var x = 0; x < buttons.Count - 1; x++)
                        {
                            item.Modifiers.Add(buttons[x]);
                        }
                    }
                }
                container.HotBarItems.Add(item);
            }
            
            return container;
        }
    }
}
